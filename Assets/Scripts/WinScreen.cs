using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinScreen : MonoBehaviour
{
    public static WinScreen Instance { get; private set; }

    public GameObject winPanel;

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

        if (winPanel != null)
            winPanel.SetActive(false);
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

        if (ExperimentFlow.Instance != null)
            ExperimentFlow.Instance.SetupEndScreen(surveyButton, restartButton, thanksRoot, thanksText);
    }
}
