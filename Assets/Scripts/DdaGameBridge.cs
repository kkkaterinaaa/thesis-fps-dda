using UnityEngine;
using System.Collections.Generic;

public class DdaGameBridge : MonoBehaviour
{
    [Tooltip("A MonoBehaviour that implements IDdaModule.")]
    public MonoBehaviour ddaModule;

    private IDdaModule module;

    void Awake()
    {
        module = ddaModule as IDdaModule;
        if (ddaModule != null && module == null)
            Debug.LogError($"{ddaModule.name} does not implement IDdaModule", this);
    }

    void OnEnable()
    {
        RunSessionEvents.RunStarted += OnRunStarted;
        RunSessionEvents.RunEnded   += OnRunEnded;
    }

    void OnDisable()
    {
        RunSessionEvents.RunStarted -= OnRunStarted;
        RunSessionEvents.RunEnded   -= OnRunEnded;
    }

    private void OnRunStarted()
    {
        if (module == null)
        {
            DifficultyState.ResetToDefaults();
            return;
        }

        Apply(module.BeginRun());

        var enemies = FindObjectsByType<EnemyController>(FindObjectsSortMode.None);
        for (int i = 0; i < enemies.Length; i++)
            if (enemies[i] != null) enemies[i].ApplyStats();
    }

    private void OnRunEnded(string telemetryJson)
    {
        module?.EndRun(telemetryJson);
    }

    private static void Apply(IReadOnlyDictionary<string, float> action)
    {
        DifficultyState.ResetToDefaults();
        if (action == null) return;

        foreach (var kv in action)
            DifficultyState.Set(kv.Key, kv.Value);
    }
}
