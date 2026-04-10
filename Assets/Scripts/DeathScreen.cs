using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathScreen : MonoBehaviour
{
    public static DeathScreen Instance { get; private set; }

    public GameObject deathPanel;

    void Awake()
    {
        Instance = this;

        if (deathPanel != null)
            deathPanel.SetActive(false);
    }

    public void Show()
    {
        if (deathPanel != null)
            deathPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0f;
    }

    public void OnRestartButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
