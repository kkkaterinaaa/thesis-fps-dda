using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }

    public static bool InputBlocked { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStaticState()
    {
        InputBlocked = false;
        Instance = null;
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
            InputBlocked = false;
        }
    }

    public enum ObjectiveType
    {
        PressTab,
        CollectArmor,
        KillWithKnife,
        CollectMedkit,
        CollectPistol,
        KillWithPistol,
        CollectMagazine,
        ManualReload,
        PressEToFinish
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
        new Objective { type = ObjectiveType.PressTab,        message = "Press Tab to see the controls" },
        new Objective { type = ObjectiveType.CollectArmor,    message = "Find and collect the protective necklace" },
        new Objective { type = ObjectiveType.KillWithKnife,   message = "Banish a ghost using the metal bar" },
        new Objective { type = ObjectiveType.CollectMedkit,   message = "Drink a healing potion" },
        new Objective { type = ObjectiveType.CollectPistol,   message = "Find and collect the magic wand" },
        new Objective { type = ObjectiveType.KillWithPistol,  message = "Banish a ghost by casting a spell with your wand" },
        new Objective { type = ObjectiveType.CollectMagazine, message = "Find and collect a spell scroll" },
        new Objective { type = ObjectiveType.ManualReload,    message = "Press R to read new spells onto your wand" },
        new Objective { type = ObjectiveType.PressEToFinish,  message = "Press E to finish the tutorial" },
    };

    [Header("UI")]
    public TMP_Text objectiveText;
    public GameObject startPanel;
    public Button startButton;
    public GameObject controlsPanel;
    public GameObject completedPanel;
    public Button goToLevelButton;
    public Button restartButton;

    [Header("Scenes")]
    [Tooltip("Scene to load when the player presses Go To Level after completing the tutorial")]
    public string levelSceneName = "MainLevel";

    [Header("Keys")]
    public KeyCode controlsKey = KeyCode.Tab;
    public KeyCode finishKey   = KeyCode.E;

    private int currentIndex;
    private bool started;

    void Awake()
    {
        Instance = this;

        if (controlsPanel  != null) controlsPanel.SetActive(false);
        if (completedPanel != null) completedPanel.SetActive(false);

        if (startPanel != null)
        {
            startPanel.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0f;
            InputBlocked = true;
            started = false;
        }
        else
        {
            InputBlocked = false;
            started = true;
        }

        if (startButton != null)
        {
            startButton.onClick.RemoveAllListeners();
            startButton.onClick.AddListener(BeginTutorial);
        }

        if (goToLevelButton != null)
        {
            goToLevelButton.onClick.RemoveAllListeners();
            goToLevelButton.onClick.AddListener(GoToLevel);
        }

        if (restartButton != null)
        {
            restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(Restart);
        }
    }

    void Start()
    {
        currentIndex = 0;
        if (started) ShowCurrentObjective();
        else if (objectiveText != null) objectiveText.gameObject.SetActive(false);
    }

    public void BeginTutorial()
    {
        Debug.Log("[Tutorial] BeginTutorial clicked");
        if (startPanel != null) startPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;
        InputBlocked = false;
        started = true;
        ShowCurrentObjective();
    }

    void Update()
    {
        if (!started) return;
        if (InputBlocked) return;

        if (Input.GetKeyDown(controlsKey))
        {
            if (controlsPanel != null)
                controlsPanel.SetActive(!controlsPanel.activeSelf);
            CompleteObjective(ObjectiveType.PressTab);
        }

        if (currentIndex < objectives.Length
            && objectives[currentIndex].type == ObjectiveType.PressEToFinish
            && Input.GetKeyDown(finishKey))
        {
            CompleteObjective(ObjectiveType.PressEToFinish);
        }
    }

    [ContextMenu("Reset Objectives To Defaults")]
    private void ResetObjectivesToDefaults()
    {
        objectives = new Objective[]
        {
            new Objective { type = ObjectiveType.PressTab,        message = "Press Tab to see the controls" },
            new Objective { type = ObjectiveType.CollectArmor,    message = "Find and collect the protective necklace" },
            new Objective { type = ObjectiveType.KillWithKnife,   message = "Banish a ghost using the metal bar" },
            new Objective { type = ObjectiveType.CollectMedkit,   message = "Drink a healing potion" },
            new Objective { type = ObjectiveType.CollectPistol,   message = "Find and collect the magic wand" },
            new Objective { type = ObjectiveType.KillWithPistol,  message = "Banish a ghost by casting a spell with your wand" },
            new Objective { type = ObjectiveType.CollectMagazine, message = "Find and collect a spell scroll" },
            new Objective { type = ObjectiveType.ManualReload,    message = "Press R to read new spells onto your wand" },
            new Objective { type = ObjectiveType.PressEToFinish,  message = "Press E to finish the tutorial" },
        };
    }

    public void GoToLevel()
    {
        Time.timeScale = 1f;
        if (!string.IsNullOrWhiteSpace(levelSceneName))
            SceneManager.LoadScene(levelSceneName);
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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

        if (controlsPanel != null)
            controlsPanel.SetActive(false);

        if (completedPanel != null)
            completedPanel.SetActive(true);

        if (TelemetryManager.Instance != null)
            TelemetryManager.Instance.EndSession("win");

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;
        InputBlocked = true;
    }
}
