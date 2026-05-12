using UnityEngine;

public class PistolPickup : MonoBehaviour
{
    [Header("Audio")]
    public AudioClip pickupClip;
    [Range(0f, 1f)] public float pickupVolume = 0.9f;

    void OnTriggerEnter(Collider other)
    {
        var switcher = other.GetComponentInParent<WeaponSwitcher>();
        if (switcher == null)
        {
            var playerGo = GameObject.FindGameObjectWithTag("Player");
            if (playerGo != null && other.transform.IsChildOf(playerGo.transform))
                switcher = playerGo.GetComponentInChildren<WeaponSwitcher>(true);
        }

        if (switcher == null) return;
        if (switcher.HasPistol) return;

        switcher.UnlockPistol();
        TutorialManager.CompleteObjective(TutorialManager.ObjectiveType.CollectPistol);
        if (pickupClip != null)
            AudioSource.PlayClipAtPoint(pickupClip, transform.position, pickupVolume);

        if (!switcher.HasScrolls && HintUI.Instance != null)
            HintUI.Instance.Show("Find scrolls to cast more spells");

        Destroy(gameObject);
    }
}
