using UnityEngine;

public class FirstAidKitPickup : MonoBehaviour
{
    public float healAmount = 20f;

    [Header("Audio")]
    public AudioClip pickupClip;
    [Range(0f, 1f)] public float pickupVolume = 0.9f;

    void OnTriggerEnter(Collider other)
    {
        var playerHealth = other.GetComponentInParent<PlayerHealth>();
        if (playerHealth == null) return;

        if (playerHealth.CurrentHealth >= playerHealth.maxHealth)
            return;

        float mult = Mathf.Clamp(DifficultyState.HealDropMult, 0.1f, 10f);
        float effectiveAmount = healAmount * mult;

        float before = playerHealth.CurrentHealth;
        playerHealth.Heal(effectiveAmount);
        float healed = Mathf.Max(0f, playerHealth.CurrentHealth - before);
        TelemetryManager.RecordMedkitPickup(healed);
        TutorialManager.CompleteObjective(TutorialManager.ObjectiveType.CollectMedkit);
        if (pickupClip != null)
            AudioSource.PlayClipAtPoint(pickupClip, transform.position, pickupVolume);
        Destroy(gameObject);
    }
}
