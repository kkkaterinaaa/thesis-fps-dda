using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class ExperimentFlow : MonoBehaviour
{
    public static ExperimentFlow Instance { get; private set; }

    private const string PrefsKey = "experiment.runIndex";

    [Header("Survey URLs (one per run)")]
    [Tooltip("Index 0 = after run 1, index 1 = after run 2, etc.")]
    public string[] surveyUrls = new string[6];

    [Header("Baseline (control) run")]
    [Tooltip("If true, the first run (index 0) uses default multipliers (all = 1) and the DDA model is not updated")]
    public bool firstRunIsBaseline = true;

    [Header("Texts")]
    public string thanksMessage = "Thank you for participating in the experiment!";

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Run index of the run that just finished (0-based).
    // 0 = first run completed.
    public int CompletedRunIndex => Mathf.Max(0, PlayerPrefs.GetInt(PrefsKey, 0));

    public bool IsFinalRun => CompletedRunIndex >= surveyUrls.Length - 1;

    public bool IsBaselineRun => firstRunIsBaseline && CompletedRunIndex == 0;

    public void SetupEndScreen(Button surveyButton, Button restartButton, GameObject thanksRoot, TMP_Text thanksText)
    {
        int idx = CompletedRunIndex;
        bool surveyClicked = false;

        // Initial visibility
        if (surveyButton != null) surveyButton.gameObject.SetActive(true);
        if (restartButton != null) restartButton.gameObject.SetActive(false);
        if (thanksRoot != null) thanksRoot.SetActive(false);

        // Configure survey button click
        if (surveyButton != null)
        {
            surveyButton.onClick.RemoveAllListeners();
            surveyButton.onClick.AddListener(() =>
            {
                string url = (idx >= 0 && idx < surveyUrls.Length) ? surveyUrls[idx] : null;
                if (!string.IsNullOrWhiteSpace(url))
                    Application.OpenURL(url);

                surveyClicked = true;
                surveyButton.gameObject.SetActive(false);

                if (IsFinalRun)
                {
                    if (thanksRoot != null) thanksRoot.SetActive(true);
                    if (thanksText != null) thanksText.text = thanksMessage;
                    // no restart on final run
                }
                else
                {
                    if (restartButton != null) restartButton.gameObject.SetActive(true);
                }
            });
        }

        // Configure restart button click — advance run index and reload scene
        if (restartButton != null)
        {
            restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(() =>
            {
                if (!surveyClicked) return; // safety: shouldn't be visible yet
                AdvanceRun();
                Time.timeScale = 1f;
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            });
        }
    }

    private void AdvanceRun()
    {
        int next = CompletedRunIndex + 1;
        PlayerPrefs.SetInt(PrefsKey, next);
        PlayerPrefs.Save();
    }

    public void ResetExperiment()
    {
        PlayerPrefs.DeleteKey(PrefsKey);
        PlayerPrefs.Save();
    }
}
