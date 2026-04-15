using UnityEngine;

[CreateAssetMenu(menuName = "DDA/LinUcbSettings")]
public class LinUcbSettings : ScriptableObject
{
    public float alpha = 0.8f;
    public float lambda = 1.0f;

    [Range(0.90f, 1.0f)]
    public float forgettingFactor = 0.99f;

    public int candidateCount = 100;
    [Range(0f, 1f)] public float globalCandidateRatio = 0.7f;
    [Range(0f, 1f)] public float localCandidateRatio = 0.25f;

    public int anchorCandidateCount = 5;

    public int seed = 0;
}
