using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class FireBulletOnActivate : MonoBehaviour
{
    public GameObject bullet;           // Bullet prefab to be instantiated
    public Transform spawnPoint;        // Muzzle point or where the bullet spawns
    public float fireSpeed = 20f;       // Speed of the bullet

    private XRGrabInteractable grabbable;

    void Start()
    {
        // Get the XRGrabInteractable component attached to this object
        grabbable = GetComponent<XRGrabInteractable>();

        // Add listener to trigger fire event on activation (button press)
        grabbable.activated.AddListener(FireBullet);

        Debug.Log("FireBulletOnActivate script initialized. Waiting for activation...");
    }

    // Called when the object is activated (button press or interaction)
    public void FireBullet(ActivateEventArgs arg)
    {
        // Log when the bullet is fired
        Debug.Log("FireBullet activated! Trigger pressed.");

        // Ensure we have the bullet prefab and spawn point
        if (bullet != null && spawnPoint != null)
        {
            Debug.Log("Bullet and spawn point are correctly assigned.");

            // Instantiate the bullet at the spawn point's position and rotation
            GameObject spawnedBullet = Instantiate(bullet, spawnPoint.position, spawnPoint.rotation);

            // Apply forward velocity to the bullet (firing it)
            Rigidbody rb = spawnedBullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = spawnPoint.forward * fireSpeed; // Apply speed in the forward direction
                Debug.Log("Bullet fired with velocity: " + rb.linearVelocity);
            }
            else
            {
                Debug.LogError("No Rigidbody attached to the bullet prefab!");
            }

            // Destroy the bullet after 5 seconds (can be adjusted based on your needs)
            Destroy(spawnedBullet, 5f);
            Debug.Log("Bullet will be destroyed in 5 seconds.");
        }
        else
        {
            Debug.LogError("Bullet or spawn point is not assigned! Please check the Inspector.");
        }
    }

    // You can optionally use Update if you want to track other states or perform other actions
    void Update()
    {
        // Example: Debugging if the gun is grabbed or activated
        if (grabbable.isSelected)
        {
            Debug.Log("Gun is currently being held by the user.");
        }
    }
}
