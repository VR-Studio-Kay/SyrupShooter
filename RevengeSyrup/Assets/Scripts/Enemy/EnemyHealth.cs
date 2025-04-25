using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(EnemyVisuals))]
public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int health = 100;
    [SerializeField] private EnemyHealthBar healthBar;

    private int maxHealth;
    private EnemyVisuals visuals;
    private RagdollController ragdoll;
    private Collider[] colliders;

    private bool isDead = false;

    private void Start()
    {
        visuals = GetComponent<EnemyVisuals>();
        ragdoll = GetComponent<RagdollController>();
        maxHealth = health;
        healthBar?.SetMaxHealth(maxHealth);

        // Cache all colliders for disabling on death
        colliders = GetComponentsInChildren<Collider>();
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

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
        if (isDead) return;
        isDead = true;

        Debug.Log($"{gameObject.name} died.");

        // Disable AI movement
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if (agent != null) agent.enabled = false;

        // Disable all colliders
        foreach (var col in colliders)
        {
            col.enabled = false;
        }

        // Activate ragdoll
        if (ragdoll != null)
        {
            ragdoll.ToggleRagdoll(true);
            ragdoll.AddForce(-transform.forward + Vector3.up, 3f);
        }

        // Destroy enemy after ragdoll effect (add some delay for better visual effect)
        Destroy(gameObject, 3f);
    }
}
