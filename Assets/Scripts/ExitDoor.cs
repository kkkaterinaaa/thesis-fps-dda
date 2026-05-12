using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ExitDoor : MonoBehaviour
{
    [Header("Interaction")]
    public KeyCode interactKey = KeyCode.E;
    [Tooltip("UI GameObject shown when player is inside the trigger (e.g. 'Press E to escape')")]
    public GameObject promptUI;

    private bool playerInside;

    void Awake()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;
        if (promptUI != null) promptUI.SetActive(false);
    }

    void Update()
    {
        if (!playerInside) return;

        if (Input.GetKeyDown(interactKey))
            TriggerWin();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!IsPlayer(other)) return;
        playerInside = true;
        if (promptUI != null) promptUI.SetActive(true);
    }

    void OnTriggerExit(Collider other)
    {
        if (!IsPlayer(other)) return;
        playerInside = false;
        if (promptUI != null) promptUI.SetActive(false);
    }

    private bool IsPlayer(Collider other)
    {
        if (other == null) return false;
        if (other.CompareTag("Player")) return true;
        return other.GetComponentInParent<PlayerHealth>() != null;
    }

    private void TriggerWin()
    {
        if (promptUI != null) promptUI.SetActive(false);

        if (TelemetryManager.Instance != null)
            TelemetryManager.Instance.EndSession("win");

        if (WinScreen.Instance != null)
            WinScreen.Instance.Show();
    }
}
