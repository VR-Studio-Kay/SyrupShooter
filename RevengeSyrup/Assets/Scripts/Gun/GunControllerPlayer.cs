using UnityEngine;
using UnityEngine.Events;
using System.Collections;

[RequireComponent(typeof(GunAudioManager))]
[DisallowMultipleComponent]
public class GunControllerPlayer : MonoBehaviour
{
    [Header("Gun Settings")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float bulletForce = 20f;
    [SerializeField] private float shootCooldown = 0.2f;

    [Header("Muzzle Flash")]
    [Tooltip("Optional muzzle flash effect played on shooting.")]
    [SerializeField] private GameObject muzzleFlashPrefab;
    [SerializeField] private Transform muzzleFlashPoint;
    [SerializeField] private float muzzleFlashDuration = 0.05f;

    [Header("Ammo Settings")]
    [SerializeField] private int maxAmmo = 10;
    [SerializeField] private UnityEvent<int> onAmmoChanged;

    public int CurrentAmmo { get; private set; }
    public int MaxAmmo => maxAmmo;
    public bool CanShoot { get; private set; } = true;

    private GunAudioManager audioManager;
    private GunUIManager uiManager;

    private void Awake()
    {
        audioManager = GetComponent<GunAudioManager>();
        uiManager = GetComponent<GunUIManager>();
    }

    private void Start()
    {
        CurrentAmmo = maxAmmo;
        onAmmoChanged?.Invoke(CurrentAmmo);
    }

    public void TryFire()
    {
        if (!CanShoot || CurrentAmmo <= 0)
        {
            audioManager?.PlayOutOfAmmo();
            uiManager?.ShowOutOfAmmo();
            return;
        }

        Fire();
    }

    private void Fire()
    {
        if (!bulletPrefab || !firePoint) return;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        if (bullet.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.linearVelocity = firePoint.forward * bulletForce;
        }

        CurrentAmmo--;
        onAmmoChanged?.Invoke(CurrentAmmo);
        StartCoroutine(ShootCooldown());
        audioManager?.PlayShoot();

        if (muzzleFlashPrefab && muzzleFlashPoint)
        {
            GameObject flash = Instantiate(muzzleFlashPrefab, muzzleFlashPoint.position, muzzleFlashPoint.rotation, muzzleFlashPoint);
            Destroy(flash, muzzleFlashDuration);
        }
    }

    private IEnumerator ShootCooldown()
    {
        CanShoot = false;
        yield return new WaitForSeconds(shootCooldown);
        CanShoot = true;
    }

    public void Reload()
    {
        CurrentAmmo = maxAmmo;
        onAmmoChanged?.Invoke(CurrentAmmo);
        audioManager?.PlayReload();
    }
}
