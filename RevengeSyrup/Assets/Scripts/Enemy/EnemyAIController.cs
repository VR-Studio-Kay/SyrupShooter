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
    private int currentPatrolIndex = 0;

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
            Patrol();
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer < sightRange)
        {
            currentState = EnemyState.Hostile;
            lostPlayerTimer = 0f;
        }
        else if (currentState == EnemyState.Hostile)
        {
            lostPlayerTimer += Time.deltaTime;
            if (lostPlayerTimer >= timeToForgetPlayer)
            {
                currentState = EnemyState.Patrol;
            }
        }

        switch (currentState)
        {
            case EnemyState.Patrol:
                Patrol();
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
        healthBar?.SetMaxHealth(maxHealth);
    }

    #endregion

    #region Behavior Logic

    private void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        Transform targetPoint = patrolPoints[currentPatrolIndex];
        agent.SetDestination(targetPoint.position);

        if (Vector3.Distance(transform.position, targetPoint.position) < 1f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        }
    }

    private void HandleHostileState(float distanceToPlayer)
    {
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
