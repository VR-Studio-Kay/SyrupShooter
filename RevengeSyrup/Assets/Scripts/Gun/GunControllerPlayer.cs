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
    [SerializeField] private GameObject muzzleFlashPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 0.5f;
    private float nextFireTime = 0f;

    [Header("Reload Settings")]
    [SerializeField] private float reloadCooldown = 2f;
    private float lastReloadTime = 0f;

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
        if (Time.time < nextFireTime || currentAmmo <= 0)
            return;

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

        // Additional shooting logic (e.g., raycasting, sound effects) can be added here.
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
