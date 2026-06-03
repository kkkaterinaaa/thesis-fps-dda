using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    public enum MotionKind { Idle, Walk, Sprint, Crouch }

    [Header("Footsteps")]
    public AudioClip[] footstepClips;
    [Range(0f, 1f)] public float footstepVolume = 0.7f;
    public float walkStepInterval   = 0.48f;
    public float sprintStepInterval = 0.34f;
    public float crouchStepInterval = 0.65f;

    [Header("Jump")]
    public AudioClip jumpClip;
    [Range(0f, 1f)] public float jumpVolume = 0.9f;

    [Header("Hit")]
    public AudioClip hitClip;
    [Range(0f, 1f)] public float hitVolume = 0.8f;

    private AudioSource source;
    private float nextStepTime;
    private int lastIndex = -1;

    void Awake()
    {
        source = GetComponent<AudioSource>();
        if (source == null)
            source = gameObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.spatialBlend = 0f;
    }

    public void TickFootsteps(bool grounded, float horizontalSpeed, MotionKind kind)
    {
        if (!grounded || horizontalSpeed < 0.1f || kind == MotionKind.Idle)
        {
            if (nextStepTime < Time.time) nextStepTime = Time.time;
            return;
        }

        if (Time.time < nextStepTime) return;
        if (footstepClips == null || footstepClips.Length == 0) return;

        int i = PickIndex(footstepClips.Length);
        source.PlayOneShot(footstepClips[i], footstepVolume);
        lastIndex = i;

        float interval;
        switch (kind)
        {
            case MotionKind.Sprint: interval = sprintStepInterval; break;
            case MotionKind.Crouch: interval = crouchStepInterval; break;
            default:                interval = walkStepInterval;   break;
        }
        nextStepTime = Time.time + interval;
    }

    public void PlayJump()
    {
        if (jumpClip == null || source == null) return;
        source.PlayOneShot(jumpClip, jumpVolume);
    }

    public void PlayHit()
    {
        if (hitClip == null || source == null) return;
        source.PlayOneShot(hitClip, hitVolume);
    }

    private int PickIndex(int n)
    {
        if (n <= 1) return 0;
        int i = Random.Range(0, n);
        if (i == lastIndex) i = (i + 1) % n;
        return i;
    }
}
