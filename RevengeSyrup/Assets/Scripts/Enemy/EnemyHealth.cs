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

        Destroy(gameObject, 2f);
    }
}
