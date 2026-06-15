using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyPatrolPerception : MonoBehaviour
{
    public Transform[] patrolPoints;
    public float patrolPointReachDistance = 0.6f;

    public float roamRadius = 6f;
    public float roamRepathDistance = 1.2f;

    public float detectionRange = 10f;
    [Range(0f, 360f)]
    public float fieldOfView = 120f;
    public LayerMask lineOfSightMask = ~0;

    [Header("Alert (taken damage)")]
    [Tooltip("How long the enemy stays alerted (engaging) after taking damage, even without line of sight")]
    public float alertDuration = 8f;

    public bool CanEngage => canEngage;

    private NavMeshAgent agent;
    private Transform player;

    private int patrolIndex;
    private bool canEngage;
    private Vector3 roamCenter;
    private Vector3 currentRoamTarget;
    private float combatStoppingDistance;
    private float alertTimer;
    private Health health;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        var playerGo = GameObject.FindWithTag("Player");
        if (playerGo != null)
            player = playerGo.transform;

        roamCenter = transform.position;
        currentRoamTarget = transform.position;
        combatStoppingDistance = agent.stoppingDistance;

        health = GetComponent<Health>();
        if (health != null)
            health.OnDamaged += OnDamaged;

        if (patrolPoints != null && patrolPoints.Length > 0)
            SetDestination(patrolPoints[0].position);
    }

    void OnDestroy()
    {
        if (health != null)
            health.OnDamaged -= OnDamaged;
    }

    private void OnDamaged(float amount)
    {
        alertTimer = alertDuration;
    }

    public void Alert(float duration)
    {
        alertTimer = Mathf.Max(alertTimer, duration);
    }

    void Update()
    {
        if (alertTimer > 0f) alertTimer -= Time.deltaTime;

        canEngage = CheckEngage() || alertTimer > 0f;

        if (canEngage)
        {
            agent.stoppingDistance = combatStoppingDistance;
            agent.isStopped = false;
            return;
        }

        Patrol();
    }

    private bool CheckEngage()
    {
        if (player == null) return false;

        Vector3 toPlayer = player.position - transform.position;
        float dist = toPlayer.magnitude;
        if (dist > detectionRange) return false;

        Vector3 dirFlat = toPlayer;
        dirFlat.y = 0f;
        if (dirFlat.sqrMagnitude > 0.001f)
        {
            float angle = Vector3.Angle(transform.forward, dirFlat);
            if (angle > fieldOfView * 0.5f)
                return false;
        }

        Vector3 origin = transform.position + Vector3.up * 1.6f;
        Vector3 target = player.position + Vector3.up * 1.2f;
        Vector3 dir = (target - origin);
        float rayDist = dir.magnitude;
        if (rayDist <= 0.001f) return true;

        RaycastHit[] hits = Physics.RaycastAll(origin, dir.normalized, rayDist, lineOfSightMask, QueryTriggerInteraction.Collide);
        if (hits != null && hits.Length > 0)
        {
            System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

            for (int i = 0; i < hits.Length; i++)
            {
                Transform t = hits[i].transform;
                if (t == null) continue;

                if (t == transform || t.IsChildOf(transform))
                    continue;

                return (t == player || t.IsChildOf(player));
            }

            return false;
        }

        return true;
    }

    private void Patrol()
    {
        agent.isStopped = false;
        agent.stoppingDistance = 0.5f;

        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            Vector3 dst = patrolPoints[patrolIndex].position;
            if (!agent.pathPending && agent.remainingDistance <= patrolPointReachDistance)
            {
                patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
                dst = patrolPoints[patrolIndex].position;
                SetDestination(dst);
            }

            return;
        }

        if (!agent.pathPending && agent.remainingDistance <= roamRepathDistance)
        {
            if (TryGetRandomNavmeshPoint(roamCenter, roamRadius, out Vector3 p))
            {
                currentRoamTarget = p;
                SetDestination(currentRoamTarget);
            }
        }
    }

    private void SetDestination(Vector3 pos)
    {
        if (agent == null) return;
        agent.SetDestination(pos);
    }

    private bool TryGetRandomNavmeshPoint(Vector3 center, float radius, out Vector3 point)
    {
        for (int i = 0; i < 10; i++)
        {
            Vector3 random = center + new Vector3(Random.Range(-radius, radius), 0f, Random.Range(-radius, radius));
            if (NavMesh.SamplePosition(random, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            {
                point = hit.position;
                return true;
            }
        }

        point = center;
        return false;
    }
}
