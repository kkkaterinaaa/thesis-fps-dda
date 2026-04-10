using UnityEngine;
using UnityEngine.UI;

public class PlayerArmorBar : MonoBehaviour
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

        if (slider != null)
        {
            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.value = GetNormalizedArmor();
            slider.gameObject.SetActive(HasArmor());
        }
    }

    void Update()
    {
        if (playerHealth == null || slider == null) return;
        slider.value = GetNormalizedArmor();
        slider.gameObject.SetActive(HasArmor());
    }

    private bool HasArmor()
    {
        if (playerHealth == null) return false;
        return playerHealth.CurrentArmor > 0.001f;
    }

    private float GetNormalizedArmor()
    {
        if (playerHealth == null) return 0f;
        if (playerHealth.maxArmor <= 0f) return 0f;
        return Mathf.Clamp01(playerHealth.CurrentArmor / playerHealth.maxArmor);
    }
}
