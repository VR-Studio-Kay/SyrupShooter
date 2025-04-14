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
    [SerializeField] private float attackCooldown = 3f;

    [Header("Health Settings")]
    [SerializeField] private int health = 100;
    private int maxHealth;
    [SerializeField] private EnemyHealthBar healthBar;

    [Header("AI Settings")]
    private NavMeshAgent agent;
    private Transform player;
    private bool alreadyAttacked;
    private LayerMask whatIsGround;

    [Header("Patrol Settings")]
    [SerializeField] private Transform[] patrolPoints;
    private EnemyPatrol enemyPatrol;

    [Header("AI State Settings")]
    [SerializeField] private float returnToPatrolDistance = 10f;
    private float lostPlayerTimer = 0f;
    [SerializeField] private float safeDistance = 3f;
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

    [Header("Gun")]
    [SerializeField] private GunController gunController;

    [Header("Vision Settings")]
    [SerializeField] private LayerMask obstructionMask;

    private Animator animator;

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

            if (!hasPlayedDetectSound)
            {
                PlaySound(detectSound);
                hasPlayedDetectSound = true;
            }

            gunController.OnPlayerDetected();
        }
        else if (currentState == EnemyState.Hostile)
        {
            lostPlayerTimer += Time.deltaTime;
            if (lostPlayerTimer >= timeToForgetPlayer || distanceToPlayer > returnToPatrolDistance)
            {
                currentState = EnemyState.Patrol;
                enemyPatrol.enabled = true;
                gunController.OnPlayerLost();
                hasPlayedDetectSound = false;
                Debug.Log("Enemy returning to patrol.");
            }
        }

        switch (currentState)
        {
            case EnemyState.Patrol:
                enemyPatrol.enabled = true;
                animator?.SetBool("IsMoving", false);
                break;

            case EnemyState.Hostile:
                HandleHostileState(distanceToPlayer);
                break;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }

    private void OnDrawGizmos()
    {
        if (player != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, player.position);
        }
    }

    #endregion

    #region Initialization

    private void InitializeComponents()
    {
        agent = GetComponent<NavMeshAgent>();
        enemyPatrol = GetComponent<EnemyPatrol>();
        animator = GetComponent<Animator>();

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
        if (distanceToPlayer < attackRange && distanceToPlayer >= safeDistance && !alreadyAttacked)
        {
            alreadyAttacked = true;
            gunController.Fire();
            PlaySound(attackSound);
            Invoke(nameof(ResetAttack), attackCooldown);
        }
        else if (distanceToPlayer < safeDistance)
        {
            MoveAwayFromPlayer();
        }
        else
        {
            ChasePlayer();
        }

        animator?.SetBool("IsMoving", true);
        LookAtPlayer();
    }

    private void MoveAwayFromPlayer()
    {
        Vector3 directionAwayFromPlayer = (transform.position - player.position).normalized;
        Vector3 targetPos = transform.position + directionAwayFromPlayer * safeDistance;

        if (NavMesh.SamplePosition(targetPos, out NavMeshHit hit, 2f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    private void ChasePlayer()
    {
        if (player != null)
            agent.SetDestination(player.position);
    }

    private void LookAtPlayer()
    {
        if (player == null) return;

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 200f * Time.deltaTime);
    }

    private bool CanSeePlayer()
    {
        Vector3 directionToPlayer = player.position - transform.position;

        if (Physics.Raycast(transform.position, directionToPlayer.normalized, out RaycastHit hit, sightRange, ~obstructionMask))
            return hit.collider.CompareTag("Player");

        return false;
    }

    public void ForceDetectPlayer()
    {
        currentState = EnemyState.Hostile;
        lostPlayerTimer = 0f;
        gunController.OnPlayerDetected();
        hasPlayedDetectSound = true;
        PlaySound(detectSound);
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
        Renderer enemyRenderer = GetComponent<Renderer>();
        if (enemyRenderer != null)
        {
            enemyRenderer.material.color = newColor;
            Invoke(nameof(ResetColor), 0.2f);
        }
    }

    private void ResetColor()
    {
        Renderer enemyRenderer = GetComponent<Renderer>();
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

    #region Reset Attack

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    #endregion
}
