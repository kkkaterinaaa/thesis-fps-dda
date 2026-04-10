using UnityEngine;

public class KnifeWeapon : MonoBehaviour
{
    public Camera fpsCam;
    public float range = 2f;
    public float radius = 0.25f;
    public float damage = 35f;
    public float cooldown = 0.4f;
    public LayerMask hitMask;

    private float nextAttackTime;

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + cooldown;
            Attack();
        }
    }

    private void Attack()
    {
        if (fpsCam == null) return;

        RaycastHit hit;
        if (Physics.SphereCast(fpsCam.transform.position, radius, fpsCam.transform.forward, out hit, range, hitMask))
        {
            var health = hit.collider.GetComponentInParent<Health>();
            if (health != null)
            {
                health.TakeDamage(damage);
                TelemetryManager.RecordShotHit(false, damage);

                if (health.currentHealth <= 0)
                    TutorialManager.CompleteObjective(TutorialManager.ObjectiveType.KillWithKnife);
            }
        }
    }
}
