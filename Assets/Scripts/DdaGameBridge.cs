using UnityEngine;

public class DdaGameBridge : MonoBehaviour
{
    public DdaController dda;

    void Awake()
    {
        if (dda == null)
            dda = FindFirstObjectByType<DdaController>();
    }

    void OnEnable()
    {
        RunSessionEvents.RunStarted += OnRunStarted;
        RunSessionEvents.RunEnded += OnRunEnded;
    }

    void OnDisable()
    {
        RunSessionEvents.RunStarted -= OnRunStarted;
        RunSessionEvents.RunEnded -= OnRunEnded;
    }

    private void OnRunStarted()
    {
        if (dda == null)
            dda = FindFirstObjectByType<DdaController>();

        if (dda == null)
        {
            DifficultyState.ResetToDefaults();
            return;
        }

        var action = dda.BeginRun();
        Apply(action);

        var enemies = FindObjectsByType<EnemyController>(FindObjectsSortMode.None);
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i] != null)
                enemies[i].ApplyStats();
        }
    }

    private void OnRunEnded(string telemetryJson)
    {
        if (dda == null)
            dda = FindFirstObjectByType<DdaController>();

        if (dda != null)
            dda.EndRun(telemetryJson);
    }

    private static void Apply(DdaAction action)
    {
        DifficultyState.ResetToDefaults();
        if (action == null) return;

        DifficultyState.EnemyDamageMult = action.Get("enemyDamageMult", 1f);
        DifficultyState.EnemyHPMult = action.Get("enemyHPMult", 1f);
        DifficultyState.SpawnIntensity = action.Get("spawnIntensity", 1f);
        DifficultyState.HealDropMult = action.Get("healDropMult", 1f);
        DifficultyState.AmmoDropMult = action.Get("ammoDropMult", 1f);
    }
}
