using UnityEngine;

public abstract class RewardFromJson : ScriptableObject
{
    public abstract float Evaluate(string telemetryJson);
}
