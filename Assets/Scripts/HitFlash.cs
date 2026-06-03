using System.Collections.Generic;
using UnityEngine;

public class HitFlash : MonoBehaviour
{
    [Header("Flash")]
    public Color flashColor = new Color(1f, 0.85f, 1f, 1f);
    public float duration = 0.15f;
    [Tooltip("If empty, will auto-collect all child Renderers on Awake")]
    public Renderer[] renderers;

    private struct Entry
    {
        public Material material;
        public string colorProp;
        public Color originalColor;
        public bool hasColor;
        public Color originalEmission;
        public bool hasEmission;
        public bool emissionWasEnabled;
    }

    private readonly List<Entry> entries = new List<Entry>();
    private float timer;
    private bool flashing;

    void Awake()
    {
        if (renderers == null || renderers.Length == 0)
            renderers = GetComponentsInChildren<Renderer>(true);

        foreach (var r in renderers)
        {
            if (r == null) continue;
            // Instantiate material so we don't modify shared assets
            var mats = r.materials;
            for (int i = 0; i < mats.Length; i++)
            {
                var m = mats[i];
                if (m == null) continue;
                var e = new Entry { material = m };
                if (m.HasProperty("_Color"))
                {
                    e.hasColor = true;
                    e.colorProp = "_Color";
                    e.originalColor = m.GetColor("_Color");
                }
                if (m.HasProperty("_EmissionColor"))
                {
                    e.hasEmission = true;
                    e.originalEmission = m.GetColor("_EmissionColor");
                    e.emissionWasEnabled = m.IsKeywordEnabled("_EMISSION");
                }
                entries.Add(e);
            }
        }
    }

    void Update()
    {
        if (!flashing) return;
        timer -= Time.deltaTime;
        if (timer <= 0f) Restore();
    }

    public void Flash()
    {
        for (int i = 0; i < entries.Count; i++)
        {
            var e = entries[i];
            if (e.material == null) continue;
            if (e.hasColor) e.material.SetColor(e.colorProp, flashColor);
            if (e.hasEmission)
            {
                e.material.EnableKeyword("_EMISSION");
                e.material.SetColor("_EmissionColor", flashColor * 2f);
            }
        }
        timer = duration;
        flashing = true;
    }

    private void Restore()
    {
        for (int i = 0; i < entries.Count; i++)
        {
            var e = entries[i];
            if (e.material == null) continue;
            if (e.hasColor) e.material.SetColor(e.colorProp, e.originalColor);
            if (e.hasEmission)
            {
                e.material.SetColor("_EmissionColor", e.originalEmission);
                if (!e.emissionWasEnabled)
                    e.material.DisableKeyword("_EMISSION");
            }
        }
        flashing = false;
    }
}
