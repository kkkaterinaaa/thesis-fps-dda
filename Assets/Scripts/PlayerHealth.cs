using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

    public float maxArmor = 100f;
    private float currentArmor;
    [Range(0f, 1f)]
    public float armorDamageReduction = 0.75f;
    public bool logDamage = false;

    public float CurrentHealth => currentHealth;
    public float CurrentArmor => currentArmor;

    void Start()
    {
        currentHealth = maxHealth;
        currentArmor = 0f;
    }

    public void TakeDamage(float amount)
    {
        if (amount <= 0f) return;

        float damageToHealth = amount;
        float blocked = 0f;

        if (currentArmor > 0f && armorDamageReduction > 0f)
        {
            float reduction = Mathf.Clamp01(armorDamageReduction);
            float desiredBlocked = amount * reduction;
            blocked = Mathf.Min(currentArmor, desiredBlocked);
            currentArmor -= blocked;
            damageToHealth = amount - blocked;
        }

        currentHealth -= damageToHealth;

        TelemetryManager.RecordPlayerDamage(amount, blocked, damageToHealth);

        if (logDamage)
            Debug.Log($"Player damage: in={amount:0.##}, blocked={blocked:0.##}, hp={damageToHealth:0.##} | HP {currentHealth:0.##}/{maxHealth} Armor {currentArmor:0.##}/{maxArmor}", this);

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            currentArmor = 0;
            TelemetryManager.RecordPlayerDeath();
            if (TelemetryManager.Instance != null)
                TelemetryManager.Instance.EndSession("death");
            Die();
        }
    }

    public void AddArmor(float amount)
    {
        if (amount <= 0f) return;
        currentArmor = Mathf.Clamp(currentArmor + amount, 0f, maxArmor);
    }

    public void Heal(float amount)
    {
        if (amount <= 0f) return;
        if (currentHealth <= 0f) return;
        currentHealth = Mathf.Clamp(currentHealth + amount, 0f, maxHealth);
    }

    void Die()
    {
        if (DeathScreen.Instance != null)
            DeathScreen.Instance.Show();
        else
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
