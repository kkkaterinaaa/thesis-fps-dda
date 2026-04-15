using UnityEngine;

public static class DifficultyState
{
    public static float EnemyDamageMult { get; set; } = 1f;
    public static float EnemyHPMult { get; set; } = 1f;
    public static float SpawnIntensity { get; set; } = 1f;
    public static float HealDropMult { get; set; } = 1f;
    public static float AmmoDropMult { get; set; } = 1f;

    public static void ResetToDefaults()
    {
        EnemyDamageMult = 1f;
        EnemyHPMult = 1f;
        SpawnIntensity = 1f;
        HealDropMult = 1f;
        AmmoDropMult = 1f;
    }
}
