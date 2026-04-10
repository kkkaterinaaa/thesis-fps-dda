using UnityEngine;

public class EnemyDeath : MonoBehaviour
{
    private Health health;

    void Start()
    {
        health = GetComponent<Health>();
        health.OnDeath += Die;
    }

    void Die()
    {
        Destroy(gameObject);
    }
}