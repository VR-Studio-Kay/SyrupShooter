using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    [SerializeField] private GameObject projectile;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private AudioClip attackSFX;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private GameObject attackVFX;

    private bool alreadyAttacked = false;

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
            // Adding randomness to the projectile direction for a more realistic effect
            Vector3 randomizedDir = direction +
                new Vector3(
                    Random.Range(-0.02f, 0.02f),
                    Random.Range(-0.02f, 0.02f),
                    Random.Range(-0.02f, 0.02f)
                );

            Vector3 finalDir = randomizedDir.normalized;

            rb.velocity = finalDir * 32f;  // Using velocity instead of linearVelocity for better control

            // Optional fallback if velocity is too low
            if (rb.velocity.magnitude < 0.1f)
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
}
