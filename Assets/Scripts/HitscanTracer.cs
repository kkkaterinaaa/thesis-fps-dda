using UnityEngine;

public class HitscanTracer : MonoBehaviour
{
    public float speed = 80f;
    public float lifetime = 1f;

    private Vector3 target;
    private bool hasTarget;

    void Start()
    {
        if (lifetime > 0f)
            Destroy(gameObject, lifetime);
    }

    public void Init(Vector3 target, float speed, float lifetime)
    {
        this.target = target;
        this.speed = speed;
        this.lifetime = lifetime;
        hasTarget = true;

        if (lifetime > 0f)
            Destroy(gameObject, lifetime);
    }

    void Update()
    {
        if (!hasTarget) return;

        Vector3 dir = (target - transform.position);
        float distThisFrame = speed * Time.deltaTime;

        if (dir.sqrMagnitude <= distThisFrame * distThisFrame)
        {
            transform.position = target;
            Destroy(gameObject);
            return;
        }

        transform.position += dir.normalized * distThisFrame;
    }
}
