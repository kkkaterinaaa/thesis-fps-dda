using UnityEngine;

public class WeaponSwitcher : MonoBehaviour
{
    [Header("Slots")]
    public GameObject slot1;
    public GameObject slot2;

    [Header("Settings")]
    public int startSlot = 2;

    private int activeSlot;
    public int ActiveSlot => activeSlot;

    private bool hasPistol = false;
    public bool HasPistol => hasPistol;

    private bool hasScrolls = false;
    public bool HasScrolls => hasScrolls;
    public void MarkScrollsCollected() { hasScrolls = true; }

    void Awake()
    {
        if (!hasPistol && startSlot == 1)
            startSlot = 2;

        activeSlot = startSlot;
        if (slot1 != null) slot1.SetActive(activeSlot == 1);
        if (slot2 != null) slot2.SetActive(activeSlot == 2);
    }

    void Update()
    {
        if (TutorialManager.InputBlocked) return;
        if (Time.timeScale == 0f) return;

        if (Input.GetKeyDown(KeyCode.Alpha1) && hasPistol) Equip(1);
        if (Input.GetKeyDown(KeyCode.Alpha2)) Equip(2);

        float wheel = Input.mouseScrollDelta.y;
        if (wheel > 0f)
        {
            int next = WrapSlot(activeSlot + 1);
            if (next == 1 && !hasPistol) next = 2;
            Equip(next);
        }
        if (wheel < 0f)
        {
            int next = WrapSlot(activeSlot - 1);
            if (next == 1 && !hasPistol) next = 2;
            Equip(next);
        }
    }

    private int WrapSlot(int slot)
    {
        if (slot < 1) return 2;
        if (slot > 2) return 1;
        return slot;
    }

    public void Equip(int slot)
    {
        if (slot == 1 && !hasPistol) return;

        activeSlot = Mathf.Clamp(slot, 1, 2);

        if (slot1 != null) slot1.SetActive(activeSlot == 1);
        if (slot2 != null) slot2.SetActive(activeSlot == 2);
    }

    public void UnlockPistol()
    {
        hasPistol = true;
        Equip(1);
    }
}
