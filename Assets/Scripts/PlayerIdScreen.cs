using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerIdScreen : MonoBehaviour
{
    [Header("UI")]
    public GameObject panel;
    public TMP_InputField inputField;
    public Button confirmButton;

    [Header("Behavior")]
    [Tooltip("If true, the screen is skipped when a player ID is already saved")]
    public bool skipIfAlreadySet = true;
    [Tooltip("Pause the game while the screen is open")]
    public bool pauseWhileOpen = true;

    [Header("Block These Scripts While Open")]
    [Tooltip("These MonoBehaviours will be disabled while the screen is open and re-enabled afterwards")]
    public MonoBehaviour[] blockScripts;

    void Start()
    {
        bool needScreen = true;
        if (skipIfAlreadySet && PlayerIdProvider.Instance != null && PlayerIdProvider.Instance.HasPlayerId())
            needScreen = false;

        if (!needScreen)
        {
            HideAndStart();
            return;
        }

        if (panel != null) panel.SetActive(true);

        if (pauseWhileOpen)
        {
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        SetBlockedScripts(false);

        if (confirmButton != null)
            confirmButton.onClick.AddListener(OnConfirm);

        if (inputField != null)
        {
            inputField.Select();
            inputField.ActivateInputField();
            inputField.onSubmit.AddListener(_ => OnConfirm());
        }
    }

    public void OnConfirm()
    {
        string id = inputField != null ? inputField.text : "";
        if (string.IsNullOrWhiteSpace(id)) return;

        if (PlayerIdProvider.Instance != null)
            PlayerIdProvider.Instance.SetPlayerId(id);

        HideAndStart();
    }

    private void HideAndStart()
    {
        if (panel != null) panel.SetActive(false);

        if (pauseWhileOpen)
        {
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        SetBlockedScripts(true);

        if (TelemetryManager.Instance != null)
            TelemetryManager.Instance.BeginSessionManually();
    }

    private void SetBlockedScripts(bool enabled)
    {
        if (blockScripts == null) return;
        for (int i = 0; i < blockScripts.Length; i++)
            if (blockScripts[i] != null) blockScripts[i].enabled = enabled;
    }
}
