using System;
using UnityEngine;

[Serializable]
public class DdaAction
{
    public string[] keys;
    public float[] values;

    public DdaAction(string[] keys, float[] values)
    {
        this.keys = keys;
        this.values = values;
    }

    public float Get(string key, float defaultValue = 1f)
    {
        if (keys == null || values == null) return defaultValue;
        int n = Mathf.Min(keys.Length, values.Length);
        for (int i = 0; i < n; i++)
        {
            if (string.Equals(keys[i], key, StringComparison.Ordinal))
                return values[i];
        }
        return defaultValue;
    }
}
