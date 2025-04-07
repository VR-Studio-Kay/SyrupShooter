using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public int damage = 20;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the projectile hits an enemy
        if (other.CompareTag("Player"))
        {
            // Example of dealing damage to the player (you can replace this with your player health code)
            Debug.Log("Player hit! Dealing damage.");
            // Call a method to deal damage to the player or trigger an effect
            Destroy(gameObject); // Destroy the projectile on impact
        }

        // If it hits something else, you might want to destroy or bounce off
        else
        {
            Destroy(gameObject); // Destroy projectile on collision
        }
    }
}
