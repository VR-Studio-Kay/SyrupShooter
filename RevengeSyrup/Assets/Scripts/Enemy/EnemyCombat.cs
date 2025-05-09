using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private GameObject projectile;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float attackCooldown = 1f;

    [Header("Audio and VFX")]
    [SerializeField] private AudioClip attackSFX;
    [SerializeField] private AudioClip detectionSFX; // New detection sound
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private GameObject attackVFX;

    private bool alreadyAttacked = false;
    private bool hasPlayedDetectionSound = false; // New state tracking

    public void Attack(Transform player)
    {
        if (alreadyAttacked || player == null) return;

        alreadyAttacked = true;
        FireProjectile(player);
        Invoke(nameof(ResetAttack), attackCooldown);
    }

    private void FireProjectile(Transform target)
    {
        if (projectile == null || spawnPoint == null || target == null) return;

        Vector3 direction = (target.position + Vector3.up * 0.5f - spawnPoint.position).normalized;

        GameObject bullet = Instantiate(projectile, spawnPoint.position, Quaternion.LookRotation(direction));
        Rigidbody rb = bullet.GetComponent<Rigidbody>();

        if (rb != null)
        {
            Vector3 randomizedDir = direction +
                new Vector3(
                    Random.Range(-0.02f, 0.02f),
                    Random.Range(-0.02f, 0.02f),
                    Random.Range(-0.02f, 0.02f)
                );

            Vector3 finalDir = randomizedDir.normalized;

            rb.linearVelocity = finalDir * 32f; // corrected to velocity
            if (rb.linearVelocity.magnitude < 0.1f)
                rb.AddForce(finalDir * 1000f);
        }

        if (attackVFX != null)
            Instantiate(attackVFX, spawnPoint.position, Quaternion.LookRotation(direction));

        if (audioSource != null && attackSFX != null)
            audioSource.PlayOneShot(attackSFX);

        Destroy(bullet, 5f);
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    // ----------------
    // Detection system:
    // ----------------

    public void OnPlayerDetected()
    {
        if (!hasPlayedDetectionSound)
        {
            if (audioSource != null && detectionSFX != null)
            {
                audioSource.PlayOneShot(detectionSFX);
                Debug.Log("[EnemyCombat] Detection sound played!");
            }
            hasPlayedDetectionSound = true;
        }
    }

    public void OnPlayerLost()
    {
        hasPlayedDetectionSound = false;
        Debug.Log("[EnemyCombat] Player lost, reset detection sound.");
    }
}
