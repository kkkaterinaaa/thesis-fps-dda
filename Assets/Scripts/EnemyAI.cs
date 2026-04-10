using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public Transform target;
    public float stoppingDistance = 2f;

    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (target == null) return;
        agent.SetDestination(target.position);

        if (agent.remainingDistance <= stoppingDistance)
        {
            // future: attack logic
        }
    }
}