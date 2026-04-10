using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerProjectile : MonoBehaviour
{
    [SerializeField] private GameObject hitEffectPrefab;

    private float damage;
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
        this.damage = damage;
        this.speed = speed;
        this.lifetime = lifetime;
        this.owner = owner;

        if (rb == null)
        {
            Debug.LogWarning("PlayerProjectile has no Rigidbody", this);
            return;
        }

        rb.isKinematic = false;
        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

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
        HandleHit(other, other.ClosestPoint(transform.position));
    }

    void OnCollisionEnter(Collision collision)
    {
        HandleHit(collision.collider, collision.GetContact(0).point);
    }

    private void HandleHit(Collider other, Vector3 hitPoint)
    {
        if (other == null) return;
        if (owner != null && other.transform.root == owner.transform) return;

        var health = other.GetComponentInParent<Health>();
        if (health != null)
            health.TakeDamage(damage);

        if (hitEffectPrefab != null)
        {
            var fx = Instantiate(hitEffectPrefab, hitPoint, transform.rotation);
            Destroy(fx, 0.3f);
        }

        Destroy(gameObject);
    }
}
