using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class GunControllerPlayer : MonoBehaviour
{
    [Header("Gun Settings")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletForce = 20f;
    public float shootCooldown = 0.2f;

    [Header("Ammo Settings")]
    public int maxAmmo = 10;
    [HideInInspector] public int currentAmmo;
    public UnityEvent<int> onAmmoChanged;

    [HideInInspector] public bool canShoot = true;

    private GunAudioManager audioManager;

    private void Start()
    {
        currentAmmo = maxAmmo;
        onAmmoChanged?.Invoke(currentAmmo);
        audioManager = GetComponent<GunAudioManager>();
    }

    public void TryFire()
    {
        if (!canShoot || currentAmmo <= 0)
        {
            audioManager?.PlayOutOfAmmo();
            return;
        }

        Fire();
    }

    private void Fire()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.linearVelocity = firePoint.forward * bulletForce;

        currentAmmo--;
        onAmmoChanged?.Invoke(currentAmmo);
        StartCoroutine(ShootCooldown());
        audioManager?.PlayShoot();
    }

    private IEnumerator ShootCooldown()
    {
        canShoot = false;
        yield return new WaitForSeconds(shootCooldown);
        canShoot = true;
    }

    public void Reload()
    {
        currentAmmo = maxAmmo;
        onAmmoChanged?.Invoke(currentAmmo);
        audioManager?.PlayReload();
    }

    public int GetCurrentAmmo() => currentAmmo;
}
