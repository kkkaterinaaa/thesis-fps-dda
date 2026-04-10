using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyProjectile : MonoBehaviour
{
    [SerializeField] private GameObject hitEffectPrefab;

    private float bodyDamage;
    private float headDamage;
    private float speed;
    private float lifetime;
    private GameObject owner;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Init(float damage, float speed, float lifetime, GameObject owner)
    {
        Init(damage, damage, speed, lifetime, owner);
    }

    public void Init(float bodyDamage, float headDamage, float speed, float lifetime, GameObject owner)
    {
        this.bodyDamage = bodyDamage;
        this.headDamage = headDamage;
        this.speed = speed;
        this.lifetime = lifetime;
        this.owner = owner;

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = false;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }

        if (rb != null)
            rb.velocity = transform.forward * speed;

        if (owner != null)
        {
            var myCollider = GetComponent<Collider>();
            var ownerColliders = owner.GetComponentsInChildren<Collider>();

            if (myCollider != null && ownerColliders != null)
            {
                for (int i = 0; i < ownerColliders.Length; i++)
                {
                    if (ownerColliders[i] != null)
                        Physics.IgnoreCollision(myCollider, ownerColliders[i], true);
                }
            }
        }

        if (lifetime > 0f)
            Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (owner != null && other.transform.root == owner.transform) return;

        var playerHealth = other.GetComponentInParent<PlayerHealth>();
        if (playerHealth != null)
        {
            bool isHead = other.CompareTag("Head");
            float damage = isHead ? headDamage : bodyDamage;
            Debug.Log(isHead ? "Enemy HEADSHOT" : "Enemy body hit");
            playerHealth.TakeDamage(damage);
        }

        if (hitEffectPrefab != null)
        {
            var fx = Instantiate(hitEffectPrefab, transform.position, transform.rotation);
            Destroy(fx, 0.3f);
        }

        Destroy(gameObject);
    }
}
