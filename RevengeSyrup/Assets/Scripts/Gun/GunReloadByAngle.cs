using UnityEngine;

[DisallowMultipleComponent]
public class GunReloadByAngle : MonoBehaviour
{
    [Header("Reload Settings")]
    [SerializeField] private float reloadAngleThreshold = 70f;
    [SerializeField] private float reloadCooldown = 2f;

    private float lastReloadTime;
    private GunControllerPlayer gunController;

    private void Start()
    {
        gunController = GetComponent<GunControllerPlayer>();
        if (!gunController)
        {
            Debug.LogWarning("GunControllerPlayer component not found on this GameObject.");
        }
    }

    private void Update()
    {
        CheckReloadByAngle();
    }

    private void CheckReloadByAngle()
    {
        if (gunController == null) return;

        // Debug the current rotation values of the controller
        Vector3 localRotation = transform.localEulerAngles;
        Debug.Log($"[Reload Debug] Rotation - X: {localRotation.x:F2}, Y: {localRotation.y:F2}, Z: {localRotation.z:F2}");

        float xRotation = localRotation.x;
        if (xRotation > 180f)
            xRotation -= 360f;

        if (Mathf.Abs(xRotation) >= reloadAngleThreshold && gunController.CurrentAmmo < gunController.MaxAmmo)
        {
            if (Time.time - lastReloadTime > reloadCooldown)
            {
                lastReloadTime = Time.time;
                gunController.Reload();
            }
        }
    }
}
