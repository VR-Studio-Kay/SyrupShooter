using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public int damage = 10;
    public float lifetime = 5f;
    public GameObject bloodEffectPrefab;
    public AudioClip hitSound;

    public LayerMask whatIsPlayer; // assign via Inspector

    private bool hasHit = false;
    private AudioSource audioSource;

    private void Start()
    {
        Destroy(gameObject, lifetime);
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasHit) return;
        hasHit = true;

        // Check if collided object is on the player layer
        if (((1 << other.gameObject.layer) & whatIsPlayer) != 0)
        {
            Debug.Log($"[EnemyBullet] Hit player-layer object: {other.name}");

            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                Debug.Log($"[EnemyBullet] Damage dealt: {damage}");

                if (bloodEffectPrefab != null)
                    Instantiate(bloodEffectPrefab, other.transform.position, Quaternion.identity);

                if (hitSound != null && audioSource != null)
                    audioSource.PlayOneShot(hitSound);
            }

            Destroy(gameObject, 0.05f);
        }
        else
        {
            Debug.Log($"[EnemyBullet] Hit non-player object: {other.name}");
        }
    }
}
