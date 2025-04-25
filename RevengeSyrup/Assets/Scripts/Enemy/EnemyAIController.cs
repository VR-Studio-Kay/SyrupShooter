using UnityEngine;
using UnityEngine.AI;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections.Generic;

public class EnemyAIController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnemyPatrol patrolScript;
    [SerializeField] private EnemyCombat combatScript;
    [SerializeField] private Transform player;
    [SerializeField] private Transform vrCamera; // VR camera reference

    [Header("Settings")]
    [SerializeField] private float detectionRange = 20f;
    [SerializeField] private float attackRange = 10f;
    [SerializeField] private float strafeSpeed = 3f;
    [SerializeField] private float strafeChangeInterval = 2f;
    [SerializeField] private float keepDistance = 8f;
    [SerializeField] private float rotationSpeed = 5f; // Adjusted for VR, smoother rotation

    private NavMeshAgent agent;
    private bool isPlayerDetected;
    private float strafeTimer;
    private Vector3 strafeDirection;
    private bool isAttacking;

    private List<InputDevice> devices = new List<InputDevice>(); // To handle VR haptics

    public event System.Action OnEnemyKilled;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (vrCamera == null)
            vrCamera = Camera.main?.transform; // Default to main camera if no VR camera specified

        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Controller, devices);
    }

    void Update()
    {
        if (player == null || vrCamera == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange && HasLineOfSight())
        {
            if (!isPlayerDetected)
            {
                isPlayerDetected = true;
                if (patrolScript != null)
                {
                    patrolScript.StopPatrol();
                    Debug.Log("Patrol stopped - Player detected");
                }
            }

            if (distanceToPlayer > keepDistance)
            {
                // Chasing the player
                isAttacking = false;
                agent.isStopped = false;
                agent.SetDestination(player.position);

                // Continuously face the player while chasing
                FacePlayer();
            }
            else
            {
                // Strafe when within attack range
                HandleStrafeMovement();
            }

            if (distanceToPlayer <= attackRange)
            {
                isAttacking = true;
                combatScript.Attack(player);

                // VR Haptic feedback when attacking
                SendHaptics(0.7f, 0.3f);
            }
        }
        else
        {
            if (isPlayerDetected)
            {
                isPlayerDetected = false;
                if (patrolScript != null)
                {
                    patrolScript.ResumePatrol();
                    Debug.Log("Player lost - Resuming patrol");
                }
            }
        }
    }

    private bool HasLineOfSight()
    {
        Vector3 dirToPlayer = player.position - transform.position;
        Ray ray = new Ray(transform.position + Vector3.up, dirToPlayer);
        if (Physics.Raycast(ray, out RaycastHit hit, detectionRange))
        {
            return hit.collider.CompareTag("Player");
        }
        return false;
    }

    private void FacePlayer()
    {
        // Smooth rotation, more suitable for VR
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0f;  // Keep the y-axis rotation fixed

        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

    private void HandleStrafeMovement()
    {
        agent.isStopped = false;

        strafeTimer -= Time.deltaTime;
        if (strafeTimer <= 0f)
        {
            Vector3 right = transform.right;
            strafeDirection = (Random.value > 0.5f ? right : -right) + transform.forward * 0.2f;
            strafeDirection.Normalize();
            strafeTimer = strafeChangeInterval;
        }

        Vector3 targetPos = transform.position + strafeDirection * strafeSpeed;
        if (NavMesh.SamplePosition(targetPos, out NavMeshHit hit, 1f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    private void SendHaptics(float intensity, float duration)
    {
        // Apply haptic feedback to the VR controllers
        foreach (var device in devices)
        {
            if (device.TryGetHapticCapabilities(out HapticCapabilities capabilities) && capabilities.supportsImpulse)
            {
                device.SendHapticImpulse(0u, intensity, duration);
            }
        }
    }

    public void NotifyDeath()
    {
        OnEnemyKilled?.Invoke();
    }
}
