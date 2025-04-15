using UnityEngine;

[DisallowMultipleComponent]
public class GunReloadByAngle : MonoBehaviour
{
    [Header("Reload Settings")]
    [SerializeField] private float reloadAngleThreshold = 70f;
    [SerializeField] private float reloadCooldown = 2f;
    [Tooltip("Optional: Assign this if the controller is a separate object (like an XR Controller)")]
    [SerializeField] private Transform controllerTransform;

    private float lastReloadTime;
    private GunControllerPlayer gunController;

    private void Start()
    {
        gunController = GetComponent<GunControllerPlayer>();
        if (!gunController)
        {
            Debug.LogWarning("[GunReloadByAngle] GunControllerPlayer component not found on this GameObject.");
        }

        if (controllerTransform == null)
        {
            controllerTransform = transform; // fallback to self if not assigned
        }
    }

    private void Update()
    {
        Debug.Log("[GunReloadByAngle] Update running."); // Confirm it's active
        CheckReloadByAngle();
    }

    private void CheckReloadByAngle()
    {
        if (gunController == null) return;

        float xRotation = controllerTransform.localEulerAngles.x;

        // Convert to -180 to +180 range
        if (xRotation > 180f)
            xRotation -= 360f;

        Debug.Log($"[Reload Debug] Controller X Rotation: {xRotation:F2}°");

        if (Mathf.Abs(xRotation) >= reloadAngleThreshold && gunController.CurrentAmmo < gunController.MaxAmmo)
        {
            if (Time.time - lastReloadTime > reloadCooldown)
            {
                Debug.Log("[GunReloadByAngle] Reload conditions met. Reloading...");
                lastReloadTime = Time.time;
                gunController.Reload();
            }
        }
    }
}
