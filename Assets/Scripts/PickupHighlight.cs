using UnityEngine;

public class PickupHighlight : MonoBehaviour
{
    [Header("Glow Light")]
    public bool addLight = true;
    public Color lightColor = new Color(0.7f, 0.85f, 1f);
    public float lightIntensity = 1.2f;
    public float lightRange = 2.5f;
    public float pulseSpeed = 2f;
    public float pulseAmount = 0.35f; // 0..1, fraction of intensity to modulate

    [Header("Motion")]
    public bool bob = true;
    public float bobAmplitude = 0.12f;
    public float bobSpeed = 1.5f;

    public bool rotate = true;
    public float rotateSpeed = 40f; // degrees per second, around Y

    private Light glow;
    private float baseIntensity;
    private Vector3 startLocalPos;
    private float seed;

    void Awake()
    {
        startLocalPos = transform.localPosition;
        seed = Random.Range(0f, 10f);

        if (addLight)
        {
            var lightGo = new GameObject("PickupGlow");
            lightGo.transform.SetParent(transform, false);
            lightGo.transform.localPosition = Vector3.zero;

            glow = lightGo.AddComponent<Light>();
            glow.type = LightType.Point;
            glow.color = lightColor;
            glow.intensity = lightIntensity;
            glow.range = lightRange;
            glow.shadows = LightShadows.None;

            baseIntensity = lightIntensity;
        }
    }

    void Update()
    {
        float t = Time.time + seed;

        if (glow != null)
            glow.intensity = baseIntensity * (1f + Mathf.Sin(t * pulseSpeed) * pulseAmount);

        if (bob)
        {
            Vector3 p = startLocalPos;
            p.y += Mathf.Sin(t * bobSpeed) * bobAmplitude;
            transform.localPosition = p;
        }

        if (rotate)
            transform.Rotate(0f, rotateSpeed * Time.deltaTime, 0f, Space.Self);
    }
}
