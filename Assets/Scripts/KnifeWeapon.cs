using UnityEngine;

public class KnifeWeapon : MonoBehaviour
{
    public Camera fpsCam;
    public float range = 2f;
    public float radius = 0.25f;
    public float damage = 35f;
    public float cooldown = 0.4f;
    public LayerMask hitMask;

    [Header("Audio")]
    public AudioClip swingClip;
    public AudioClip hitClip;
    [Range(0f, 1f)] public float swingVolume = 0.7f;
    [Range(0f, 1f)] public float hitVolume = 0.9f;
    private AudioSource audioSource;

    private float nextAttackTime;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;
    }

    void Update()
    {
        if (TutorialManager.InputBlocked) return;

        if (Input.GetMouseButtonDown(0) && Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + cooldown;
            Attack();
        }
    }

    private void Attack()
    {
        if (fpsCam == null) return;

        if (swingClip != null && audioSource != null)
            audioSource.PlayOneShot(swingClip, swingVolume);

        TelemetryManager.RecordShotFired();

        RaycastHit hit;
        if (Physics.SphereCast(fpsCam.transform.position, radius, fpsCam.transform.forward, out hit, range, hitMask))
        {
            var health = hit.collider.GetComponentInParent<Health>();
            if (health != null)
            {
                health.TakeDamage(damage);
                TelemetryManager.RecordShotHit(false, damage);

                if (hitClip != null && audioSource != null)
                    audioSource.PlayOneShot(hitClip, hitVolume);

                if (health.currentHealth <= 0)
                    TutorialManager.CompleteObjective(TutorialManager.ObjectiveType.KillWithKnife);
            }
        }
    }
}
