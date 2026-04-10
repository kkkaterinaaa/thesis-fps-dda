using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("Regeneration")]
    public bool canRegenerate = false;  // Может ли здоровье восстанавливаться?
    public float regenRate = 5f;        // Скорость восстановления здоровья
    public float regenCap = 0.5f;       // На сколько процентов от maxHealth восстанавливается (например, до 50%)

    public System.Action OnDeath;       // Событие смерти

    void Start()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {
        // Регистрация регенерации
        if (canRegenerate && currentHealth < maxHealth * regenCap)
        {
            RegenerateHealth();
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;

        if (GetComponent<EnemyBase>() != null || GetComponent<EnemyController>() != null || GetComponent<EnemyDeath>() != null)
        {
            Debug.Log($"{gameObject.name} HP: {currentHealth}/{maxHealth} (-{amount})", this);
        }

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            OnDeath?.Invoke();

            if (GetComponent<EnemyBase>() != null || GetComponent<EnemyController>() != null || GetComponent<EnemyDeath>() != null)
                TelemetryManager.RecordEnemyKilled();

            Debug.Log($"{gameObject.name} died!");
        }
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth); // Не даём превышать maxHealth
    }

    private void RegenerateHealth()
    {
        currentHealth += regenRate * Time.deltaTime;
        currentHealth = Mathf.Min(currentHealth, maxHealth * regenCap); // Регенерация до порога
    }

    // Устанавливает максимальное здоровье
    public void SetMax(float newMax)
    {
        maxHealth = newMax;
        currentHealth = Mathf.Min(currentHealth, maxHealth); // Ограничиваем здоровье новым max
    }
}