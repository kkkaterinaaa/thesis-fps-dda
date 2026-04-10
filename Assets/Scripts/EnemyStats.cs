using UnityEngine;

[CreateAssetMenu(menuName = "DDA/EnemyStats")]
public class EnemyStats : ScriptableObject
{
    public EnemyType type;

    [Header("Core")]
    public float maxHP = 100f;
    public float moveSpeed = 3.5f;

    [Header("Combat")]
    public float damage = 10f;
    public float fireRate = 1.0f;
    public float accuracy = 0.7f;
    public float attackRange = 10f;
    public float hitChance = 0.6f;
    public float headshotChance = 0.2f;

    public float bodyDamage = 10f;
    public float headDamage = 25f;

    [Header("Behavior")]
    public float desiredDistance = 1.2f;
    public float idealDistance = 6f;
}