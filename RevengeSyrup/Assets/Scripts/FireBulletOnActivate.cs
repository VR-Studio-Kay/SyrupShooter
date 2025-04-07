using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class FireBulletOnActivate : MonoBehaviour
{
    public GameObject bullet;
    public Transform spawnPoint;
    public float fireSpeed = 20f;

    void Start()
    {
        XRGrabInteractable grabbable = GetComponent<XRGrabInteractable>();
        if (grabbable == null)
        {
            Debug.LogError("XRGrabInteractable is missing on the gun object.");
            return;
        }

        grabbable.activated.AddListener(FireBullet);
        Debug.Log("FireBulletOnActivate initialized: Listening for trigger activation.");
    }

    public void FireBullet(ActivateEventArgs args)
    {
        Debug.Log("Trigger activated. Attempting to fire...");

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

        GameObject spawnedBullet = Instantiate(bullet, spawnPoint.position, spawnPoint.rotation);

        Rigidbody rb = spawnedBullet.GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Bullet prefab does not have a Rigidbody!");
            return;
        }

        rb.linearVelocity = spawnPoint.forward * fireSpeed;
        Debug.Log("Bullet fired with velocity: " + rb.linearVelocity);

        Destroy(spawnedBullet, 5f);
    }
}
