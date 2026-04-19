using System.Collections.Generic;

public interface IDdaModule
{
    IReadOnlyDictionary<string, float> BeginRun();
    void EndRun(string telemetryJson);
}