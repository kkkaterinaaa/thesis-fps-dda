using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HintUI : MonoBehaviour
{
    public static HintUI Instance { get; private set; }

    [Header("UI")]
    [Tooltip("GameObject (panel/text) to show while a hint is active")]
    public GameObject hintRoot;
    [Tooltip("Legacy UI Text (optional)")]
    public Text textLegacy;
    [Tooltip("TextMeshPro text (optional)")]
    public TMP_Text textTMP;

    [Header("Timing")]
    public float defaultDuration = 3.5f;

    private Coroutine routine;

    void Awake()
    {
        Instance = this;
        if (hintRoot != null) hintRoot.SetActive(false);
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void Show(string message, float duration = -1f)
    {
        if (hintRoot == null) return;
        if (textLegacy != null) textLegacy.text = message;
        if (textTMP != null) textTMP.text = message;

        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(ShowRoutine(duration > 0f ? duration : defaultDuration));
    }

    private IEnumerator ShowRoutine(float duration)
    {
        hintRoot.SetActive(true);
        yield return new WaitForSeconds(duration);
        hintRoot.SetActive(false);
        routine = null;
    }
}
