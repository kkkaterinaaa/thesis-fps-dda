using UnityEngine;

public class ArmorPickup : MonoBehaviour
{
    public float armorAmount = 50f;
    public bool fillToMax = false;

    void OnTriggerEnter(Collider other)
    {
        var playerHealth = other.GetComponentInParent<PlayerHealth>();
        if (playerHealth == null) return;

        float amountToAdd = fillToMax ? playerHealth.maxArmor : armorAmount;
        playerHealth.AddArmor(amountToAdd);
        TelemetryManager.RecordArmorPickup(amountToAdd);
        TutorialManager.CompleteObjective(TutorialManager.ObjectiveType.CollectArmor);
        Destroy(gameObject);
    }
}
