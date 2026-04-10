using UnityEngine;

public class MagazinePickup : MonoBehaviour
{
    public int cartridges = 10;

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
        Destroy(gameObject);
    }
}
