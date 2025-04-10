using UnityEngine;

public class GunController : MonoBehaviour
{
    [Header("Gun Settings")]
    [SerializeField] private Transform gunTransform; // Reference to the gun's transform
    [SerializeField] private float gunRotationSpeed = 200f; // How fast the gun rotates
    [SerializeField] private float maxGunAngle = 25f; // Maximum angle for gun to rotate
    [SerializeField] private GameObject projectile; // The projectile prefab
    [SerializeField] private Transform spawnPoint; // The spawn point of the bullets

    private Transform player;

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
            AimGunAtPlayer();
        }
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
        if (projectile != null && spawnPoint != null)
        {
            // Ensure the spawn point's rotation aligns with the gun's rotation
            GameObject spawnedBullet = Instantiate(projectile, spawnPoint.position, spawnPoint.rotation);
            Rigidbody rb = spawnedBullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(spawnPoint.forward * 32f, ForceMode.Impulse); // Fire the bullet forward from the spawn point
            }
            Destroy(spawnedBullet, 5f);
        }
    }
}
