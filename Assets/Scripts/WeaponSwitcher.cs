using UnityEngine;

public class WeaponSwitcher : MonoBehaviour
{
    [Header("Slots")]
    public GameObject slot1;
    public GameObject slot2;

    [Header("Settings")]
    public int startSlot = 1;

    private int activeSlot;
    public int ActiveSlot => activeSlot;

    void Start()
    {
        Equip(startSlot);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) Equip(1);
        if (Input.GetKeyDown(KeyCode.Alpha2)) Equip(2);

        float wheel = Input.mouseScrollDelta.y;
        if (wheel > 0f) Equip(WrapSlot(activeSlot + 1));
        if (wheel < 0f) Equip(WrapSlot(activeSlot - 1));
    }

    private int WrapSlot(int slot)
    {
        if (slot < 1) return 2;
        if (slot > 2) return 1;
        return slot;
    }

    public void Equip(int slot)
    {
        activeSlot = Mathf.Clamp(slot, 1, 2);

        if (slot1 != null) slot1.SetActive(activeSlot == 1);
        if (slot2 != null) slot2.SetActive(activeSlot == 2);
    }
}
