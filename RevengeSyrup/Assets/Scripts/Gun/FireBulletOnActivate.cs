using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
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
    public float reloadAngleThreshold = 70f;
    public float reloadCooldown = 2f;
    private float lastReloadTime;

    [Header("Audio")]
    public AudioSource shootSound;
    public AudioSource reloadSound;
    public AudioClip outOfAmmoClip;
    [Range(0.8f, 1.2f)] public float pitchVariationMin = 0.95f;
    [Range(0.8f, 1.2f)] public float pitchVariationMax = 1.05f;

    [Header("Events")]
    public UnityEvent<int> onAmmoChanged;

    [Header("Input Actions")]
    public InputActionProperty activateAction; // Reference to the Activate Action

    private void OnEnable()
    {
        if (activateAction != null)
        {
            activateAction.action.performed += OnActivatePerformed;
            activateAction.action.Enable();
        }
    }

    private void OnDisable()
    {
        if (activateAction != null)
        {
            activateAction.action.performed -= OnActivatePerformed;
            activateAction.action.Disable();
        }
    }

    private void Start()
    {
        currentAmmo = maxAmmo;
        onAmmoChanged?.Invoke(currentAmmo);
    }

    private void OnActivatePerformed(InputAction.CallbackContext context)
    {
        Fire();
    }

    public void Fire()
    {
        if (!canShoot || currentAmmo <= 0)
        {
            Debug.Log("[FireBulletOnActivate] Cannot shoot: either on cooldown or out of ammo.");

            // Play out-of-ammo sound if available
            if (outOfAmmoClip && shootSound)
            {
                shootSound.PlayOneShot(outOfAmmoClip);
            }

            return;
        }

        Shoot();
        currentAmmo--;
        onAmmoChanged?.Invoke(currentAmmo);
        Debug.Log($"[FireBulletOnActivate] Shot fired. Ammo left: {currentAmmo}");
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
        Debug.Log("[FireBulletOnActivate] Bullet fired.");
    }

    private IEnumerator ShootCooldown()
    {
        canShoot = false;
        yield return new WaitForSeconds(shootCooldown);
        canShoot = true;
        Debug.Log("[FireBulletOnActivate] Shoot cooldown complete.");
    }

    private void CheckReloadByAngle()
    {
        float zRotation = transform.localEulerAngles.z;
        if (zRotation > 180f)
        {
            zRotation -= 360f;
        }

        Debug.Log($"[ReloadCheck] Local Z-axis rotation: {zRotation}°");

        if (Mathf.Abs(zRotation) >= reloadAngleThreshold)
        {
            Debug.Log("[ReloadCheck] Gun rolled beyond threshold on Z-axis.");

            if (currentAmmo < maxAmmo)
            {
                Debug.Log($"[ReloadCheck] Ammo is not full. Current ammo: {currentAmmo}/{maxAmmo}");

                if (Time.time - lastReloadTime > reloadCooldown)
                {
                    Debug.Log("[ReloadCheck] Cooldown passed. Reloading...");
                    Reload();
                }
                else
                {
                    Debug.Log($"[ReloadCheck] Cooldown not finished. Time left: {reloadCooldown - (Time.time - lastReloadTime):F2}s");
                }
            }
            else
            {
                Debug.Log("[ReloadCheck] Ammo is already full. No need to reload.");
            }
        }
        else
        {
            Debug.Log("[ReloadCheck] Gun has not rolled enough on Z-axis to trigger reload.");
        }
    }

    private void Reload()
    {
        currentAmmo = maxAmmo;
        lastReloadTime = Time.time;
        onAmmoChanged?.Invoke(currentAmmo);

        Debug.Log($"[Reload] Gun reloaded! Ammo restored to {maxAmmo}.");

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
