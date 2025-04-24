using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrol : MonoBehaviour
{
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private float pointReachThreshold = 1f;
    [SerializeField] private float waitTimeAtPoint = 2f;

    private NavMeshAgent agent;
    private int currentPatrolIndex = 0;
    private float waitTimer = 0f;
    private bool waiting = false;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (patrolPoints.Length == 0)
        {
            Debug.LogWarning("No patrol points assigned to EnemyPatrol.");
            enabled = false;
            return;
        }

        MoveToNextPoint();
    }

    private void Update()
    {
        if (agent.pathPending || patrolPoints.Length == 0)
            return;

        if (!waiting && agent.remainingDistance <= pointReachThreshold)
        {
            waiting = true;
            waitTimer = waitTimeAtPoint;
            agent.isStopped = true;
        }

        if (waiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0f)
            {
                waiting = false;
                MoveToNextPoint();
            }
        }
    }

    private void MoveToNextPoint()
    {
        if (patrolPoints.Length == 0)
            return;

        agent.isStopped = false;
        agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;

        Debug.Log($"Moving to patrol point: {currentPatrolIndex}");
    }

    private void OnEnable()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        agent.isStopped = false;
        MoveToNextPoint();
    }

    private void OnDisable()
    {
        if (agent != null)
        {
            agent.ResetPath();
            agent.isStopped = true;
        }
    }
}
