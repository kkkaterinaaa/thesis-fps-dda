using System.Collections.Generic;

public static class DifficultyState
{
    private static readonly Dictionary<string, float> values = new();

    public static float Get(string key, float defaultValue = 1f)
        => values.TryGetValue(key, out var v) ? v : defaultValue;

    public static void Set(string key, float value) => values[key] = value;

    public static void ResetToDefaults() => values.Clear();

    public static float EnemyDamageMult => Get("enemyDamageMult");
    public static float EnemyHPMult     => Get("enemyHPMult");
    public static float SpawnIntensity  => Get("spawnIntensity");
    public static float HealDropMult    => Get("healDropMult");
    public static float AmmoDropMult    => Get("ammoDropMult");
}