using UnityEngine;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }

    public enum ObjectiveType
    {
        CollectArmor,
        KillWithKnife,
        CollectMedkit,
        CollectPistol,
        KillWithPistol,
        CollectMagazine
    }

    [System.Serializable]
    public class Objective
    {
        public ObjectiveType type;
        public string message;
        [HideInInspector] public bool completed;
    }

    [Header("Objectives (in order)")]
    public Objective[] objectives = new Objective[]
    {
        new Objective { type = ObjectiveType.CollectArmor,   message = "Find and collect armor" },
        new Objective { type = ObjectiveType.KillWithKnife,  message = "Kill enemy using knife" },
        new Objective { type = ObjectiveType.CollectMedkit,  message = "Find and collect first aid kit" },
        new Objective { type = ObjectiveType.CollectPistol,  message = "Find and collect pistol" },
        new Objective { type = ObjectiveType.KillWithPistol, message = "Kill enemy using pistol" },
        new Objective { type = ObjectiveType.CollectMagazine,message = "Find and collect ammo magazine" },
    };

    [Header("UI")]
    public TMP_Text objectiveText;
    public GameObject completedPanel;

    private int currentIndex;

    void Awake()
    {
        Instance = this;

        if (completedPanel != null)
            completedPanel.SetActive(false);
    }

    void Start()
    {
        currentIndex = 0;
        ShowCurrentObjective();
    }

    private void ShowCurrentObjective()
    {
        if (objectiveText == null) return;

        if (currentIndex < objectives.Length)
        {
            objectiveText.gameObject.SetActive(true);
            objectiveText.text = objectives[currentIndex].message;
        }
        else
        {
            objectiveText.gameObject.SetActive(false);
        }
    }

    public static void CompleteObjective(ObjectiveType type)
    {
        if (Instance == null) return;
        Instance.TryComplete(type);
    }

    private void TryComplete(ObjectiveType type)
    {
        if (currentIndex >= objectives.Length) return;
        if (objectives[currentIndex].type != type) return;

        objectives[currentIndex].completed = true;
        currentIndex++;

        if (currentIndex >= objectives.Length)
        {
            ShowTutorialCompleted();
        }
        else
        {
            ShowCurrentObjective();
        }
    }

    private void ShowTutorialCompleted()
    {
        if (objectiveText != null)
            objectiveText.gameObject.SetActive(false);

        if (completedPanel != null)
            completedPanel.SetActive(true);

        if (TelemetryManager.Instance != null)
            TelemetryManager.Instance.EndSession("win");

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;
    }
}
