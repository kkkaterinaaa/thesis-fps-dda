using System.Collections.Generic;
using UnityEngine;

public class DdaControllerAdapter : MonoBehaviour, IDdaModule
{
    public DdaController controller;

    // Keys declared by the action-space ScriptableObject in use.
    // Edit this list (or expose it in the Inspector) if the action
    // space changes.
    private static readonly string[] Keys = {
        "enemyDamageMult",
        "enemyHPMult",
        "spawnIntensity",
        "healDropMult",
        "ammoDropMult",
    };

    void Awake()
    {
        if (controller == null) controller = GetComponent<DdaController>();
    }

    public IReadOnlyDictionary<string, float> BeginRun()
    {
        var dict = new Dictionary<string, float>();
        var action = controller != null ? controller.BeginRun() : null;
        if (action == null) return dict;

        for (int i = 0; i < Keys.Length; i++)
            dict[Keys[i]] = action.Get(Keys[i], 1f);
        return dict;
    }

    public void EndRun(string telemetryJson)
    {
        if (controller != null) controller.EndRun(telemetryJson);
    }
}