using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    [Header("UI")]
    [Tooltip("Root panel for the settings UI. Hidden by default.")]
    public GameObject settingsPanel;

    [Tooltip("Slider that controls music volume (set its Min Value = 0, Max Value = 1).")]
    public Slider musicVolumeSlider;

    [Tooltip("Optional label that shows the current volume as a percentage.")]
    public TMP_Text musicVolumeLabel;

    [Tooltip("Panel to hide while settings are open and re-show when Back is pressed (e.g. the pause or main menu panel). Optional.")]
    public GameObject returnPanel;

    [Header("Navigation (optional)")]
    [Tooltip("Button that opens this settings panel (e.g. the 'Settings' button on a menu).")]
    public Button openButton;
    [Tooltip("Button that closes this settings panel and returns to the previous menu.")]
    public Button backButton;

    void Awake()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
        else Debug.LogWarning("SettingsMenu: Settings Panel is not assigned.", this);

        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.minValue = 0f;
            musicVolumeSlider.maxValue = 1f;
            musicVolumeSlider.value = BackgroundMusic.GetSavedVolume();
            musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        }
        else Debug.LogWarning("SettingsMenu: Music Volume Slider is not assigned.", this);

        if (backButton != null) backButton.onClick.AddListener(Close);
        else Debug.LogWarning("SettingsMenu: Back Button is not assigned.", this);

        UpdateLabel(BackgroundMusic.GetSavedVolume());

        if (openButton != null)
        {
            Debug.LogWarning("SettingsMenu: Open Button is assigned. Leave this empty when PauseMenu/MainMenu handle opening — having both causes double-open.", this);
            openButton.onClick.AddListener(Open);
        }
    }

    // Opens settings and remembers which panel to return to.
    public void Open(GameObject panelToReturnTo)
    {
        if (panelToReturnTo == null)
            Debug.LogWarning("SettingsMenu: Open was called with a null return panel — Back button will not restore any panel. Check that Pause Panel is assigned in PauseMenu.", this);
        returnPanel = panelToReturnTo;
        Open();
    }

    public void Open()
    {
        Debug.Log("SettingsMenu: Open called.");
        if (returnPanel != null) returnPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(true);

        // Re-sync the slider in case the value changed elsewhere.
        if (musicVolumeSlider != null)
            musicVolumeSlider.SetValueWithoutNotify(BackgroundMusic.GetSavedVolume());

        UpdateLabel(BackgroundMusic.GetSavedVolume());
    }

    public void Close()
    {
        string rpName = returnPanel != null ? returnPanel.name : "NULL";
        Debug.Log("SettingsMenu: Close called. returnPanel = '" + rpName + "'.");
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (returnPanel != null)
        {
            returnPanel.SetActive(true);
            Debug.Log($"SettingsMenu: Restored '{returnPanel.name}' (active={returnPanel.activeSelf}).");
        }
        else
            Debug.LogWarning("SettingsMenu: returnPanel is null on Close — nothing to return to.", this);
    }

    private void OnMusicVolumeChanged(float value)
    {
        string bgmName = BackgroundMusic.Instance != null ? BackgroundMusic.Instance.name : "NULL";
        Debug.Log("SettingsMenu: Volume slider -> " + value.ToString("0.00") + ". BackgroundMusic.Instance=" + bgmName + ".");
        if (BackgroundMusic.Instance == null)
            Debug.LogWarning("SettingsMenu: BackgroundMusic instance is null — add a BackgroundMusic object to this scene.", this);
        BackgroundMusic.SetVolume(value);
        UpdateLabel(value);
    }

    private void UpdateLabel(float value)
    {
        if (musicVolumeLabel != null)
            musicVolumeLabel.text = Mathf.RoundToInt(value * 100f) + "%";
    }
}
