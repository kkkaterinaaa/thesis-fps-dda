using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Health))]
public class EnemyController : MonoBehaviour
{
    public EnemyStats stats;  // Подключаем EnemyStats

    private EnemyBase enemyBase;

    void Start()
    {
        enemyBase = GetComponent<EnemyBase>();
        ApplyStats();
    }

    public void ApplyStats()
    {
        if (stats == null) return;

        var agent = GetComponent<NavMeshAgent>();
        agent.speed = stats.moveSpeed;

        var hp = GetComponent<Health>();
        float hpMult = DifficultyState.EnemyHPMult;
        hpMult = Mathf.Clamp(hpMult, 0.1f, 10f);
        hp.SetMax(stats.maxHP * hpMult, fillToMax: true);

        var baseAI = GetComponent<EnemyBase>();
        if (baseAI != null)
        {
            baseAI.stats = stats;
            baseAI.attackRange = stats.attackRange;
            baseAI.fireRate = stats.fireRate;
        }

        var chaser = GetComponent<EnemyChaser>();
        if (chaser != null)
            chaser.desiredDistance = stats.desiredDistance;

        var strafe = GetComponent<EnemyStrafeShooter>();
        if (strafe != null)
            strafe.idealDistance = stats.idealDistance;
    }
}