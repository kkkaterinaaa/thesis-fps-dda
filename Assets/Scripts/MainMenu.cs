using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Scene To Load On Play")]
    [Tooltip("Name of the gameplay scene to load. Must be added to Build Settings.")]
    public string gameSceneName = "level";

    [Header("Buttons")]
    public Button playButton;
    public Button settingsButton;
    public Button quitButton;

    [Header("Settings")]
    [Tooltip("SettingsMenu opened by the Settings button. Optional.")]
    public SettingsMenu settingsMenu;

    [Header("Optional Panels")]
    [Tooltip("Main buttons panel, shown by default.")]
    public GameObject mainPanel;
    [Tooltip("Credits/about panel, hidden by default. Optional.")]
    public GameObject creditsPanel;
    [Tooltip("Opens the credits panel. Optional.")]
    public Button creditsButton;
    [Tooltip("Returns from the credits panel to the main panel. Optional.")]
    public Button creditsBackButton;

    void Start()
    {
        // A menu is never paused and always shows the cursor.
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (mainPanel != null) mainPanel.SetActive(true);
        if (creditsPanel != null) creditsPanel.SetActive(false);

        if (playButton != null) playButton.onClick.AddListener(Play);
        if (settingsButton != null) settingsButton.onClick.AddListener(OpenSettings);
        if (quitButton != null) quitButton.onClick.AddListener(QuitGame);
        if (creditsButton != null) creditsButton.onClick.AddListener(ShowCredits);
        if (creditsBackButton != null) creditsBackButton.onClick.AddListener(ShowMain);
    }

    public void Play()
    {
        if (string.IsNullOrWhiteSpace(gameSceneName))
        {
            Debug.LogError("MainMenu: gameSceneName is not set.", this);
            return;
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene(gameSceneName);
    }

    public void OpenSettings()
    {
        if (settingsMenu != null) settingsMenu.Open(mainPanel);
    }

    public void ShowCredits()
    {
        if (mainPanel != null) mainPanel.SetActive(false);
        if (creditsPanel != null) creditsPanel.SetActive(true);
    }

    public void ShowMain()
    {
        if (creditsPanel != null) creditsPanel.SetActive(false);
        if (mainPanel != null) mainPanel.SetActive(true);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
