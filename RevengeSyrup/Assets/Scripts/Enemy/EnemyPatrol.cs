using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrol : MonoBehaviour
{
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private float waitTimeAtPoint = 1.5f;
    [SerializeField] private bool randomPatrol = false;
    [SerializeField] private bool loopPatrol = true;

    private int currentPointIndex = 0;
    private float waitTimer = 0f;
    private NavMeshAgent agent;
    private bool waiting = false;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (patrolPoints.Length > 0 && patrolPoints[currentPointIndex] != null)
            agent.SetDestination(patrolPoints[currentPointIndex].position);
    }

    private void Update()
    {
        if (patrolPoints.Length == 0 || agent.pathPending) return;

        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            if (!waiting)
            {
                waiting = true;
                waitTimer = waitTimeAtPoint;
            }

            waitTimer -= Time.deltaTime;

            if (waitTimer <= 0f)
            {
                GoToNextPoint();
                waiting = false;
            }
        }

        FaceMovementDirection();
    }

    private void GoToNextPoint()
    {
        if (patrolPoints.Length == 0) return;

        if (randomPatrol)
        {
            int newIndex;
            do
            {
                newIndex = Random.Range(0, patrolPoints.Length);
            } while (newIndex == currentPointIndex && patrolPoints.Length > 1);

            currentPointIndex = newIndex;
        }
        else
        {
            currentPointIndex++;
            if (currentPointIndex >= patrolPoints.Length)
            {
                if (loopPatrol)
                    currentPointIndex = 0;
                else
                    return; // Stop patrolling if not looping
            }
        }

        if (patrolPoints[currentPointIndex] != null)
            agent.SetDestination(patrolPoints[currentPointIndex].position);
    }

    private void FaceMovementDirection()
    {
        if (agent.velocity.sqrMagnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(agent.velocity.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (patrolPoints == null || patrolPoints.Length == 0) return;

        Gizmos.color = Color.cyan;
        for (int i = 0; i < patrolPoints.Length; i++)
        {
            if (patrolPoints[i] != null)
            {
                Gizmos.DrawSphere(patrolPoints[i].position, 0.3f);

                if (i + 1 < patrolPoints.Length && patrolPoints[i + 1] != null)
                    Gizmos.DrawLine(patrolPoints[i].position, patrolPoints[i + 1].position);
                else if (loopPatrol && patrolPoints[0] != null)
                    Gizmos.DrawLine(patrolPoints[i].position, patrolPoints[0].position);
            }
        }
    }
}
