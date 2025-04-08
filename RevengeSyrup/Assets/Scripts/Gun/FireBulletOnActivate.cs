using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class FireBulletOnActivate : MonoBehaviour
{
    [Header("Bullet Settings")]
    public GameObject bullet;
    public Transform spawnPoint;
    public float fireSpeed = 20f;

    [Header("Input Settings")]
    public InputActionProperty triggerAction;  // Reference to the input action for the trigger

    private void OnEnable()
    {
        // Ensure the action is enabled when the object is active
        triggerAction.action.Enable();
    }

    private void OnDisable()
    {
        // Disable the action when the object is disabled
        triggerAction.action.Disable();
    }

    void Update()
    {
        // Check if the trigger button is pressed
        if (triggerAction.action.ReadValue<float>() > 0.1f) // Check if the trigger is pressed (adjust threshold if needed)
        {
            Debug.Log("Trigger pressed, firing bullet.");
            FireBullet();
        }
    }

    private void FireBullet()
    {
        // Check if the bullet prefab and spawn point are assigned
        if (bullet == null)
        {
            Debug.LogError("Bullet prefab is not assigned.");
            return;
        }

        if (spawnPoint == null)
        {
            Debug.LogError("Spawn point is not assigned.");
            return;
        }

        // Instantiate the bullet at the spawn point's position and rotation
        GameObject spawnedBullet = Instantiate(bullet, spawnPoint.position, spawnPoint.rotation);

        // Get the Rigidbody component and apply velocity to the bullet
        Rigidbody rb = spawnedBullet.GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Bullet prefab does not have a Rigidbody!");
            return;
        }

        rb.linearVelocity = spawnPoint.forward * fireSpeed;
        Debug.Log("Bullet fired with velocity: " + rb.linearVelocity);

        // Destroy the bullet after 5 seconds
        Destroy(spawnedBullet, 5f);
    }
}
