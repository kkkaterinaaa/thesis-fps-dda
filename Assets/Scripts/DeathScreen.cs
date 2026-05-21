using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeathScreen : MonoBehaviour
{
    public static DeathScreen Instance { get; private set; }

    public GameObject deathPanel;

    [Header("Experiment Buttons")]
    public Button surveyButton;
    public Button restartButton;
    public GameObject thanksRoot;
    public TMP_Text thanksText;

    [Tooltip("Scripts to disable while the screen is shown")]
    public MonoBehaviour[] blockScripts;

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

        if (blockScripts != null)
            for (int i = 0; i < blockScripts.Length; i++)
                if (blockScripts[i] != null) blockScripts[i].enabled = false;

        if (DamageFlash.Instance != null)
            DamageFlash.Instance.Clear();

        if (ExperimentFlow.Instance != null)
            ExperimentFlow.Instance.SetupEndScreen(surveyButton, restartButton, thanksRoot, thanksText);
    }
}
