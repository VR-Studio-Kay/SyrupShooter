using UnityEngine;
using UnityEngine.AI;
using System;

public class EnemyAIController : MonoBehaviour
{
    public event Action OnEnemyKilled;

    [Header("Detection")]
    [SerializeField] private float sightRange = 15f;
    [SerializeField] private float fov = 110f;
    [SerializeField] private LayerMask obstructionMask;

    [Header("Combat")]
    [SerializeField] private float attackRange = 10f;
    [SerializeField] private float stopDistance = 7f;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private float strafeSpeed = 3f;
    [SerializeField] private float strafeChangeInterval = 2f;

    [Header("References")]
    [SerializeField] private EnemyPatrol patrolScript;
    [SerializeField] private EnemyCombat combat;

    private NavMeshAgent agent;
    private Transform player;
    private bool isPlayerVisible = false;
    private float cooldown = 0f;

    private float strafeTimer = 0f;
    private Vector3 strafeDirection = Vector3.zero;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (patrolScript != null)
        {
            patrolScript.enabled = true;
        }

        if (player == null)
        {
            Debug.LogWarning("Player not found. Ensure the player is tagged correctly.");
        }
    }

    private void Update()
    {
        if (player == null) return;

        cooldown -= Time.deltaTime;
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (IsPlayerInSight())
        {
            if (!isPlayerVisible)
            {
                isPlayerVisible = true;
                if (patrolScript != null) patrolScript.enabled = false;
            }

            if (distanceToPlayer > stopDistance)
            {
                agent.isStopped = false;
                agent.SetDestination(player.position);
            }
            else
            {
                agent.isStopped = false;
                HandleCombatMovement();
            }

            FacePlayer();

            if (distanceToPlayer <= attackRange && cooldown <= 0f)
            {
                combat.Attack(player);
                cooldown = attackCooldown;
            }
        }
        else
        {
            if (isPlayerVisible)
            {
                isPlayerVisible = false;
                if (patrolScript != null) patrolScript.enabled = true;
            }
        }
    }

    private void HandleCombatMovement()
    {
        strafeTimer -= Time.deltaTime;
        if (strafeTimer <= 0f)
        {
            Vector3 right = transform.right;
            strafeDirection = (UnityEngine.Random.value > 0.5f) ? right : -right;
            strafeDirection += transform.forward * 0.2f;
            strafeDirection.Normalize();
            strafeTimer = strafeChangeInterval;
        }

        Vector3 targetPosition = transform.position + strafeDirection * strafeSpeed;
        if (NavMesh.SamplePosition(targetPosition, out NavMeshHit hit, 1f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    private bool IsPlayerInSight()
    {
        Vector3 dirToPlayer = (player.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, dirToPlayer);

        if (angle < fov / 2f)
        {
            Vector3 origin = transform.position + Vector3.up;
            Vector3 destination = player.position + Vector3.up;

            if (Physics.Raycast(origin, destination - origin, out RaycastHit hit, sightRange, ~obstructionMask))
            {
                Debug.DrawRay(origin, destination - origin, Color.red);
                return hit.collider.CompareTag("Player");
            }
        }

        return false;
    }

    private void FacePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            Quaternion rotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 5f);
        }
    }

    public void Kill()
    {
        Debug.Log("Enemy killed.");
        OnEnemyKilled?.Invoke();
        Destroy(gameObject);
    }
}