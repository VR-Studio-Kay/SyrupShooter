using UnityEngine;

public class FireBulletOnActivate : MonoBehaviour
{
    public GameObject bullet;  // Bullet prefab to instantiate
    public Transform spawnPoint;  // Where the bullet will be spawned from
    public float fireSpeed = 20f;  // Speed at which the bullet will move

    void Update()
    {
        // Debug log to check if the trigger is being pressed
        if (Input.GetButtonDown("Fire1"))
        {
            Debug.Log("Trigger button pressed - firing bullet...");
            FireBullet();
        }
        else
        {
            // Optional: Log if the trigger is not being pressed
            Debug.Log("Trigger not pressed.");
        }
    }

    public void FireBullet()
    {
        Debug.Log("Fire1 button pressed. Attempting to fire...");

        if (bullet == null)
        {
            Debug.LogError("Bullet prefab is not assigned.");
            return;
        }

        if (spawnPoint == null)
        {
            Debug.LogError("SpawnPoint is not assigned.");
            return;
        }

        // Instantiate the bullet at the spawn point
        GameObject spawnedBullet = Instantiate(bullet, spawnPoint.position, spawnPoint.rotation);
        Debug.Log("Bullet spawned at position: " + spawnPoint.position);

        Rigidbody rb = spawnedBullet.GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Bullet prefab does not have a Rigidbody!");
            return;
        }

        // Set the linear velocity of the bullet in the direction it is facing
        rb.linearVelocity = spawnPoint.forward * fireSpeed;
        Debug.Log("Bullet fired with linearVelocity: " + rb.linearVelocity);

        // Destroy the bullet after 5 seconds
        Destroy(spawnedBullet, 5f);
    }
}
