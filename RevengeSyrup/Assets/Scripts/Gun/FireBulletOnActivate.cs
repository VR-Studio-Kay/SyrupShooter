using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class FireBulletOnActivate : MonoBehaviour
{
    [Header("Gun Settings")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletForce = 20f;
    public float shootCooldown = 0.2f;

    [Header("Ammo Settings")]
    public int maxAmmo = 10;
    private int currentAmmo;
    private bool canShoot = true;

    [Header("Reload Settings")]
    public float reloadAngleThreshold = 60f;
    public float reloadCooldown = 2f;
    private float lastReloadTime;

    [Header("Audio")]
    public AudioSource shootSound;
    public AudioSource reloadSound;
    [Range(0.8f, 1.2f)] public float pitchVariationMin = 0.95f;
    [Range(0.8f, 1.2f)] public float pitchVariationMax = 1.05f;

    [Header("Events")]
    public UnityEvent<int> onAmmoChanged;

    private void Start()
    {
        currentAmmo = maxAmmo;
        onAmmoChanged?.Invoke(currentAmmo);
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetButtonDown("Fire1")) // For keyboard testing
        {
            Fire();
        }
#endif
        CheckReloadByAngle();
    }

    public void Fire() // Call from XR interaction
    {
        if (!canShoot || currentAmmo <= 0) return;

        Shoot();
        currentAmmo--;
        onAmmoChanged?.Invoke(currentAmmo);
        Debug.Log($"[Gun] Shot fired. Ammo left: {currentAmmo}");
        StartCoroutine(ShootCooldown());

        if (shootSound)
        {
            shootSound.pitch = Random.Range(pitchVariationMin, pitchVariationMax);
            shootSound.Play();
        }
    }

    private void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.linearVelocity = firePoint.forward * bulletForce;
    }

    private IEnumerator ShootCooldown()
    {
        canShoot = false;
        yield return new WaitForSeconds(shootCooldown);
        canShoot = true;
    }

    private void CheckReloadByAngle()
    {
        float angle = Vector3.Angle(transform.forward, Vector3.down);
        if (angle < reloadAngleThreshold && currentAmmo < maxAmmo && Time.time - lastReloadTime > reloadCooldown)
        {
            Reload();
        }
    }

    private void Reload()
    {
        currentAmmo = maxAmmo;
        lastReloadTime = Time.time;
        onAmmoChanged?.Invoke(currentAmmo);
        Debug.Log("[Gun] Reloaded!");

        if (reloadSound)
        {
            reloadSound.pitch = Random.Range(pitchVariationMin, pitchVariationMax);
            reloadSound.Play();
        }
    }

    public int GetCurrentAmmo()
    {
        return currentAmmo;
    }
}
