using UnityEngine;

public class FireBulletOnActivate : MonoBehaviour
{
    [Header("Gun Settings")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletForce = 20f;
    public float reloadAngleThreshold = 60f; // Degrees below horizontal to reload

    [Header("Ammo Settings")]
    public int maxAmmo = 10;
    private int currentAmmo;

    private void Start()
    {
        currentAmmo = maxAmmo;
    }

    private void Update()
    {
        HandleInput();
        CheckReloadByAngle();
    }

    private void HandleInput()
    {
        // Replace this with XR trigger input if needed
        if (Input.GetButtonDown("Fire1"))
        {
            TryShoot();
        }
    }

    private void TryShoot()
    {
        if (currentAmmo <= 0)
        {
            Debug.Log("[Gun] Out of ammo!");
            return;
        }

        Shoot();
        currentAmmo--;
        Debug.Log($"[Gun] Shot fired. Ammo left: {currentAmmo}");
    }

    private void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.linearVelocity = firePoint.forward * bulletForce;
    }

    private void CheckReloadByAngle()
    {
        // Check if gun is pointing downward
        float angle = Vector3.Angle(transform.forward, Vector3.down);
        if (angle < reloadAngleThreshold && currentAmmo < maxAmmo)
        {
            Reload();
        }
    }

    private void Reload()
    {
        currentAmmo = maxAmmo;
        Debug.Log("[Gun] Reloaded!");
    }

    public int GetCurrentAmmo()
    {
        return currentAmmo;
    }
}
