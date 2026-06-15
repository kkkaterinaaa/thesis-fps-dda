using System.Collections;
using UnityEngine;

public class WallRaiser : MonoBehaviour
{
    [Tooltip("How far up the wall moves (world units)")]
    public float riseHeight = 5f;

    [Tooltip("Time in seconds to complete the rise")]
    public float duration = 1.5f;

    private bool _raised;

    public void Rise()
    {
        if (_raised) return;
        _raised = true;
        StartCoroutine(RiseRoutine());
    }

    private IEnumerator RiseRoutine()
    {
        Vector3 start = transform.position;
        Vector3 end   = start + Vector3.up * riseHeight;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            transform.position = Vector3.Lerp(start, end, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }

        transform.position = end;
    }
}
