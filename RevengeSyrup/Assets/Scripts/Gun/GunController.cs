using UnityEngine;

public class GunController : MonoBehaviour
{
    [Header("Gun Settings")]
    [SerializeField] private Transform gunTransform;
    [SerializeField] private float gunRotationSpeed = 200f;
    [SerializeField] private float maxGunAngle = 25f;
    [SerializeField] private GameObject projectile;
    [SerializeField] private Transform spawnPoint;

    [Header("Fire Settings")]
    [SerializeField] private float fireDelay = 3f;
    [SerializeField] private float fireDelayRandomness = 0.5f; // Add randomness to fire delay
    [SerializeField] private int burstCount = 1; // How many bullets per shot
    [SerializeField] private float bulletSpreadAngle = 2f; // Add slight inaccuracy
    private float fireCooldown = 0f;

    [Header("Effects")]
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private AudioClip fireSound;
    private AudioSource audioSource;

    private Transform player;
    private bool hasDetectedPlayer = false;

    void Start()
    {
        // Get player reference
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
        else
            Debug.LogWarning("[GunController] Player object not found!");

        // Setup audio source
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = 1f; // 3D sound
        }

        if (spawnPoint == null)
            Debug.LogWarning("[GunController] Spawn point not assigned!");
    }

    void Update()
    {
        if (player != null && hasDetectedPlayer)
        {
            AimGunAtPlayer();
        }

        if (fireCooldown > 0f)
        {
            fireCooldown -= Time.deltaTime;
        }
    }

    public void OnPlayerDetected()
    {
        hasDetectedPlayer = true;
        Debug.Log("[GunController] Player detected, aiming started.");
    }

    public void OnPlayerLost()
    {
        hasDetectedPlayer = false;
        Debug.Log("[GunController] Player lost, aiming stopped.");
    }

    private void AimGunAtPlayer()
    {
        Vector3 directionToPlayer = player.position - gunTransform.position;
        Vector3 directionOnPlane = new Vector3(directionToPlayer.x, 0f, directionToPlayer.z);

        if (directionOnPlane.sqrMagnitude < 0.01f)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(directionOnPlane);
        gunTransform.rotation = Quaternion.RotateTowards(gunTransform.rotation, targetRotation, gunRotationSpeed * Time.deltaTime);

        Vector3 localEuler = gunTransform.localEulerAngles;
        localEuler.x = Mathf.Clamp(localEuler.x, -maxGunAngle, maxGunAngle);
        localEuler.z = 0f;
        gunTransform.localEulerAngles = localEuler;

        if (spawnPoint != null)
            spawnPoint.rotation = gunTransform.rotation;
    }

    public void Fire()
    {
        if (fireCooldown <= 0f)
        {
            if (projectile != null && spawnPoint != null)
            {
                for (int i = 0; i < burstCount; i++)
                {
                    FireSingleBullet();
                }
            }
            else
            {
                Debug.LogWarning("[GunController] Cannot fire: projectile or spawnPoint missing.");
            }

            // Set next fire delay with optional randomness
            fireCooldown = fireDelay + Random.Range(-fireDelayRandomness, fireDelayRandomness);
            Debug.Log($"[GunController] Fired! Next shot in {fireCooldown:F2} seconds.");
        }
    }

    private void FireSingleBullet()
    {
        GameObject spawnedBullet = Instantiate(projectile, spawnPoint.position, spawnPoint.rotation);

        // Apply random spread
        float spreadX = Random.Range(-bulletSpreadAngle, bulletSpreadAngle);
        float spreadY = Random.Range(-bulletSpreadAngle, bulletSpreadAngle);
        spawnedBullet.transform.Rotate(spreadX, spreadY, 0f);

        Rigidbody rb = spawnedBullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(spawnPoint.forward * 32f, ForceMode.Impulse);
        }
        else
        {
            Debug.LogWarning("[GunController] Bullet prefab missing Rigidbody!");
        }

        Destroy(spawnedBullet, 5f);

        // Play firing effects
        if (muzzleFlash != null)
            muzzleFlash.Play();

        if (fireSound != null && audioSource != null)
            audioSource.PlayOneShot(fireSound);
    }
}
