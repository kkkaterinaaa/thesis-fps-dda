using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeathScreen : MonoBehaviour
{
    public static DeathScreen Instance { get; private set; }

    public GameObject deathPanel;

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

        if (deathPanel != null)
            deathPanel.SetActive(false);

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
        if (deathPanel != null)
            deathPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0f;

        if (blockScripts != null)
            for (int i = 0; i < blockScripts.Length; i++)
                if (blockScripts[i] != null) blockScripts[i].enabled = false;

        if (DamageFlash.Instance != null)
            DamageFlash.Instance.Clear();
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
