using UnityEngine;

public class FirstAidKitPickup : MonoBehaviour
{
    public float healAmount = 20f;

    void OnTriggerEnter(Collider other)
    {
        var playerHealth = other.GetComponentInParent<PlayerHealth>();
        if (playerHealth == null) return;

        if (playerHealth.CurrentHealth >= playerHealth.maxHealth)
            return;

        float before = playerHealth.CurrentHealth;
        playerHealth.Heal(healAmount);
        float healed = Mathf.Max(0f, playerHealth.CurrentHealth - before);
        TelemetryManager.RecordMedkitPickup(healed);
        TutorialManager.CompleteObjective(TutorialManager.ObjectiveType.CollectMedkit);
        Destroy(gameObject);
    }
}
