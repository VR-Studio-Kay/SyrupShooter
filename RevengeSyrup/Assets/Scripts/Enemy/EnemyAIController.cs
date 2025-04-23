using UnityEngine;
using UnityEngine.AI;
using System;

[RequireComponent(typeof(NavMeshAgent))]
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
    [SerializeField] private float strafeSpeed = 3f;
    [SerializeField] private float strafeChangeInterval = 2f;

    [Header("References")]
    [SerializeField] private EnemyPatrol patrolScript;
    [SerializeField] private GunController gun;

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
            Debug.Log("Patrol script enabled on start.");
        }

        if (player == null)
        {
            Debug.LogWarning("Player not found. Make sure the Player has the 'Player' tag.");
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
                Debug.Log("Player detected!");

                if (patrolScript != null)
                {
                    patrolScript.enabled = false;
                    Debug.Log("Patrol disabled due to player detection.");
                }

                gun.OnPlayerDetected();
            }

            if (distanceToPlayer > attackRange)
            {
                // Chase the player
                agent.updateRotation = true;
                agent.ResetPath();
                agent.SetDestination(player.position);
                Debug.Log("Chasing player...");
            }
            else
            {
                // Strafe around the player
                agent.updateRotation = false;
                HandleCombatMovement();
                Debug.Log("Strafing around player.");
            }

            FacePlayer();

            if (distanceToPlayer <= attackRange && cooldown <= 0f)
            {
                gun.Fire();
                cooldown = attackCooldown;
                Debug.Log("Firing at player.");
            }
        }
        else
        {
            if (isPlayerVisible)
            {
                isPlayerVisible = false;
                Debug.Log("Lost sight of player. Returning to patrol.");

                if (patrolScript != null)
                {
                    patrolScript.enabled = true;
                    Debug.Log("Patrol re-enabled.");
                }

                gun.OnPlayerLost();
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

            // Slight forward motion
            strafeDirection += transform.forward * 0.2f;
            strafeDirection.Normalize();

            strafeTimer = strafeChangeInterval;

            Debug.Log($"New strafe direction: {strafeDirection}");
        }

        Vector3 targetPos = transform.position + strafeDirection * strafeSpeed;
        if (NavMesh.SamplePosition(targetPos, out NavMeshHit hit, 1f, NavMesh.AllAreas))
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

                bool hitPlayer = hit.collider.CompareTag("Player");
                Debug.Log($"Raycast hit: {hit.collider.name}, Is player: {hitPlayer}");

                return hitPlayer;
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
