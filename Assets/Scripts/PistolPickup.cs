using UnityEngine;

public class PistolPickup : MonoBehaviour
{
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
        Destroy(gameObject);
    }
}
