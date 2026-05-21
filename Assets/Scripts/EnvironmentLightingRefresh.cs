using UnityEngine;

// Forces Unity to recompute ambient/skybox lighting after a runtime scene load.
// Without this, scenes loaded from another scene at runtime can appear very dark
// because environment/ambient probes inherit the previous scene's data.
public class EnvironmentLightingRefresh : MonoBehaviour
{
    void Start()
    {
        DynamicGI.UpdateEnvironment();
    }
}
