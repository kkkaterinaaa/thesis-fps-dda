using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyBase : MonoBehaviour
{
    protected NavMeshAgent agent;
    protected Transform player;

    public float attackRange = 8f;
    public float fireRate = 1f;
    protected float fireCooldown = 0f;

    [Header("Shooting")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform muzzle;
    [SerializeField] private float projectileSpeed = 25f;
    [SerializeField] private float projectileLifetime = 4f;
    [SerializeField] private GameObject muzzleFlashPrefab;
    [SerializeField] private float spreadAngle = 2f;
    [SerializeField] private LayerMask shootMask = ~0;
    [SerializeField] private float aimHeight = 1.1f;

    [Header("Tracer (visual only)")]
    [SerializeField] private GameObject tracerPrefab;
    [SerializeField] private float tracerSpeed = 80f;
    [SerializeField] private float tracerLifetime = 1f;

    // Movement throttle
    protected float nextMoveUpdate = 0f;
    protected const float MOVE_UPDATE_INTERVAL = 0.15f; // 6-7 times per second

    public EnemyStats stats;

    public virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        var playerGo = GameObject.FindWithTag("Player");
        if (playerGo != null)
            player = playerGo.transform;
    }

    public virtual void Update()
    {
        fireCooldown -= Time.deltaTime;

        var patrol = GetComponent<EnemyPatrolPerception>();
        if (patrol != null && !patrol.CanEngage)
            return;

        bool shouldMove = (Time.time >= nextMoveUpdate);

        Act(shouldMove);

        if (shouldMove)
            nextMoveUpdate = Time.time + MOVE_UPDATE_INTERVAL;
    }

    protected abstract void Act(bool allowMovement);

    public void TryShoot()
{
    if (fireCooldown > 0) return;

    if (player == null) return;

    float dist = Vector3.Distance(transform.position, player.position);
    if (dist > attackRange) return;

    fireCooldown = fireRate;

    Transform spawn = (muzzle != null) ? muzzle : transform;
    Vector3 aimPoint = player.position + Vector3.up * aimHeight;
    Vector3 toPlayer = (aimPoint - spawn.position);
    if (toPlayer.sqrMagnitude < 0.001f) return;

    Quaternion aimRot = Quaternion.LookRotation(toPlayer.normalized);
    if (spreadAngle > 0f)
        aimRot = aimRot * Quaternion.Euler(Random.Range(-spreadAngle, spreadAngle), Random.Range(-spreadAngle, spreadAngle), 0f);

    Vector3 shotDir = aimRot * Vector3.forward;

    if (muzzleFlashPrefab != null)
    {
        var flash = Instantiate(muzzleFlashPrefab, spawn.position, aimRot);
        Destroy(flash, 0.2f);
    }

    RaycastHit[] hits = Physics.RaycastAll(spawn.position, shotDir, attackRange, shootMask, QueryTriggerInteraction.Collide);
    if (hits != null && hits.Length > 0)
    {
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        SpawnTracer(spawn.position, hits[0].point);

        for (int i = 0; i < hits.Length; i++)
        {
            var playerHealth = hits[i].collider.GetComponentInParent<PlayerHealth>();
            if (playerHealth == null) continue;

            bool isHead = hits[i].collider.CompareTag("Head");
            float damage = isHead ? stats.headDamage : stats.bodyDamage;
            Debug.Log(isHead ? "Enemy HEADSHOT" : "Enemy body hit");
            playerHealth.TakeDamage(damage);
            break;
        }
    }
    else
    {
        SpawnTracer(spawn.position, spawn.position + shotDir.normalized * attackRange);
    }
}

    private void SpawnTracer(Vector3 start, Vector3 end)
    {
        if (tracerPrefab == null) return;

        GameObject tracerGo = Instantiate(tracerPrefab, start, Quaternion.LookRotation(end - start));
        var tracer = tracerGo.GetComponent<HitscanTracer>();
        if (tracer != null)
            tracer.Init(end, tracerSpeed, tracerLifetime);
        else
            Destroy(tracerGo, tracerLifetime);
    }

    public void TryShoot(float fireRate, float attackRange)
    {
        this.fireRate = fireRate;
        this.attackRange = attackRange;
        TryShoot();
    }

    public void FaceTarget()
    {
        Vector3 dir = player.position - transform.position;
        dir.y = 0;

        if (dir.sqrMagnitude < 0.1f) return;

        Quaternion targetRot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 6f);
    }
}