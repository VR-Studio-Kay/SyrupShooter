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
    [SerializeField, Range(0, 360)] private float fieldOfViewAngle = 110f;

    private Animator animator;

    #endregion

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
            }
        }

        switch (currentState)
        {
            case EnemyState.Patrol:
                enemyPatrol.enabled = true;
                animator?.SetBool("IsMoving", true);
                break;

            case EnemyState.Hostile:
                enemyPatrol.enabled = false;
                HandleHostileState(distanceToPlayer);
                break;
        }
    }

    private void HandleHostileState(float distanceToPlayer)
    {
        if (distanceToPlayer < attackRange && distanceToPlayer >= safeDistance && !alreadyAttacked && CanSeePlayer())
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
        Vector3 directionAway = (transform.position - player.position).normalized;
        Vector3 retreatPosition = transform.position + directionAway * safeDistance;

        if (NavMesh.SamplePosition(retreatPosition, out NavMeshHit hit, 2f, NavMesh.AllAreas))
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

        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion rotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 5f * Time.deltaTime);
    }

    private bool CanSeePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, direction);

        if (angle < fieldOfViewAngle * 0.5f)
        {
            if (Physics.Raycast(transform.position + Vector3.up, direction, out RaycastHit hit, sightRange, ~obstructionMask))
            {
                return hit.collider.CompareTag("Player");
            }
        }
        return false;
    }

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
        if (TryGetComponent<Collider>(out var col))
            col.enabled = false;

        PlaySound(deathSound);
        Destroy(gameObject, 2f);
    }

    public void ChangeColor(Color newColor)
    {
        if (TryGetComponent<Renderer>(out var rend))
        {
            rend.material.color = newColor;
            Invoke(nameof(ResetColor), 0.2f);
        }
    }

    private void ResetColor()
    {
        if (TryGetComponent<Renderer>(out var rend))
            rend.material.color = Color.red;
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.pitch = Random.Range(pitchMin, pitchMax);
            audioSource.PlayOneShot(clip);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void ForceDetectPlayer()
    {
        currentState = EnemyState.Hostile;
        lostPlayerTimer = 0f;
        gunController.OnPlayerDetected();
        hasPlayedDetectSound = true;
        PlaySound(detectSound);
    }
}
