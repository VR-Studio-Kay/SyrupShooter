using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class FireBulletOnTrigger : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletForce = 20f;

    private InputDevice rightController;

    void Start()
    {
        // Get the right-hand XR controller
        var rightHandedControllers = new List<InputDevice>();
        InputDevices.GetDevicesAtXRNode(XRNode.RightHand, rightHandedControllers);
        if (rightHandedControllers.Count > 0)
        {
            rightController = rightHandedControllers[0];
            Debug.Log("Right hand controller found!");
        }
        else
        {
            Debug.LogWarning("No right hand controller found!");
        }
    }

    void Update()
    {
        if (rightController != null)
        {
            if (rightController.TryGetFeatureValue(CommonUsages.triggerButton, out bool triggerPressed) && triggerPressed)
            {
                Fire();
            }
        }
    }

    void Fire()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.linearVelocity = firePoint.forward * bulletForce;
        Debug.Log("Gun fired!");
    }
}
