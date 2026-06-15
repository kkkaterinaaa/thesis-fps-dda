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
    public System.Action<float> OnDamaged; // amount

    [Header("Audio")]
    public AudioClip hitClip;
    public AudioClip deathClip;
    [Range(0f, 1f)] public float hitVolume = 0.8f;
    [Range(0f, 1f)] public float deathVolume = 0.9f;
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 1f;
            audioSource.minDistance = 2f;
            audioSource.maxDistance = 30f;
        }
    }

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
        OnDamaged?.Invoke(amount);

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

            if (deathClip != null)
                AudioSource.PlayClipAtPoint(deathClip, transform.position, deathVolume);

            Debug.Log($"{gameObject.name} died!");
        }
        else
        {
            if (hitClip != null && audioSource != null)
                audioSource.PlayOneShot(hitClip, hitVolume);

            var flash = GetComponent<HitFlash>();
            if (flash == null) flash = GetComponentInChildren<HitFlash>();
            if (flash != null) flash.Flash();
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
    public void SetMax(float newMax, bool fillToMax = false)
    {
        maxHealth = newMax;
        if (fillToMax)
            currentHealth = maxHealth;
        else
            currentHealth = Mathf.Min(currentHealth, maxHealth);
    }
}