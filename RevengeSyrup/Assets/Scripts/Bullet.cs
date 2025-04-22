using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class Bullet : MonoBehaviour
{
    [Header("Stats")]
    public int baseDamage = 40;
    public float lifetime = 5f;
    public bool destroyOnImpact = true;

    [Header("Movement")]
    public float speed = 20f;
    public bool usePhysics = false;
    private Rigidbody rb;

    [Header("Target Filtering")]
    [Tooltip("Set to the layer(s) representing enemies.")]
    public LayerMask WhatIsEnemy;

    [Header("Effects")]
    public ParticleSystem impactEffect; // Optional: Impact effect (like a prefab or particle system)
    public AudioClip hitSound;

    [Header("Impact Force")]
    public float knockbackForce = 10f;
    public ForceMode forceMode = ForceMode.Impulse;

    [Header("Advanced Options")]
    public bool useObjectPooling = false;
    public GameObject bulletPoolReturnTarget; // Set this if using pooling

    private AudioSource audioSource;
    private bool hasImpacted = false;

    // Public ParticleSystem field for Inspector assignment
    public ParticleSystem impactParticleSystem; // Particle system for impact effect

    public GameObject intro;
    //Script script = (Script) intro.GetComponent(typeof(Script));

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (usePhysics)
        {
            rb = GetComponent<Rigidbody>();
        }
    }

    private void OnEnable()
    {
        hasImpacted = false;

        if (usePhysics && rb != null)
        {
            rb.linearVelocity = transform.forward * speed;
        }

        // Self-destroy or return to pool after lifetime
        if (!destroyOnImpact)
            StartCoroutine(DelayedDespawn(lifetime));
    }

    private void Update()
    {
        if (!usePhysics && !hasImpacted)
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasImpacted) return;

        // Check if collided object is in the enemy layer
        if (((1 << other.gameObject.layer) & WhatIsEnemy) == 0)
            return;

        hasImpacted = true;

        // Check if collided object is the intro trigger
        if(other.CompareTag("introTrigger")){
             //intro.trigger();
             intro.GetComponent<intro_jeu>().allo();
        }

        // Damage logic
        int finalDamage = baseDamage;

        if (other.CompareTag("CriticalHitZone")) // Optional critical zone (like a head)
        {
            finalDamage = Mathf.RoundToInt(baseDamage * 1.5f); // 50% bonus damage
        }

        if (other.TryGetComponent(out EnemyHealth enemy))
        {
            enemy.TakeDamage(finalDamage);

            if (enemy.TryGetComponent(out EnemyVisuals visuals))
                visuals.ChangeColor(Color.red);
        }

        // Apply knockback force if Rigidbody found
        if (other.attachedRigidbody != null)
        {
            Vector3 direction = (other.transform.position - transform.position).normalized;
            other.attachedRigidbody.AddForce(direction * knockbackForce, forceMode);
        }

        // Impact effect (particle system)
        if (impactParticleSystem != null)
        {
            // Play the particle system at the bullet's impact position
            Instantiate(impactParticleSystem, transform.position, Quaternion.LookRotation(-transform.forward));
        }

        // Impact effect (alternative, if using impactEffect GameObject)
        if (impactEffect != null)
        {
            Instantiate(impactEffect, transform.position, Quaternion.LookRotation(-transform.forward));
        }

        // Sound
        if (hitSound != null && audioSource != null)
        {
            audioSource.pitch = Random.Range(0.95f, 1.05f);
            audioSource.PlayOneShot(hitSound);
        }

        // Destroy or pool return
        if (destroyOnImpact)
            StartCoroutine(DestroyAfterSound());
    }

    private IEnumerator DestroyAfterSound()
    {
        if (audioSource != null && hitSound != null)
            yield return new WaitForSeconds(hitSound.length);

        Despawn();
    }

    private IEnumerator DelayedDespawn(float time)
    {
        yield return new WaitForSeconds(time);
        Despawn();
    }

    private void Despawn()
    {
        if (useObjectPooling && bulletPoolReturnTarget != null)
        {
            gameObject.SetActive(false); // Or use your pooling manager
            transform.SetParent(bulletPoolReturnTarget.transform);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
