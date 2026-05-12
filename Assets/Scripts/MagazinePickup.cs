using UnityEngine;

public class MagazinePickup : MonoBehaviour
{
    public int cartridges = 10;

    [Header("Audio")]
    public AudioClip pickupClip;
    [Range(0f, 1f)] public float pickupVolume = 0.9f;

    void OnTriggerEnter(Collider other)
    {
        var gun = other.GetComponentInParent<RaycastGun>();
        if (gun == null)
        {
            var playerGo = GameObject.FindGameObjectWithTag("Player");
            if (playerGo != null && other.transform.IsChildOf(playerGo.transform))
                gun = playerGo.GetComponentInChildren<RaycastGun>(true);
        }

        if (gun == null) return;

        gun.AddReserveAmmo(cartridges);
        TelemetryManager.RecordMagazinePickup(cartridges);
        TutorialManager.CompleteObjective(TutorialManager.ObjectiveType.CollectMagazine);
        if (pickupClip != null)
            AudioSource.PlayClipAtPoint(pickupClip, transform.position, pickupVolume);

        var switcher = gun.GetComponentInParent<WeaponSwitcher>();
        if (switcher == null)
        {
            var playerGo = GameObject.FindGameObjectWithTag("Player");
            if (playerGo != null) switcher = playerGo.GetComponentInChildren<WeaponSwitcher>(true);
        }
        if (switcher != null)
        {
            switcher.MarkScrollsCollected();
            if (!switcher.HasPistol && HintUI.Instance != null)
                HintUI.Instance.Show("Find the magic wand to cast spells");
        }

        Destroy(gameObject);
    }
}
