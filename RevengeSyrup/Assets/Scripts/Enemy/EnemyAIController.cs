using UnityEngine;
using UnityEngine.AI;
using System;

public class EnemyAIController : MonoBehaviour
{
    public event Action OnEnemyKilled; // <-- Instance event

    [Header("Detection")]
    [SerializeField] private float sightRange = 15f;
    [SerializeField] private float fov = 110f;
    [SerializeField] private LayerMask obstructionMask;

    [Header("Combat")]
    [SerializeField] private float attackRange = 10f;
    [SerializeField] private float attackCooldown = 2f;

    [Header("References")]
    [SerializeField] private EnemyPatrol patrolScript;
    [SerializeField] private GunController gun;

    private NavMeshAgent agent;
    private Transform player;
    private bool isPlayerVisible = false;
    private float cooldown = 0f;

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
                Vector3 direction = (transform.position - player.position).normalized;
                Vector3 targetPosition = player.position + direction * attackRange;

                if (NavMesh.SamplePosition(targetPosition, out NavMeshHit hit, 2f, NavMesh.AllAreas))
                {
                    agent.SetDestination(hit.position);
                }
            }
            else
            {
                agent.SetDestination(transform.position);
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
        // Optional: play animation, VFX, etc.
        OnEnemyKilled?.Invoke();
        Destroy(gameObject);
    }
}
