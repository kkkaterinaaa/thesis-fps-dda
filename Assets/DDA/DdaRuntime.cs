using UnityEngine;

public static class DdaRuntime
{
    public static DdaAction CurrentAction { get; private set; }

    public static void SetAction(DdaAction action)
    {
        CurrentAction = action;
    }

    public static float EnemyDamageMult => CurrentAction != null ? CurrentAction.Get("enemyDamageMult", 1f) : 1f;
    public static float EnemyHPMult => CurrentAction != null ? CurrentAction.Get("enemyHPMult", 1f) : 1f;
    public static float EnemyFireRateMult => CurrentAction != null ? CurrentAction.Get("enemyFireRateMult", 1f) : 1f;
    public static float HealDropMult => CurrentAction != null ? CurrentAction.Get("healDropMult", 1f) : 1f;
    public static float AmmoDropMult => CurrentAction != null ? CurrentAction.Get("ammoDropMult", 1f) : 1f;
}
