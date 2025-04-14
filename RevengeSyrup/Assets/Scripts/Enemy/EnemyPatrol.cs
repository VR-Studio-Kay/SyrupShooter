using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrol : MonoBehaviour
{
    [Header("Patrol Settings")]
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private float waitTimeAtPoint = 2f;

    private NavMeshAgent agent;
    private int currentIndex = 0;
    private float waitTimer = 0f;
    private bool waiting = false;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (patrolPoints.Length > 0)
            agent.SetDestination(patrolPoints[currentIndex].position);
    }

    private void Update()
    {
        if (patrolPoints.Length == 0) return;

        if (!waiting && Vector3.Distance(transform.position, patrolPoints[currentIndex].position) < 1f)
        {
            waiting = true;
            waitTimer = 0f;
        }

        if (waiting)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitTimeAtPoint)
            {
                currentIndex = (currentIndex + 1) % patrolPoints.Length;
                agent.SetDestination(patrolPoints[currentIndex].position);
                waiting = false;
            }
        }
        else
        {
            agent.SetDestination(patrolPoints[currentIndex].position);
        }
    }

    public void SetPatrolPoints(Transform[] points)
    {
        patrolPoints = points;
        currentIndex = 0;
        if (agent != null && patrolPoints.Length > 0)
            agent.SetDestination(patrolPoints[currentIndex].position);
    }
}
