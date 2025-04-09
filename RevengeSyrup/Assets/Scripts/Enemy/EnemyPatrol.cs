using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrol : MonoBehaviour
{
    [Header("Patrol Settings")]
    [SerializeField] private Transform[] patrolPoints;  // Patrol points for the enemy
    private int currentPatrolIndex = 0;  // Index for patrol points
    private NavMeshAgent agent;  // NavMeshAgent for movement
    private Transform targetPoint;  // The current patrol point

    private void Start()
    {
        // Initialize the NavMeshAgent
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent component not found on this object.");
        }
    }

    private void Update()
    {
        // If there are no patrol points, exit the Update function
        if (patrolPoints.Length == 0) return;

        // Update the patrol logic
        Patrol();
    }

    private void Patrol()
    {
        // Set the target patrol point
        targetPoint = patrolPoints[currentPatrolIndex];

        // Set the destination for the agent to move towards
        agent.SetDestination(targetPoint.position);

        // Debug log for checking patrol movement
        Debug.Log("Enemy is patrolling towards: " + targetPoint.position);

        // Rotate towards the patrol point (enemy should face the patrol point)
        FaceTarget(targetPoint.position);

        // If the enemy reaches the patrol point, change to the next one
        if (Vector3.Distance(transform.position, targetPoint.position) < 1f)
        {
            // Debug log for reaching the patrol point
            Debug.Log("Reached patrol point: " + targetPoint.position);

            // Move to the next patrol point
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        }
    }

    // Function to make the enemy face the patrol point
    private void FaceTarget(Vector3 targetPosition)
    {
        // Calculate the direction towards the target position
        Vector3 direction = (targetPosition - transform.position).normalized;

        // Calculate the rotation we want to apply
        Quaternion lookRotation = Quaternion.LookRotation(direction);

        // Smoothly rotate the enemy towards the target position
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    // Expose patrol points to other scripts if necessary
    public void SetPatrolPoints(Transform[] newPatrolPoints)
    {
        patrolPoints = newPatrolPoints;
    }
}
