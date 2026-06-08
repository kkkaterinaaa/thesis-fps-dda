using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu Instance { get; private set; }

    [Header("UI")]
    [Tooltip("Root panel that contains the pause menu UI. Hidden while playing.")]
    public GameObject pausePanel;
    public Button resumeButton;
    public Button restartButton;
    public Button settingsButton;
    public Button mainMenuButton;
    public Button quitButton;

    [Header("Settings")]
    [Tooltip("SettingsMenu opened by the Settings button. Optional.")]
    public SettingsMenu settingsMenu;

    [Header("Main Menu")]
    [Tooltip("Name of the start-menu scene loaded by the Main Menu button. Must be in Build Settings.")]
    public string mainMenuSceneName = "MainMenu";

    [Header("Input")]
    [Tooltip("Key that toggles the pause menu.")]
    public KeyCode toggleKey = KeyCode.Escape;

    [Header("Block These Scripts While Paused")]
    [Tooltip("These MonoBehaviours are disabled while the menu is open and re-enabled on resume (e.g. PlayerController, RaycastGun).")]
    public MonoBehaviour[] blockScripts;

    public bool IsPaused { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (pausePanel != null) pausePanel.SetActive(false);
        else Debug.LogWarning("PauseMenu: Pause Panel is not assigned.", this);

        if (resumeButton != null) resumeButton.onClick.AddListener(Resume);
        else Debug.LogWarning("PauseMenu: Resume Button is not assigned.", this);

        if (restartButton != null) restartButton.onClick.AddListener(Restart);
        else Debug.LogWarning("PauseMenu: Restart Button is not assigned.", this);

        if (settingsButton != null) settingsButton.onClick.AddListener(OpenSettings);
        else Debug.LogWarning("PauseMenu: Settings Button is not assigned.", this);

        if (settingsMenu != null) { } // assigned, good
        else Debug.LogWarning("PauseMenu: Settings Menu is not assigned — settings button will do nothing.", this);

        if (mainMenuButton != null) mainMenuButton.onClick.AddListener(GoToMainMenu);

        if (quitButton != null) quitButton.onClick.AddListener(QuitGame);
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    void Update()
    {
        if (!Input.GetKeyDown(toggleKey)) return;

        if (IsPaused) Resume();
        else if (CanPause()) Pause();
    }

    private bool CanPause()
    {
        // Do not steal control from the death/win end screens.
        if (DeathScreen.Instance != null && DeathScreen.Instance.deathPanel != null &&
            DeathScreen.Instance.deathPanel.activeInHierarchy)
            return false;

        if (WinScreen.Instance != null && WinScreen.Instance.winPanel != null &&
            WinScreen.Instance.winPanel.activeInHierarchy)
            return false;

        return true;
    }

    public void Pause()
    {
        IsPaused = true;

        if (pausePanel != null) pausePanel.SetActive(true);

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SetBlockedScripts(false);
    }

    public void Resume()
    {
        Debug.Log("PauseMenu: Resume called.");
        IsPaused = false;

        if (pausePanel != null) pausePanel.SetActive(false);

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        SetBlockedScripts(true);
    }

    public void Restart()
    {
        Debug.Log("PauseMenu: Restart called.");
        IsPaused = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OpenSettings()
    {
        Debug.Log("PauseMenu: OpenSettings called.");
        if (settingsMenu != null)
            settingsMenu.Open(pausePanel);
        else
            Debug.LogWarning("PauseMenu: settingsMenu is null, cannot open settings.", this);
    }

    public void GoToMainMenu()
    {
        Debug.Log($"PauseMenu: GoToMainMenu called — loading '{mainMenuSceneName}'.");
        IsPaused = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void QuitGame()
    {
        Debug.Log("PauseMenu: QuitGame called.");
        Time.timeScale = 1f;

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void SetBlockedScripts(bool enabled)
    {
        if (blockScripts == null) return;
        for (int i = 0; i < blockScripts.Length; i++)
            if (blockScripts[i] != null) blockScripts[i].enabled = enabled;
    }
}
