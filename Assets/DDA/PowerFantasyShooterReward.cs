using System;
using UnityEngine;

[CreateAssetMenu(menuName = "DDA/Reward/PowerFantasyShooterReward")]
public class PowerFantasyShooterReward : RewardFromJson
{
    [Serializable]
    private class SessionData
    {
        public int deaths;
        public float damageTakenPerMinute;
        public float killsPerMinute;
    }

    public float dtpmTarget = 30f;
    public float dtpmTolerance = 25f;

    public float kpmTarget = 2.2f;
    public float kpmTolerance = 1.5f;

    public override float Evaluate(string telemetryJson)
    {
        if (string.IsNullOrWhiteSpace(telemetryJson)) return 0f;

        SessionData data;
        try
        {
            data = JsonUtility.FromJson<SessionData>(telemetryJson);
        }
        catch
        {
            return 0f;
        }

        float deathScore = (data.deaths == 0) ? 1f : 0f;
        float dtpmScore = TriangleScore(data.damageTakenPerMinute, dtpmTarget, dtpmTolerance);
        float kpmScore = TriangleScore(data.killsPerMinute, kpmTarget, kpmTolerance);

        float r = 0.55f * deathScore + 0.25f * dtpmScore + 0.20f * kpmScore;
        return Mathf.Clamp01(r);
    }

    private static float TriangleScore(float x, float target, float tolerance)
    {
        float w = Mathf.Max(0.0001f, tolerance);
        float v = 1f - (Mathf.Abs(x - target) / w);
        return Mathf.Clamp01(v);
    }
}
