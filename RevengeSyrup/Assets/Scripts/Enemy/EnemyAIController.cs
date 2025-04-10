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
    [SerializeField] private float returnToPatrolDistance = 10f;
    private float lostPlayerTimer = 0f;
    [SerializeField] private float safeDistance = 3f; // Minimum distance the enemy should keep from the player
    [SerializeField] private float timeToForgetPlayer = 5f;
    private enum EnemyState { Patrol, Hostile }
    private EnemyState currentState = EnemyState.Patrol;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip detectSound;
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip deathSound;
    [Range(0.8f, 1.2f)] public float pitchMin = 0.95f;
    [Range(0.8f, 1.2f)] public float pitchMax = 1.05f;

    private bool hasPlayedDetectSound = false;

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

        if (distanceToPlayer < sightRange && CanSeePlayer())
        {
            currentState = EnemyState.Hostile;
            lostPlayerTimer = 0f;
        }
        else if (currentState == EnemyState.Hostile)
        {
            lostPlayerTimer += Time.deltaTime;
            if (lostPlayerTimer >= timeToForgetPlayer || distanceToPlayer > returnToPatrolDistance)
            {
                currentState = EnemyState.Patrol;
                enemyPatrol.enabled = true;
                Debug.Log("Enemy returning to patrol.");
            }
        }

        switch (currentState)
        {
            case EnemyState.Patrol:
                enemyPatrol.enabled = true;
                break;
            case EnemyState.Hostile:
                HandleHostileState(distanceToPlayer);
                break;
        }

        // Always make sure the enemy faces the player in the hostile state
        if (currentState == EnemyState.Hostile)
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
        enemyPatrol = GetComponent<EnemyPatrol>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        whatIsGround = LayerMask.GetMask("WhatIsGround");

        maxHealth = health;
        healthBar?.SetMaxHealth(maxHealth);

        if (enemyPatrol != null && patrolPoints.Length > 0)
            enemyPatrol.SetPatrolPoints(patrolPoints);

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    #endregion

    #region Behavior Logic

    private void HandleHostileState(float distanceToPlayer)
    {
        // If the player is in range for attacking and not too close
        if (distanceToPlayer < attackRange && distanceToPlayer >= safeDistance && !alreadyAttacked)
        {
            alreadyAttacked = true;
            FireBullet();
            Invoke(nameof(ResetAttack), 1f);
        }
        else if (distanceToPlayer < safeDistance)
        {
            // Move away from the player if too close
            MoveAwayFromPlayer();
        }
        else
        {
            // Continue chasing the player if within sight range
            ChasePlayer();
        }

        // Always make sure the enemy is facing the player
        LookAtPlayer();
    }

    private void MoveAwayFromPlayer()
    {
        // Calculate the direction to move away from the player
        Vector3 directionAwayFromPlayer = transform.position - player.position;

        // Use Vector3.MoveTowards to smoothly move away from the player
        Vector3 newPosition = Vector3.MoveTowards(transform.position, transform.position + directionAwayFromPlayer.normalized * safeDistance, agent.speed * Time.deltaTime);

        // Set the agent's destination to the new position to move smoothly away
        agent.SetDestination(newPosition);
    }



    private void ChasePlayer()
    {
        if (player != null)
            agent.SetDestination(player.position);
    }

    private void LookAtPlayer()
    {
        if (player == null) return;

        // Get the direction to the player
        Vector3 directionToPlayer = (player.position - transform.position).normalized;

        // Calculate the desired rotation to face the player
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);

        // Smoothly rotate the enemy towards the player using RotateTowards
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 200f * Time.deltaTime); // Adjust '200f' to change speed
    }


    private bool CanSeePlayer()
    {
        RaycastHit hit;
        Vector3 directionToPlayer = player.position - transform.position;

        if (Physics.Raycast(transform.position, directionToPlayer, out hit, sightRange))
            return hit.collider.CompareTag("Player");

        return false;
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
                rb.AddForce(transform.up * 8f, ForceMode.Impulse);
        }

        PlaySound(attackSound);
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
            DestroyEnemy();
    }

    private void DestroyEnemy()
    {
        agent.enabled = false;
        Collider mainCollider = GetComponent<Collider>();
        if (mainCollider != null) mainCollider.enabled = false;

        PlaySound(deathSound);
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
            enemyRenderer.material.color = Color.red;
    }

    #endregion

    #region Audio Helper

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.pitch = Random.Range(pitchMin, pitchMax);
            audioSource.PlayOneShot(clip);
        }
    }

    #endregion
}
