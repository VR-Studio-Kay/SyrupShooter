using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(EnemyVisuals))]
public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int health = 100;
    [SerializeField] private EnemyHealthBar healthBar;

    [Header("Stagger Settings")]
    [SerializeField] private float staggerDurationMin = 0.4f; // random between min and max
    [SerializeField] private float staggerDurationMax = 0.7f;
    [SerializeField] private float staggerPushForce = 3f;

    [Header("Damage Feedback")]
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private ParticleSystem hitEffect;
    [SerializeField] private Animator animator; // Optional if you want animations
    [SerializeField] private string flinchTriggerName = "Flinch"; // Name of the trigger parameter in Animator

    private int maxHealth;
    private bool isDead = false;
    private bool isStaggered = false;
    private bool isInvincible = false;

    private EnemyVisuals visuals;
    private RagdollController ragdoll;
    private Collider[] colliders;
    private NavMeshAgent agent;
    private Rigidbody rb;
    private AudioSource audioSource;

    private void Start()
    {
        visuals = GetComponent<EnemyVisuals>();
        ragdoll = GetComponent<RagdollController>();
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();

        maxHealth = health;
        healthBar?.SetMaxHealth(maxHealth);

        colliders = GetComponentsInChildren<Collider>();
    }

    public void TakeDamage(int damage)
    {
        if (isDead || isInvincible) return;

        health -= damage;
        visuals.ChangeColor(Color.yellow);
        healthBar?.SetHealth(health);

        PlayHitFeedback();
        Stagger();

        if (health <= 0)
        {
            Die();
        }
    }

    private void PlayHitFeedback()
    {
        // Sound
        if (hitSound != null && audioSource != null)
            audioSource.PlayOneShot(hitSound);

        // Particle
        if (hitEffect != null)
            Instantiate(hitEffect, transform.position + Vector3.up * 1f, Quaternion.identity);

        // Animation
        if (animator != null && !string.IsNullOrEmpty(flinchTriggerName))
            animator.SetTrigger(flinchTriggerName);
    }

    private void Stagger()
    {
        if (isStaggered || isDead) return;

        isStaggered = true;
        isInvincible = true;

        if (agent != null)
            agent.isStopped = true;

        if (rb != null)
        {
            Vector3 pushDirection = (-transform.forward + Vector3.up).normalized;
            rb.AddForce(pushDirection * staggerPushForce, ForceMode.Impulse);
        }

        // Random stagger time between min and max
        float staggerDuration = Random.Range(staggerDurationMin, staggerDurationMax);
        Invoke(nameof(ResumeMovement), staggerDuration);
    }

    private void ResumeMovement()
    {
        if (agent != null && !isDead)
            agent.isStopped = false;

        isStaggered = false;
        isInvincible = false;
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log($"{gameObject.name} died.");

        if (agent != null) agent.enabled = false;

        foreach (var col in colliders)
            col.enabled = false;

        if (ragdoll != null)
        {
            ragdoll.ToggleRagdoll(true);
            ragdoll.AddForce(-transform.forward + Vector3.up, 3f);
        }

        Destroy(gameObject, 3f);
    }
}
