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

    private void Start()
    {
        visuals = GetComponent<EnemyVisuals>();
        ragdoll = GetComponent<RagdollController>();
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

        if (ragdoll != null)
        {
            ragdoll.ToggleRagdoll(true);
            ragdoll.AddForce(-transform.forward + Vector3.up, 3f);
        }

        Destroy(gameObject, 5f);
    }
}