using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrol : MonoBehaviour
{
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private float pauseDuration = 1f;
    [SerializeField] private float rotationSpeed = 360f;

    private NavMeshAgent agent;
    private int currentPointIndex = 0;
    private bool isPaused = false;
    private float pauseTimer = 0f;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (patrolPoints.Length > 0)
        {
            agent.SetDestination(patrolPoints[currentPointIndex].position);
        }
    }

    private void Update()
    {
        if (isPaused) HandlePause();
        else HandlePatrol();
    }

    private void HandlePatrol()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.2f)
        {
            Debug.Log("Reached patrol point. Pausing to scan...");
            isPaused = true;
            pauseTimer = pauseDuration;
            agent.isStopped = true;
        }
    }

    private void HandlePause()
    {
        pauseTimer -= Time.deltaTime;

        // Simulate robotic scanning (optional)
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

        if (pauseTimer <= 0f)
        {
            isPaused = false;
            agent.isStopped = false;
            currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
            agent.SetDestination(patrolPoints[currentPointIndex].position);

            Debug.Log("Resuming patrol to next point.");
        }
    }

    public void EnablePatrol()
    {
        enabled = true;
        if (!agent.hasPath && patrolPoints.Length > 0)
            agent.SetDestination(patrolPoints[currentPointIndex].position);
        Debug.Log("Robot patrol enabled.");
    }

    public void DisablePatrol()
    {
        enabled = false;
        if (agent != null) agent.ResetPath();
        Debug.Log("Robot patrol disabled.");
    }
}
