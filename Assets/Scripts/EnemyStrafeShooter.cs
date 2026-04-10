using UnityEngine;
using UnityEngine.AI;

public class EnemyStrafeShooter : EnemyBase
{
    public float idealDistance = 6f;
    private float orbitOffset = 3f;

    public override void Start()
    {
        base.Start();
        idealDistance = stats.idealDistance;
    }

    protected override void Act(bool allowMovement)
    {
        float dist = Vector3.Distance(transform.position, player.position);

        if (Mathf.Abs(dist - idealDistance) > 1f)
        {
            if (allowMovement)
            {
                Vector3 dir = (transform.position - player.position).normalized;
                Vector3 targetPos = player.position + dir * idealDistance;
                agent.isStopped = false;
                agent.SetDestination(targetPos);
            }
            return;
        }

        if (allowMovement)
        {
            Vector3 right = Quaternion.Euler(0, 90, 0) * (player.position - transform.position).normalized;
            Vector3 orbit = transform.position + right * orbitOffset;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(orbit, out hit, 1f, NavMesh.AllAreas))
            {
                agent.isStopped = false;
                agent.SetDestination(hit.position);
            }
        }

        FaceTarget();
        TryShoot(stats.fireRate, stats.attackRange);
    }
}