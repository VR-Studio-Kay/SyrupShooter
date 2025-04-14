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
        if (projectile == null || spawnPoint == null) return;

        GameObject bullet = Instantiate(projectile, spawnPoint.position, Quaternion.identity);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();

        if (rb != null)
        {
            Vector3 direction = (target.position - spawnPoint.position).normalized;
            direction += new Vector3(
                Random.Range(-0.02f, 0.02f),
                Random.Range(-0.02f, 0.02f),
                Random.Range(-0.02f, 0.02f));

            rb.AddForce(direction * 32f, ForceMode.Impulse);
        }

        if (attackVFX != null)
            Instantiate(attackVFX, spawnPoint.position, Quaternion.identity);

        if (audioSource != null && attackSFX != null)
            audioSource.PlayOneShot(attackSFX);

        Destroy(bullet, 5f);
    }

    private void ResetAttack() => alreadyAttacked = false;
}
