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
        public float durationSeconds;
        public float accuracy;
    }

    [Header("Damage taken per minute (in-zone target)")]
    public float dtpmTarget = 50f;
    public float dtpmTolerance = 30f;

    [Header("Kills per minute (in-zone target)")]
    public float kpmTarget = 4f;
    public float kpmTolerance = 2f;

    [Header("Match duration in seconds (in-zone target)")]
    public float durationTarget = 120f;
    public float durationTolerance = 60f;

    [Header("Accuracy (in-zone target)")]
    public float accuracyTarget = 0.5f;
    public float accuracyTolerance = 0.35f;

    [Header("Weights (should sum to ~1)")]
    public float wNotDead   = 0.15f;
    public float wDtpm      = 0.30f;
    public float wKpm       = 0.25f;
    public float wDuration  = 0.20f;
    public float wAccuracy  = 0.10f;

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

        float notDead    = (data.deaths == 0) ? 1f : 0f;
        float dtpmScore  = TriangleScore(data.damageTakenPerMinute, dtpmTarget, dtpmTolerance);
        float kpmScore   = TriangleScore(data.killsPerMinute,       kpmTarget,  kpmTolerance);
        float durScore   = TriangleScore(data.durationSeconds,      durationTarget, durationTolerance);
        float accScore   = TriangleScore(data.accuracy,             accuracyTarget, accuracyTolerance);

        float r = wNotDead  * notDead
                + wDtpm     * dtpmScore
                + wKpm      * kpmScore
                + wDuration * durScore
                + wAccuracy * accScore;

        return Mathf.Clamp01(r);
    }

    private static float TriangleScore(float x, float target, float tolerance)
    {
        float w = Mathf.Max(0.0001f, tolerance);
        float v = 1f - (Mathf.Abs(x - target) / w);
        return Mathf.Clamp01(v);
    }
}
