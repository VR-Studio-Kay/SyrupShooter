using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAiTutorial : MonoBehaviour
{
    [Header("Combat Settings")]
    [SerializeField] private GameObject projectile;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float sightRange = 5f;
    [SerializeField] private float walkPointRange = 5f;

    [Header("Health Settings")]
    [SerializeField] private int health = 100;
    private int maxHealth;
    [SerializeField] private EnemyHealthBar healthBar;

    [Header("AI")]
    private NavMeshAgent agent;
    private Transform player;
    private Vector3 walkPoint;
    private bool walkPointSet;
    private bool alreadyAttacked;
    private LayerMask whatIsGround;

    [Header("Visual Feedback")]
    private Renderer enemyRenderer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        enemyRenderer = GetComponent<Renderer>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Origin");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogWarning("Player with tag 'Origin' not found. Enemy will not chase.");
        }

        whatIsGround = LayerMask.GetMask("Ground");

        maxHealth = health;

        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
        }

        SearchWalkPoint();
    }

    void Update()
    {
        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer < sightRange)
            {
                ChasePlayer();
            }
            else
            {
                Patrol();
            }

            if (distanceToPlayer < attackRange && !alreadyAttacked)
            {
                alreadyAttacked = true;
                FireBullet();
                Invoke(nameof(ResetAttack), 1f);
            }
        }
        else
        {
            Patrol();
        }
    }

    void Patrol()
    {
        if (!walkPointSet)
        {
            SearchWalkPoint();
        }
        else
        {
            agent.SetDestination(walkPoint);

            if (Vector3.Distance(transform.position, walkPoint) < 1f)
            {
                walkPointSet = false;
            }
        }
    }

    void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
        {
            walkPointSet = true;
        }
    }

    void ChasePlayer()
    {
        if (player != null)
        {
            agent.SetDestination(player.position);
        }
    }

    void FireBullet()
    {
        GameObject spawnedBullet = Instantiate(projectile);
        Rigidbody rb = spawnedBullet.GetComponent<Rigidbody>();
        rb.position = spawnPoint.position;
        rb.AddForce(transform.forward * 32f, ForceMode.Impulse);

        if (player.position.y > transform.position.y)
        {
            rb.AddForce(transform.up * 8f, ForceMode.Impulse);
        }

        Destroy(spawnedBullet, 5f);
    }

    void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        ChangeColor(Color.yellow);

        if (healthBar != null)
        {
            healthBar.SetHealth(health);
        }

        if (health <= 0)
        {
            DestroyEnemy();
        }
    }

    void DestroyEnemy()
    {
        if (agent != null) agent.enabled = false;
        Collider mainCollider = GetComponent<Collider>();
        if (mainCollider != null) mainCollider.enabled = false;

        Destroy(gameObject, 2f);
    }

    public void ChangeColor(Color newColor)
    {
        if (enemyRenderer != null)
        {
            enemyRenderer.material.color = newColor;
            Invoke(nameof(ResetColor), 0.2f);
        }
    }

    void ResetColor()
    {
        if (enemyRenderer != null)
        {
            enemyRenderer.material.color = Color.red;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
