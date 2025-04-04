using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Stats")]
    public int damage = 40;
    public float lifetime = 5f;
    public bool destroyOnImpact = true;

    [Header("Target Filtering")]
    public LayerMask WhatIsEnemy; // <-- updated name

    [Header("Effects")]
    public GameObject impactEffect; // Optional particle/sound effect prefab
    public AudioClip hitSound;


    private AudioSource audioSource;

    private void Start()
    {
        Destroy(gameObject, lifetime); // Auto-cleanup
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if bullet hit something in the valid layer
        if (((1 << other.gameObject.layer) & WhatIsEnemy) == 0)
            return;

        // Enemy hit?
        EnemyAiTutorial enemy = other.GetComponent<EnemyAiTutorial>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            enemy.ChangeColor(Color.yellow); // Flash color for feedback
        }

        // Optional impact effect
        if (impactEffect != null)
        {
            Instantiate(impactEffect, transform.position, Quaternion.identity);
        }

        // Optional hit sound
        if (hitSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(hitSound);
        }

        if (destroyOnImpact)
        {
            Destroy(gameObject);
        }
    }
}
