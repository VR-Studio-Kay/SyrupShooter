using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public int damage = 10;
    public float lifetime = 5f;
    public GameObject bloodEffectPrefab; // Assign a blood screen UI or particle prefab
    public AudioClip hitSound;           // Optional: sound effect on hit

    private bool hasHit = false;         // Prevent double damage
    private AudioSource audioSource;

    private void Start()
    {
        Destroy(gameObject, lifetime); // Clean up after time
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Log the name and tag of the object the bullet collided with
        Debug.Log($"[EnemyBullet] Hit object: {other.gameObject.name}, Tag: {other.tag}");

        if (hasHit) return; // Prevent multiple collisions
        hasHit = true;

        // Check if the collided object is a child of the "Player" tagged object (XR Origin)
        if (other.transform.root.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponentInParent<PlayerHealth>();
            if (playerHealth != null && playerHealth.CurrentHealth > 0)
            {
                // Call TakeDamage method on the player and log the damage
                playerHealth.TakeDamage(damage);
                Debug.Log($"[EnemyBullet] Player hit! -{damage} HP | Remaining: {playerHealth.CurrentHealth}");

                // Optional: Blood screen or hit feedback
                if (bloodEffectPrefab != null)
                {
                    Instantiate(bloodEffectPrefab, other.transform.position, Quaternion.identity);
                }

                // Play hit sound if available
                if (hitSound != null && audioSource != null)
                {
                    audioSource.PlayOneShot(hitSound);
                }
            }
        }
        else
        {
            // Log if the bullet hit something that is not the player
            Debug.Log("[EnemyBullet] Hit non-player object.");
        }

        // Destroy bullet after a small delay (allow sound effect to play)
        Destroy(gameObject, 0.05f);
    }
}
