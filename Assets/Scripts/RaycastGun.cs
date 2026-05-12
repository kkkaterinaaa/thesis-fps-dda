using UnityEngine;

public class RaycastGun : MonoBehaviour
{
    public float damage = 25f;
    public float range = 100f;
    public float fireRate = 0.2f;

    [Header("Ammo")]
    public int magazineSize = 10;
    public int ammoInMagazine = 10;
    public int reserveAmmo = 20;
    public float reloadDuration = 2f;
    public KeyCode reloadKey = KeyCode.R;

    private bool isReloading;

    public float spreadAngle = 0.8f;

    [Header("Tracer (visual only)")]
    public GameObject tracerPrefab;
    public float tracerSpeed = 80f;
    public float tracerLifetime = 1f;

    [Header("Projectile (optional)")]
    public GameObject projectilePrefab;
    public Transform muzzle;
    public float projectileSpeed = 45f;
    public float projectileLifetime = 3f;

    public Camera fpsCam;
    float nextFireTime = 0f;

    public LayerMask hitMask;
    public GameObject hitEffectPrefab;
    
    public Crosshair crosshair;

    [Header("Audio")]
    public AudioClip shootClip;
    public AudioClip reloadClip;
    [Range(0f, 1f)] public float shootVolume = 0.9f;
    [Range(0f, 1f)] public float reloadVolume = 0.8f;
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;
    }

    void Update()
    {
        if (!isReloading && ammoInMagazine <= 0 && reserveAmmo > 0)
        {
            StartReload(false);
        }

        if (Input.GetKeyDown(reloadKey) && !isReloading && ammoInMagazine < magazineSize && reserveAmmo > 0)
        {
            StartReload(true);
        }

        if (Input.GetMouseButton(0) && Time.time >= nextFireTime && !isReloading)
        {
            if (ammoInMagazine <= 0)
                return;

            nextFireTime = Time.time + fireRate;
            ammoInMagazine--;
            TelemetryManager.RecordShotFired();
            Shoot();
        }
    }

    private void StartReload(bool manual)
    {
        if (isReloading) return;
        if (reserveAmmo <= 0) return;
        if (ammoInMagazine >= magazineSize) return;

        TelemetryManager.RecordReload(manual);
        if (reloadClip != null && audioSource != null)
            audioSource.PlayOneShot(reloadClip, reloadVolume);
        StartCoroutine(ReloadRoutine());
    }

    private System.Collections.IEnumerator ReloadRoutine()
    {
        isReloading = true;
        yield return new WaitForSeconds(reloadDuration);

        int needed = Mathf.Max(0, magazineSize - ammoInMagazine);
        int toLoad = Mathf.Min(needed, reserveAmmo);
        ammoInMagazine += toLoad;
        reserveAmmo -= toLoad;

        isReloading = false;
    }

    public int GetAmmoInMagazine() => ammoInMagazine;
    public int GetMagazineSize() => magazineSize;
    public int GetReserveAmmo() => reserveAmmo;
    public bool IsReloading() => isReloading;

    public void AddReserveAmmo(int amount)
    {
        if (amount <= 0) return;
        reserveAmmo += amount;
    }

    void Shoot()
    {
        if (shootClip != null && audioSource != null)
            audioSource.PlayOneShot(shootClip, shootVolume);

        crosshair.AddSpread(6f);

        RaycastHit hit;

        Vector3 dir = fpsCam.transform.forward;
        if (spreadAngle > 0f)
        {
            Quaternion spread = Quaternion.Euler(Random.Range(-spreadAngle, spreadAngle), Random.Range(-spreadAngle, spreadAngle), 0f);
            dir = spread * dir;
        }

        if (Physics.Raycast(fpsCam.transform.position, dir, out hit, range, hitMask, QueryTriggerInteraction.Collide))
        {
            SpawnTracer(fpsCam.transform.position, hit.point);

            Health h = hit.collider.GetComponentInParent<Health>();
            if (h != null)
            {
                h.TakeDamage(damage);

                bool isHead = hit.collider != null && hit.collider.CompareTag("Head");
                TelemetryManager.RecordShotHit(isHead, damage);

                if (h.currentHealth <= 0)
                    TutorialManager.CompleteObjective(TutorialManager.ObjectiveType.KillWithPistol);
            }

            if (hitEffectPrefab != null)
            {
                GameObject fx = Instantiate(hitEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(fx, 0.3f);
            }
        }
        else
        {
            SpawnTracer(fpsCam.transform.position, fpsCam.transform.position + dir.normalized * range);
        }
    }

    private void SpawnTracer(Vector3 start, Vector3 end)
    {
        if (tracerPrefab == null) return;

        GameObject tracerGo = Instantiate(tracerPrefab, start, Quaternion.LookRotation(end - start));
        var tracer = tracerGo.GetComponent<HitscanTracer>();
        if (tracer != null)
            tracer.Init(end, tracerSpeed, tracerLifetime);
        else
            Destroy(tracerGo, tracerLifetime);
    }
}