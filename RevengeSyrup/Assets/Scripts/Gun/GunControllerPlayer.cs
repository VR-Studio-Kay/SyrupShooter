using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class IntEvent : UnityEvent<int> { }

public class GunControllerPlayer : MonoBehaviour
{
    [Header("Ammo Settings")]
    [SerializeField] private int maxAmmo = 10;
    private int currentAmmo;

    [Header("Shooting Settings")]
    [SerializeField] private ParticleSystem muzzleFlashPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 0.5f;
    private float nextFireTime = 0f;

    [Header("Bullet Settings")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed = 20f;

    [Header("Reload Settings")]
    [SerializeField] private float reloadCooldown = 2f;
    private float lastReloadTime = 0f;

    [Header("UI Manager")]
    [SerializeField] private GunUIManager gunUIManager;

    public IntEvent OnAmmoChanged = new IntEvent();

    public int CurrentAmmo => currentAmmo;
    public int MaxAmmo => maxAmmo;

    private void Start()
    {
        currentAmmo = maxAmmo;
        OnAmmoChanged?.Invoke(currentAmmo);
    }

    public void TryFire()
    {
        if (Time.time < nextFireTime)
            return;

        if (currentAmmo <= 0)
        {
            gunUIManager?.ShowOutOfAmmo();
            return;
        }

        Fire();
    }
    private void Fire()
    {
        currentAmmo--;
        OnAmmoChanged?.Invoke(currentAmmo);
        nextFireTime = Time.time + fireRate;

        if (muzzleFlashPrefab && firePoint)
        {
            Instantiate(muzzleFlashPrefab, firePoint.position, firePoint.rotation);
        }

        if (bulletPrefab && firePoint)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = firePoint.forward * bulletSpeed;
            }

            Destroy(bullet, 5f); // Auto-destroy the bullet after 5 seconds
        }
    }

    public void Reload()
    {
        if (Time.time - lastReloadTime < reloadCooldown || currentAmmo == maxAmmo)
            return;

        currentAmmo = maxAmmo;
        lastReloadTime = Time.time;
        OnAmmoChanged?.Invoke(currentAmmo);
    }
}
