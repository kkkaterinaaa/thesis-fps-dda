using System;

public static class RunSessionEvents
{
    public static event Action RunStarted;
    public static event Action<string> RunEnded;

    public static void RaiseRunStarted()
    {
        RunStarted?.Invoke();
    }

    public static void RaiseRunEnded(string telemetryJson)
    {
        RunEnded?.Invoke(telemetryJson);
    }
}
