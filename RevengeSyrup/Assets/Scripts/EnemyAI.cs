using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAiTutorial : MonoBehaviour
{
    [SerializeField] private GameObject projectile;
    [SerializeField] private Transform spawnPoint;

    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float sightRange = 5f;
    [SerializeField] private float walkPointRange = 5f;

    private NavMeshAgent agent;
    private Transform player;

    private Vector3 walkPoint;
    private bool walkPointSet;
    private bool alreadyAttacked;

    private int health = 5;
    private LayerMask whatIsGround;

    private Renderer enemyRenderer;
    private Color originalColor;

    private Rigidbody[] ragdollBodies;
    private Collider[] ragdollColliders;
    private Animator animator;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

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

        enemyRenderer = GetComponent<Renderer>();
        if (enemyRenderer != null)
        {
            originalColor = enemyRenderer.material.color;
        }

        // Setup ragdoll
        ragdollBodies = GetComponentsInChildren<Rigidbody>();
        ragdollColliders = GetComponentsInChildren<Collider>();

        foreach (var rb in ragdollBodies)
        {
            if (rb != GetComponent<Rigidbody>())
                rb.isKinematic = true;
        }

        foreach (var col in ragdollColliders)
        {
            if (col != GetComponent<Collider>())
                col.enabled = false;
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
                Debug.Log("Chasing player...");
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
            Debug.Log("Patrolling to: " + walkPoint);

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
            Debug.Log("Found new walk point: " + walkPoint);
        }
        else
        {
            Debug.Log("Raycast miss: walk point not on ground");
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
            rb.AddForce(transform.up * 8f, ForceMode.Impulse);

        Destroy(spawnedBullet, 5);
    }

    void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        ChangeColor(Color.yellow);

        if (health <= 0) DestroyEnemy();
    }

    void DestroyEnemy()
    {
        // Disable AI components
        if (agent != null) agent.enabled = false;
        if (animator != null) animator.enabled = false;

        // Enable ragdoll physics
        foreach (var rb in ragdollBodies)
        {
            if (rb != GetComponent<Rigidbody>())
                rb.isKinematic = false;
        }

        foreach (var col in ragdollColliders)
        {
            if (col != GetComponent<Collider>())
                col.enabled = true;
        }

        // Disable main collider to avoid physics issues
        Collider mainCollider = GetComponent<Collider>();
        if (mainCollider != null) mainCollider.enabled = false;

        // Optional: destroy after time
        Destroy(gameObject, 10f);
    }

    public void ChangeColor(Color color)
    {
        if (enemyRenderer != null)
        {
            enemyRenderer.material.color = color;
            Invoke(nameof(ResetColor), 0.2f);
        }
    }

    void ResetColor()
    {
        if (enemyRenderer != null)
        {
            enemyRenderer.material.color = originalColor;
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
