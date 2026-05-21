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
    public TMP_Text reserveText;
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

        if (isPistol && gun != null)
        {
            bool reloading = gun.IsReloading();
            if (ammoText != null)
            {
                string suffix = reloading ? " reading..." : "";
                ammoText.text = $"{gun.GetAmmoInMagazine()} / {gun.GetMagazineSize()}{suffix}";
            }
            if (reserveText != null)
                reserveText.text = $"+{gun.GetReserveAmmo()}";
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
