using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    public PlayerHealth playerHealth;
    public Slider slider;

    void Start()
    {
        if (playerHealth == null)
        {
            var playerGo = GameObject.FindGameObjectWithTag("Player");
            if (playerGo != null)
                playerHealth = playerGo.GetComponentInChildren<PlayerHealth>();

            if (playerHealth == null)
                playerHealth = FindFirstObjectByType<PlayerHealth>();
        }

        if (playerHealth != null && slider != null)
        {
            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.value = GetNormalizedHealth();
        }
    }

    void Update()
    {
        if (playerHealth == null || slider == null) return;

        slider.value = GetNormalizedHealth();
    }

    private float GetNormalizedHealth()
    {
        if (playerHealth == null) return 0f;
        if (playerHealth.maxHealth <= 0f) return 0f;
        return Mathf.Clamp01(playerHealth.CurrentHealth / playerHealth.maxHealth);
    }
}
