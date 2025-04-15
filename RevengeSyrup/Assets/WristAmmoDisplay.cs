using UnityEngine;
using TMPro;

public class WristAmmoDisplay : MonoBehaviour
{
    [Header("Gun Reference")]
    [SerializeField] private GunControllerPlayer gunController;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI ammoText;

    private void Start()
    {
        if (!gunController)
        {
            Debug.LogWarning("[WristAmmoDisplay] No GunControllerPlayer reference set!");
            return;
        }

        gunController.OnAmmoChanged.AddListener(UpdateAmmoUI);
        UpdateAmmoUI(gunController.CurrentAmmo);
    }

    private void UpdateAmmoUI(int ammoCount)
    {
        if (ammoText != null)
        {
            ammoText.text = $"Ammo: {ammoCount}";
        }
    }

    private void OnDestroy()
    {
        if (gunController)
        {
            gunController.OnAmmoChanged.RemoveListener(UpdateAmmoUI);
        }
    }
}
