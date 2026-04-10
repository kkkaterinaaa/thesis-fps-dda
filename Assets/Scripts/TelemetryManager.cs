using System;
using System.IO;
using UnityEngine;

public class TelemetryManager : MonoBehaviour
{
    public static TelemetryManager Instance { get; private set; }

    [Serializable]
    public class SessionData
    {
        public string sessionId;
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

    public bool writeJsonFile = true;
    public bool logSummaryToConsole = true;
    public string outputFolderOverride = "";

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

        StartSession();
    }

    private void StartSession()
    {
        ended = false;
        startTime = Time.realtimeSinceStartup;

        data = new SessionData();
        data.sessionId = Guid.NewGuid().ToString("N");
        data.startedAtUtc = DateTime.UtcNow.ToString("o");
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

        if (writeJsonFile)
            WriteJson();

        if (logSummaryToConsole)
            Debug.Log($"Telemetry: session={data.sessionId} dur={data.durationSeconds:0.0}s acc={data.accuracy:0.00} kills={data.enemiesKilled} dmgTaken={data.damageTaken:0.##} score={data.skillScore:0.0}");
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

    private void WriteJson()
    {
        try
        {
            string folder = GetOutputFolder();
            Directory.CreateDirectory(folder);

            string fileName = $"telemetry_{DateTime.UtcNow:yyyyMMdd_HHmmss}_{data.sessionId}.json";
            string path = Path.Combine(folder, fileName);

            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(path, json);

            if (logSummaryToConsole)
                Debug.Log($"Telemetry written: {path}");
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Telemetry write failed: {e.Message}");
        }
    }

    private string GetOutputFolder()
    {
        if (!string.IsNullOrWhiteSpace(outputFolderOverride))
            return outputFolderOverride;

#if UNITY_EDITOR
        return Path.GetFullPath(Path.Combine(Application.dataPath, "..", "telemetry"));
#else
        return Path.Combine(Application.persistentDataPath, "telemetry");
#endif
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
