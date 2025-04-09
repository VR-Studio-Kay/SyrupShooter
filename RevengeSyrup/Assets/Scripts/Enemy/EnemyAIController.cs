using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAIController : MonoBehaviour
{
    #region Fields

    [Header("Combat Settings")]
    [SerializeField] private GameObject projectile;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float sightRange = 5f;

    [Header("Health Settings")]
    [SerializeField] private int health = 100;
    private int maxHealth;
    [SerializeField] private EnemyHealthBar healthBar;

    [Header("AI Settings")]
    private NavMeshAgent agent;
    private Transform player;
    private bool alreadyAttacked;
    private LayerMask whatIsGround;

    [Header("Visual Feedback")]
    private Renderer enemyRenderer;

    [Header("Patrol Settings")]
    [SerializeField] private Transform[] patrolPoints;
    private EnemyPatrol enemyPatrol;

    [Header("AI State Settings")]
    [SerializeField] private float returnToPatrolDistance = 10f; // Distance at which enemy returns to patrol mode after losing sight of player

    private enum EnemyState { Patrol, Hostile }
    private EnemyState currentState = EnemyState.Patrol;

    private float lostPlayerTimer = 0f;
    [SerializeField] private float timeToForgetPlayer = 5f;

    #endregion

    #region Unity Methods

    private void Start()
    {
        InitializeComponents();
    }

    private void Update()
    {
        if (player == null)
        {
            currentState = EnemyState.Patrol;
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Check if the player is within sight range and there is a clear line of sight
        if (distanceToPlayer < sightRange && CanSeePlayer())
        {
            // Player detected and can be seen, switch to hostile state
            currentState = EnemyState.Hostile;
            lostPlayerTimer = 0f;
        }
        else if (currentState == EnemyState.Hostile)
        {
            // Check if the player is far enough to return to patrol
            lostPlayerTimer += Time.deltaTime;
            if (lostPlayerTimer >= timeToForgetPlayer || distanceToPlayer > returnToPatrolDistance)
            {
                currentState = EnemyState.Patrol;
                enemyPatrol.enabled = true; // Re-enable patrolling
                Debug.Log("Enemy returning to patrol.");
            }
        }

        switch (currentState)
        {
            case EnemyState.Patrol:
                // The patrolling is now handled by the EnemyPatrol script
                enemyPatrol.enabled = true;
                Debug.Log("Enemy is in Patrol state.");
                break;
            case EnemyState.Hostile:
                HandleHostileState(distanceToPlayer);
                break;
        }

        if (currentState == EnemyState.Hostile && distanceToPlayer < sightRange)
        {
            LookAtPlayer();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }

    #endregion

    #region Initialization

    private void InitializeComponents()
    {
        agent = GetComponent<NavMeshAgent>();
        enemyRenderer = GetComponent<Renderer>();
        enemyPatrol = GetComponent<EnemyPatrol>(); // Reference to the EnemyPatrol script

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogWarning("Player with tag 'Origin' not found. Enemy will not chase.");
        }

        whatIsGround = LayerMask.GetMask("WhatIsGround");
        maxHealth = health;
        healthBar?.SetMaxHealth(maxHealth);

        // Set patrol points for the enemyPatrol script
        if (enemyPatrol != null && patrolPoints.Length > 0)
        {
            enemyPatrol.SetPatrolPoints(patrolPoints);
        }
    }

    #endregion

    #region Behavior Logic

    private void HandleHostileState(float distanceToPlayer)
    {
        // Enemy will chase the player
        ChasePlayer();

        if (distanceToPlayer < attackRange && !alreadyAttacked)
        {
            alreadyAttacked = true;
            FireBullet();
            Invoke(nameof(ResetAttack), 1f);
        }
    }

    private void ChasePlayer()
    {
        if (player != null)
        {
            agent.SetDestination(player.position);
        }
    }

    private void LookAtPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    // Check if there are any obstacles between the enemy and the player
    private bool CanSeePlayer()
    {
        RaycastHit hit;
        Vector3 directionToPlayer = player.position - transform.position;

        // Perform a raycast to check if there's any obstacle between the enemy and the player
        if (Physics.Raycast(transform.position, directionToPlayer, out hit, sightRange))
        {
            if (hit.collider.CompareTag("Player"))
            {
                return true; // The player is in sight, and there's nothing blocking the way
            }
            else
            {
                return false; // The ray hit something other than the player (like a wall)
            }
        }

        return false; // No raycast hit anything, so the player is not in sight
    }

    #endregion

    #region Combat

    private void FireBullet()
    {
        if (projectile == null || spawnPoint == null)
        {
            Debug.LogError("Projectile or spawn point is not assigned.");
            return;
        }

        GameObject spawnedBullet = Instantiate(projectile, spawnPoint.position, Quaternion.identity);
        Rigidbody rb = spawnedBullet.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.AddForce(transform.forward * 32f, ForceMode.Impulse);

            if (player.position.y > transform.position.y)
            {
                rb.AddForce(transform.up * 8f, ForceMode.Impulse);
            }
        }
        else
        {
            Debug.LogError("Projectile does not have a Rigidbody attached.");
        }

        Destroy(spawnedBullet, 5f);
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    #endregion

    #region Health

    public void TakeDamage(int damage)
    {
        health -= damage;
        ChangeColor(Color.yellow);
        healthBar?.SetHealth(health);

        if (health <= 0)
        {
            DestroyEnemy();
        }
    }

    private void DestroyEnemy()
    {
        if (agent != null) agent.enabled = false;

        Collider mainCollider = GetComponent<Collider>();
        if (mainCollider != null) mainCollider.enabled = false;

        Destroy(gameObject, 2f);
    }

    #endregion

    #region Visual Feedback

    public void ChangeColor(Color newColor)
    {
        if (enemyRenderer != null)
        {
            enemyRenderer.material.color = newColor;
            Invoke(nameof(ResetColor), 0.2f);
        }
    }

    private void ResetColor()
    {
        if (enemyRenderer != null)
        {
            enemyRenderer.material.color = Color.red;
        }
    }

    #endregion
}
