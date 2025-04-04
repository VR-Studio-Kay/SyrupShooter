using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Stats")]
    public int damage = 40;
    public float lifetime = 5f;
    public bool destroyOnImpact = true;

    [Header("Target Filtering")]
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

    private void OnTriggerEnter(Collider other)
    {
        if (hasImpacted) return; // Prevent multiple triggers
        if (((1 << other.gameObject.layer) & WhatIsEnemy) == 0)
            return;

        EnemyAiTutorial enemy = other.GetComponent<EnemyAiTutorial>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            enemy.ChangeColor(Color.yellow);
        }

        if (impactEffect != null)
            Instantiate(impactEffect, transform.position, Quaternion.identity);

        if (hitSound != null && audioSource != null)
            audioSource.PlayOneShot(hitSound);

        if (destroyOnImpact)
        {
            hasImpacted = true;
            Destroy(gameObject, 5f); // Delay destruction by 5 seconds
        }
    }
}
