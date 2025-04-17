using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(EnemyVisuals))]
public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int health = 100;
    [SerializeField] private EnemyHealthBar healthBar;

    private int maxHealth;
    private EnemyVisuals visuals;

    private void Start()
    {
        visuals = GetComponent<EnemyVisuals>();
        maxHealth = health;
        healthBar?.SetMaxHealth(maxHealth);
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        visuals.ChangeColor(Color.yellow);
        healthBar?.SetHealth(health);

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        GetComponent<NavMeshAgent>().enabled = false;

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>(); // Add Rigidbody if not already there
        }

        rb.isKinematic = false;
        rb.useGravity = true;

        // Optional: Add a backward force to simulate collapsing
        rb.AddForce(-transform.forward * 2f + Vector3.up * 2f, ForceMode.Impulse);

        Destroy(gameObject, 4f); // Delay destroy so we can see the fall
    }
}
