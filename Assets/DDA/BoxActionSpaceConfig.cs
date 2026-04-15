using System;
using UnityEngine;

[CreateAssetMenu(menuName = "DDA/BoxActionSpaceConfig")]
public class BoxActionSpaceConfig : ScriptableObject
{
    [Serializable]
    public class Dimension
    {
        public string key;
        public float min = 0.5f;
        public float max = 1.5f;
        public float localStdDev = 0.1f;
        public bool normalizeToUnit = true;
    }

    [Serializable]
    public class Anchor
    {
        public string name;
        public float[] values;
    }

    public Dimension[] dimensions;

    public Anchor[] anchors;

    public int DimensionCount => dimensions != null ? dimensions.Length : 0;
}
