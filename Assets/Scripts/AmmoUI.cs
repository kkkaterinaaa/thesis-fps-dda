using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AmmoUI : MonoBehaviour
{
    [Header("References")]
    public RaycastGun gun;
    public WeaponSwitcher weaponSwitcher;

    [Header("Ammo Display")]
    public TMP_Text ammoText;
    public GameObject ammoRoot;

    [Header("Weapon Icon")]
    public Image weaponIcon;
    public Sprite pistolSprite;
    public Sprite knifeSprite;

    void Start()
    {
        if (gun == null)
        {
            var playerGo = GameObject.FindGameObjectWithTag("Player");
            if (playerGo != null)
                gun = playerGo.GetComponentInChildren<RaycastGun>(true);

            if (gun == null)
                gun = FindFirstObjectByType<RaycastGun>();
        }

        if (weaponSwitcher == null)
        {
            var playerGo = GameObject.FindGameObjectWithTag("Player");
            if (playerGo != null)
                weaponSwitcher = playerGo.GetComponentInChildren<WeaponSwitcher>(true);

            if (weaponSwitcher == null)
                weaponSwitcher = FindFirstObjectByType<WeaponSwitcher>();
        }

        UpdateUI();
    }

    void Update()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        bool isPistol = weaponSwitcher != null && weaponSwitcher.ActiveSlot == 1;

        if (ammoRoot != null && ammoRoot.activeSelf != isPistol)
            ammoRoot.SetActive(isPistol);

        if (isPistol && gun != null && ammoText != null)
        {
            int total = gun.GetAmmoInMagazine() + gun.GetReserveAmmo();
            string suffix = gun.IsReloading() ? "  reloading..." : "";
            ammoText.text = $"{total}{suffix}";
        }

        if (weaponIcon != null)
        {
            if (isPistol && pistolSprite != null)
                weaponIcon.sprite = pistolSprite;
            else if (!isPistol && knifeSprite != null)
                weaponIcon.sprite = knifeSprite;
        }
    }
}
