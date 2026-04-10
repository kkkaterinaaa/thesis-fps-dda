using UnityEngine;

public class EnemyChaser : EnemyBase
{

    public float desiredDistance = 5f;


    public override void Start()
    {
        base.Start();

        if (stats != null && stats.desiredDistance > 0.5f)
            desiredDistance = stats.desiredDistance;

        if (agent != null)
            agent.stoppingDistance = desiredDistance;
    }

    protected override void Act(bool allowMovement)
    {
        if (agent != null)
            agent.stoppingDistance = desiredDistance;

        float dist = Vector3.Distance(transform.position, player.position);

        if (dist > desiredDistance + 0.3f)
        {
            agent.isStopped = false;
            if (allowMovement)
                agent.SetDestination(player.position);
        }
        else
        {
            agent.isStopped = true;
            FaceTarget();
            TryShoot(stats.fireRate, stats.attackRange);
        }
    }
}