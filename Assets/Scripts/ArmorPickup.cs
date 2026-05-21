using UnityEngine;

public class ArmorPickup : MonoBehaviour
{
    public float armorAmount = 50f;
    public bool fillToMax = false;

    [Header("Audio")]
    public AudioClip pickupClip;
    [Range(0f, 1f)] public float pickupVolume = 0.9f;

    void OnTriggerEnter(Collider other)
    {
        var playerHealth = other.GetComponentInParent<PlayerHealth>();
        if (playerHealth == null) return;

        float mult = Mathf.Clamp(DifficultyState.HealDropMult, 0.1f, 10f);
        float amountToAdd = fillToMax ? playerHealth.maxArmor : armorAmount * mult;
        playerHealth.AddArmor(amountToAdd);
        TelemetryManager.RecordArmorPickup(amountToAdd);
        TutorialManager.CompleteObjective(TutorialManager.ObjectiveType.CollectArmor);
        if (pickupClip != null)
            AudioSource.PlayClipAtPoint(pickupClip, transform.position, pickupVolume);
        Destroy(gameObject);
    }
}
