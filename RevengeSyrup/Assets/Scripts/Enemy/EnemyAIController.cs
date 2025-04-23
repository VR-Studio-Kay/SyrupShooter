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
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private float strafeSpeed = 3f; // NEW
    [SerializeField] private float strafeChangeInterval = 2f; // NEW

    [Header("References")]
    [SerializeField] private EnemyPatrol patrolScript;
    [SerializeField] private GunController gun;

    private NavMeshAgent agent;
    private Transform player;
    private bool isPlayerVisible = false;
    private float cooldown = 0f;

    private float strafeTimer = 0f; // NEW
    private Vector3 strafeDirection = Vector3.zero; // NEW

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (patrolScript != null)
            patrolScript.enabled = true;
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
                gun.OnPlayerDetected();
            }

            if (distanceToPlayer > attackRange)
            {
                agent.SetDestination(player.position);
            }
            else
            {
                HandleCombatMovement(); // NEW: Strafe instead of standing still
            }

            FacePlayer();

            if (distanceToPlayer <= attackRange && cooldown <= 0f)
            {
                gun.Fire();
                cooldown = attackCooldown;
            }
        }
        else
        {
            if (isPlayerVisible)
            {
                isPlayerVisible = false;
                if (patrolScript != null) patrolScript.enabled = true;
                gun.OnPlayerLost();
            }
        }
    }

    private void HandleCombatMovement() // NEW
    {
        strafeTimer -= Time.deltaTime;
        if (strafeTimer <= 0f)
        {
            // Choose new strafe direction: left or right
            Vector3 right = transform.right;
            strafeDirection = (UnityEngine.Random.value > 0.5f) ? right : -right;

            // Add a little forward motion too
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
        OnEnemyKilled?.Invoke();
        Destroy(gameObject);
    }
}
