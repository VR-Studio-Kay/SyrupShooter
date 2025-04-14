using UnityEngine;

public class GunController : MonoBehaviour
{
    [Header("Gun Settings")]
    [SerializeField] private Transform gunTransform; // Reference to the gun's transform
    [SerializeField] private float gunRotationSpeed = 200f; // How fast the gun rotates
    [SerializeField] private float maxGunAngle = 25f; // Maximum angle for gun to rotate
    [SerializeField] private GameObject projectile; // The projectile prefab
    [SerializeField] private Transform spawnPoint; // The spawn point of the bullets

    [Header("Fire Settings")]
    [SerializeField] private float fireDelay = 3f; // Time in seconds before the enemy can fire again
    private float fireCooldown = 0f; // Cooldown timer for firing

    private Transform player;
    private bool hasDetectedPlayer = false; // Track if the enemy has detected the player

    void Start()
    {
        // Get player reference if not already assigned
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    void Update()
    {
        if (player != null)
        {
            if (hasDetectedPlayer)
            {
                // Only aim the gun at the player if detected
                AimGunAtPlayer();
            }
        }

        // Update fireCooldown timer
        if (fireCooldown > 0f)
        {
            fireCooldown -= Time.deltaTime; // Decrease the cooldown timer
        }
    }

    // Call this method when the enemy detects the player
    public void OnPlayerDetected()
    {
        hasDetectedPlayer = true; // Enable aiming once the player is detected
    }

    // Call this method when the enemy loses sight of the player
    public void OnPlayerLost()
    {
        hasDetectedPlayer = false; // Disable aiming when the player is no longer detected
    }

    private void AimGunAtPlayer()
    {
        // Get the direction to the player
        Vector3 directionToPlayer = player.position - gunTransform.position;

        // Calculate the desired rotation to face the player
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);

        // Clamp the rotation to only allow up to the max angle
        Vector3 eulerRotation = targetRotation.eulerAngles;
        eulerRotation.x = Mathf.Clamp(eulerRotation.x, -maxGunAngle, maxGunAngle);
        eulerRotation.z = 0; // Keep the gun rotation only on the y-axis for proper orientation

        Quaternion clampedRotation = Quaternion.Euler(eulerRotation);

        // Smoothly rotate the gun towards the player
        gunTransform.rotation = Quaternion.RotateTowards(gunTransform.rotation, clampedRotation, gunRotationSpeed * Time.deltaTime);

        // Ensure the spawn point follows the gun rotation
        spawnPoint.rotation = gunTransform.rotation;
    }

    public void Fire()
    {
        if (fireCooldown <= 0f) // Check if the cooldown has elapsed
        {
            if (projectile != null && spawnPoint != null)
            {
                // Fire the projectile from the spawn point
                GameObject spawnedBullet = Instantiate(projectile, spawnPoint.position, spawnPoint.rotation);
                Rigidbody rb = spawnedBullet.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddForce(spawnPoint.forward * 32f, ForceMode.Impulse); // Fire the bullet forward from the spawn point
                }
                Destroy(spawnedBullet, 5f);
            }

            // Reset the cooldown timer after firing
            fireCooldown = fireDelay;
        }
    }
}
