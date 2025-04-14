using UnityEngine;

public class GunReloadByAngle : MonoBehaviour
{
    public float reloadAngleThreshold = 70f;
    public float reloadCooldown = 2f;

    private float lastReloadTime;
    private GunControllerPlayer gunController;

    private void Start()
    {
        gunController = GetComponent<GunControllerPlayer>();
    }

    private void Update()
    {
        CheckReloadByAngle();
    }

    private void CheckReloadByAngle()
    {
        float zRotation = transform.localEulerAngles.z;
        if (zRotation > 180f)
            zRotation -= 360f;

        if (Mathf.Abs(zRotation) >= reloadAngleThreshold && gunController.currentAmmo < gunController.maxAmmo)
        {
            if (Time.time - lastReloadTime > reloadCooldown)
            {
                lastReloadTime = Time.time;
                gunController.Reload();
            }
        }
    }
}
