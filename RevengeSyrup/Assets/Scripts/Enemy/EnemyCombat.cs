using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    [SerializeField] private GameObject projectile;
    [SerializeField] private Transform spawnPoint;

    private bool alreadyAttacked = false;

    public void Attack(Transform player)
    {
        if (alreadyAttacked) return;

        alreadyAttacked = true;
        FireProjectile(player);
        Invoke(nameof(ResetAttack), 1f);
    }

    private void FireProjectile(Transform target)
    {
        if (projectile == null || spawnPoint == null) return;

        GameObject bullet = Instantiate(projectile, spawnPoint.position, Quaternion.identity);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();

        if (rb != null)
        {
            Vector3 direction = (target.position - spawnPoint.position).normalized;
            rb.AddForce(direction * 32f, ForceMode.Impulse);
        }

        Destroy(bullet, 5f);
    }

    private void ResetAttack() => alreadyAttacked = false;
}
