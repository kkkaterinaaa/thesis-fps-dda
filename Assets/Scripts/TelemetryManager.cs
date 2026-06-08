using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TelemetryManager : MonoBehaviour
{
    public static TelemetryManager Instance { get; private set; }

    [Serializable]
    public class SessionData
    {
        public string startedAtUtc;
        public string endedAtUtc;
        public string endReason;

        public float durationSeconds;

        public int shotsFired;
        public int shotsHit;
        public int headHits;
        public int reloadsAuto;
        public int reloadsManual;

        public float damageDealt;
        public float damageTaken;
        public float damageBlockedByArmor;

        public int enemiesKilled;
        public int deaths;

        public int pickupsArmor;
        public int pickupsMedkit;
        public int pickupsMagazines;

        public float medkitHealed;
        public float armorGained;
        public int ammoGained;

        public float accuracy;
        public float killsPerMinute;
        public float damageTakenPerMinute;
        public float skillScore;
    }

    public bool logSummaryToConsole = true;

    private SessionData data;
    private float startTime;
    private bool ended;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;

        StartSession();
    }

    void OnDestroy()
    {
        if (Instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (ended)
            StartSession();
    }

    private void StartSession()
    {
        ended = false;
        startTime = Time.realtimeSinceStartup;

        data = new SessionData();
        data.startedAtUtc = DateTime.UtcNow.ToString("o");

        RunSessionEvents.RaiseRunStarted();
    }

    void OnApplicationQuit()
    {
        EndSession("quit");
    }

    public void EndSession(string reason)
    {
        if (ended) return;
        ended = true;

        data.endReason = reason;
        data.endedAtUtc = DateTime.UtcNow.ToString("o");
        data.durationSeconds = Mathf.Max(0f, Time.realtimeSinceStartup - startTime);

        ComputeDerived();

        string json = JsonUtility.ToJson(data, true);

        RunSessionEvents.RaiseRunEnded(json);

        if (logSummaryToConsole)
            Debug.Log($"Telemetry: dur={data.durationSeconds:0.0}s acc={data.accuracy:0.00} kills={data.enemiesKilled} dmgTaken={data.damageTaken:0.##} score={data.skillScore:0.0}");
    }

    private void ComputeDerived()
    {
        data.accuracy = data.shotsFired > 0 ? (float)data.shotsHit / data.shotsFired : 0f;

        float minutes = data.durationSeconds / 60f;
        data.killsPerMinute = minutes > 0f ? data.enemiesKilled / minutes : 0f;
        data.damageTakenPerMinute = minutes > 0f ? data.damageTaken / minutes : data.damageTaken;

        float accScore = Mathf.Clamp01(data.accuracy) * 40f;
        float kpmScore = Mathf.Clamp01(data.killsPerMinute / 3f) * 30f;
        float survivalScore = Mathf.Clamp01(data.durationSeconds / 300f) * 20f;
        float dmgPenalty = Mathf.Clamp01(data.damageTakenPerMinute / 200f) * 20f;

        data.skillScore = Mathf.Clamp(accScore + kpmScore + survivalScore - dmgPenalty, 0f, 100f);
    }

    public static void RecordShotFired()
    {
        if (Instance == null) return;
        Instance.data.shotsFired++;
    }

    public static void RecordShotHit(bool isHead, float damageDealt)
    {
        if (Instance == null) return;
        Instance.data.shotsHit++;
        if (isHead) Instance.data.headHits++;
        Instance.data.damageDealt += Mathf.Max(0f, damageDealt);
    }

    public static void RecordReload(bool manual)
    {
        if (Instance == null) return;
        if (manual) Instance.data.reloadsManual++;
        else Instance.data.reloadsAuto++;
    }

    public static void RecordPlayerDamage(float incomingDamage, float blockedByArmor, float damageToHealth)
    {
        if (Instance == null) return;
        Instance.data.damageTaken += Mathf.Max(0f, damageToHealth);
        Instance.data.damageBlockedByArmor += Mathf.Max(0f, blockedByArmor);
    }

    public static void RecordEnemyKilled()
    {
        if (Instance == null) return;
        Instance.data.enemiesKilled++;
    }

    public static void RecordPlayerDeath()
    {
        if (Instance == null) return;
        Instance.data.deaths++;
    }

    public static void RecordArmorPickup(float amount)
    {
        if (Instance == null) return;
        Instance.data.pickupsArmor++;
        Instance.data.armorGained += Mathf.Max(0f, amount);
    }

    public static void RecordMedkitPickup(float healed)
    {
        if (Instance == null) return;
        Instance.data.pickupsMedkit++;
        Instance.data.medkitHealed += Mathf.Max(0f, healed);
    }

    public static void RecordMagazinePickup(int cartridges)
    {
        if (Instance == null) return;
        Instance.data.pickupsMagazines++;
        Instance.data.ammoGained += Mathf.Max(0, cartridges);
    }
}
