using UnityEngine;

public class PlayerIdProvider : MonoBehaviour, IPlayerIdProvider
{
    private const string PrefsKey = "dda.anonymousPlayerId";

    [Tooltip("If true, the provider survives scene loads")]
    public bool dontDestroyOnLoad = true;

    public static PlayerIdProvider Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        if (dontDestroyOnLoad)
            DontDestroyOnLoad(gameObject);
    }

    public string GetPlayerId()
    {
        return PlayerPrefs.GetString(PrefsKey, "");
    }

    public void SetPlayerId(string id)
    {
        if (string.IsNullOrWhiteSpace(id)) return;
        string trimmed = id.Trim();

        string previous = PlayerPrefs.GetString(PrefsKey, "");
        PlayerPrefs.SetString(PrefsKey, trimmed);
        PlayerPrefs.Save();

        if (previous != trimmed && ExperimentFlow.Instance != null)
            ExperimentFlow.Instance.ResetExperiment();
    }

    public bool HasPlayerId()
    {
        return !string.IsNullOrWhiteSpace(PlayerPrefs.GetString(PrefsKey, ""));
    }

    public void Clear()
    {
        PlayerPrefs.DeleteKey(PrefsKey);
        PlayerPrefs.Save();
    }
}
