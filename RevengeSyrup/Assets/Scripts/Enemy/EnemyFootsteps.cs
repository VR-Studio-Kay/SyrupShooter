using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyFootsteps : MonoBehaviour
{
    [Header("Footstep Settings")]
    [SerializeField] private AudioSource footstepSource; // AudioSource for footsteps
    [SerializeField] private float normalStepInterval = 0.5f; // Time between steps while patrolling
    [SerializeField] private float chaseStepInterval = 0.25f; // Time between steps while chasing

    private NavMeshAgent agent;
    private bool isChasing = false;
    private float stepTimer = 0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (footstepSource == null)
        {
            Debug.LogWarning("[EnemyFootsteps] No footstep AudioSource assigned!");
        }
        else
        {
            footstepSource.spatialBlend = 1f; // Make sure it's 3D sound
            footstepSource.loop = false; // We manually trigger steps
        }
    }

    void Update()
    {
        if (agent.velocity.magnitude > 0.1f) // Enemy is moving
        {
            stepTimer -= Time.deltaTime;
            if (stepTimer <= 0f)
            {
                PlayFootstep();
                stepTimer = isChasing ? chaseStepInterval : normalStepInterval;
            }
        }
        else
        {
            stepTimer = 0f; // Reset when not moving
        }
    }

    private void PlayFootstep()
    {
        if (footstepSource != null && footstepSource.clip != null)
        {
            footstepSource.pitch = Random.Range(0.9f, 1.1f); // Optional: small pitch variation
            footstepSource.PlayOneShot(footstepSource.clip);
            Debug.Log("[EnemyFootsteps] Footstep played.");
        }
    }

    public void OnPlayerDetected()
    {
        isChasing = true;
        Debug.Log("[EnemyFootsteps] Now chasing player, footsteps faster!");
    }

    public void OnPlayerLost()
    {
        isChasing = false;
        Debug.Log("[EnemyFootsteps] Lost player, footsteps slower.");
    }
}
