using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class DamageFlash : MonoBehaviour
{
    public static DamageFlash Instance { get; private set; }

    [Header("Flash")]
    public Color flashColor = new Color(0.8f, 0f, 0f, 0.5f);
    public float maxAlpha = 0.55f;
    public float fadeSpeed = 2.0f;
    [Tooltip("Damage value mapped to maxAlpha (clamped)")]
    public float damageForFullAlpha = 35f;

    private Image image;
    private float currentAlpha;

    void Awake()
    {
        Instance = this;
        image = GetComponent<Image>();
        var c = flashColor; c.a = 0f;
        image.color = c;
        image.raycastTarget = false;
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    void Update()
    {
        if (currentAlpha > 0f)
        {
            currentAlpha = Mathf.Max(0f, currentAlpha - fadeSpeed * Time.deltaTime);
            var c = flashColor; c.a = currentAlpha;
            image.color = c;
        }
    }

    public void Clear()
    {
        currentAlpha = 0f;
        var c = flashColor; c.a = 0f;
        if (image != null) image.color = c;
    }

    public void Flash(float damage)
    {
        float t = damageForFullAlpha > 0f ? Mathf.Clamp01(damage / damageForFullAlpha) : 1f;
        float a = Mathf.Lerp(0.25f, maxAlpha, t);
        currentAlpha = Mathf.Max(currentAlpha, a);
        var c = flashColor; c.a = currentAlpha;
        image.color = c;
    }
}
