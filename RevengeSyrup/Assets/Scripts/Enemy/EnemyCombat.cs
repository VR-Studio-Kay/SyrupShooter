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
            Vector3 randomizedDir = direction +
                new Vector3(
                    Random.Range(-0.02f, 0.02f),
                    Random.Range(-0.02f, 0.02f),
                    Random.Range(-0.02f, 0.02f)
                );

            rb.linearVelocity = randomizedDir.normalized * 32f; // Velocity is more consistent than force for aiming
        }

        if (attackVFX != null)
            Instantiate(attackVFX, spawnPoint.position, Quaternion.LookRotation(direction));

        if (audioSource != null && attackSFX != null)
            audioSource.PlayOneShot(attackSFX);

        Destroy(bullet, 5f);
    }

    private void ResetAttack() => alreadyAttacked = false;
}
