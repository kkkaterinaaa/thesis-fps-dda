using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinScreen : MonoBehaviour
{
    public static WinScreen Instance { get; private set; }

    public GameObject winPanel;

    [Header("Buttons")]
    public Button restartButton;
    public Button mainMenuButton;

    [Tooltip("Name of the start-menu scene loaded by the Main Menu button. Must be in Build Settings.")]
    public string mainMenuSceneName = "MainMenu";

    [Tooltip("Scripts to disable while the screen is shown")]
    public MonoBehaviour[] blockScripts;

    void Awake()
    {
        Instance = this;

        if (winPanel != null)
            winPanel.SetActive(false);

        if (restartButton != null)
        {
            restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(Restart);
        }

        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.RemoveAllListeners();
            mainMenuButton.onClick.AddListener(GoToMainMenu);
        }
    }

    public void Show()
    {
        if (winPanel != null)
            winPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0f;

        if (blockScripts != null)
            for (int i = 0; i < blockScripts.Length; i++)
                if (blockScripts[i] != null) blockScripts[i].enabled = false;
    }

    private void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
