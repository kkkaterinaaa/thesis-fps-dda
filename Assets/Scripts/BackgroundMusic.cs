using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    public const string VolumePrefsKey = "settings.musicVolume";

    public static BackgroundMusic Instance { get; private set; }

    [Header("Music")]
    public AudioClip musicClip;
    [Range(0f, 1f)]
    public float volume = 0.3f;
    public bool loop = true;

    private AudioSource audioSource;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Use the saved volume if the player has changed it before.
        volume = PlayerPrefs.GetFloat(VolumePrefsKey, volume);

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = musicClip;
        audioSource.volume = volume;
        audioSource.loop = loop;
        audioSource.playOnAwake = false;

        if (musicClip == null)
            Debug.LogWarning("BackgroundMusic: Music Clip is not assigned — no music will play and the volume slider will have no audible effect.", this);
        else
            audioSource.Play();
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    // Returns the saved music volume (0..1), or the given default if none saved yet.
    public static float GetSavedVolume(float fallback = 0.3f)
    {
        return PlayerPrefs.GetFloat(VolumePrefsKey, fallback);
    }

    // Sets, applies and persists the music volume (0..1).
    public static void SetVolume(float value)
    {
        value = Mathf.Clamp01(value);

        PlayerPrefs.SetFloat(VolumePrefsKey, value);
        PlayerPrefs.Save();

        if (Instance != null)
        {
            Instance.volume = value;
            if (Instance.audioSource != null)
                Instance.audioSource.volume = value;
        }
    }
}
