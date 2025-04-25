using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrol : MonoBehaviour
{
    [SerializeField] private Transform[] patrolPoints;
    private int currentIndex = 0;
    private NavMeshAgent agent;
    private bool isPatrolling = true;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (patrolPoints.Length > 0)
        {
            agent.SetDestination(patrolPoints[currentIndex].position);
        }
    }

    private void Update()
    {
        if (!isPatrolling || patrolPoints.Length == 0 || agent.pathPending) return;

        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            currentIndex = (currentIndex + 1) % patrolPoints.Length;
            agent.SetDestination(patrolPoints[currentIndex].position);
        }
    }

    public void StopPatrol()
    {
        enabled = false;
        GetComponent<NavMeshAgent>().ResetPath();
    }

    public void ResumePatrol()
    {
        enabled = true;
    }

}
