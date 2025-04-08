using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class Bullet : MonoBehaviour
{
    [Header("Stats")]
    public int damage = 40;
    public float lifetime = 5f;
    public bool destroyOnImpact = true;

    [Header("Movement")]
    public float speed = 20f;

    [Header("Target Filtering")]
    [Tooltip("Set to the layer(s) representing enemies.")]
    public LayerMask WhatIsEnemy;

    [Header("Effects")]
    public GameObject impactEffect;
    public AudioClip hitSound;

    private AudioSource audioSource;
    private bool hasImpacted = false;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // Only auto-destroy if not destroyOnImpact
        if (!destroyOnImpact)
            Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        // Move the bullet forward
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasImpacted) return;

        // Check if the collided object is in the enemy layer
        if (((1 << other.gameObject.layer) & WhatIsEnemy) == 0)
            return;

        // Apply damage if the target has an EnemyHealth component
        EnemyHealth enemy = other.GetComponent<EnemyHealth>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);

            // Optional: visual feedback
            EnemyVisuals visuals = enemy.GetComponent<EnemyVisuals>();
            if (visuals != null)
                visuals.ChangeColor(Color.yellow);
        }

        // Create impact visual effect
        if (impactEffect != null)
        {
            GameObject effect = Instantiate(impactEffect, transform.position, Quaternion.LookRotation(-transform.forward));
            Destroy(effect, 2f); // Clean up the effect after time
        }

        // Play impact sound with slight pitch variation
        if (hitSound != null && audioSource != null)
        {
            audioSource.pitch = Random.Range(0.95f, 1.05f);
            audioSource.PlayOneShot(hitSound);
        }

        if (destroyOnImpact)
        {
            hasImpacted = true;
            StartCoroutine(DestroyAfterSound());
        }
    }

    private IEnumerator DestroyAfterSound()
    {
        if (audioSource != null && hitSound != null)
            yield return new WaitForSeconds(hitSound.length);
        Destroy(gameObject);
    }
}
