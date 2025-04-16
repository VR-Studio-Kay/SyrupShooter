using UnityEngine;
using TMPro;

public class WristAmmoDisplay : MonoBehaviour
{
    [Header("Gun References")]
    [SerializeField] private GunControllerPlayer leftGunController;
    [SerializeField] private GunControllerPlayer rightGunController;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI leftAmmoText;
    [SerializeField] private TextMeshProUGUI rightAmmoText;
    [SerializeField] private TextMeshProUGUI playerHealthText;  // Added for health display

    [Header("Player Health Reference")]
    [SerializeField] private PlayerHealth playerHealth;  // Added PlayerHealth reference

    private void Start()
    {
        if (leftGunController == null || rightGunController == null || playerHealth == null)
        {
            Debug.LogWarning("[WristAmmoDisplay] GunControllerPlayer or PlayerHealth references not assigned!");
            return;
        }

        // Listen for ammo changes in both guns
        leftGunController.OnAmmoChanged.AddListener(UpdateLeftAmmoUI);
        rightGunController.OnAmmoChanged.AddListener(UpdateRightAmmoUI);

        // Listen for health changes
        playerHealth.onHealthChanged.AddListener(UpdateHealthUI);

        // Initial update of ammo UI
        UpdateLeftAmmoUI(leftGunController.CurrentAmmo);
        UpdateRightAmmoUI(rightGunController.CurrentAmmo);
        UpdateHealthUI(playerHealth.CurrentHealth);
    }

    // Update the left ammo UI text
    private void UpdateLeftAmmoUI(int ammoCount)
    {
        if (leftAmmoText != null)
        {
            leftAmmoText.text = $"Left Ammo: {ammoCount}";
        }
    }

    // Update the right ammo UI text
    private void UpdateRightAmmoUI(int ammoCount)
    {
        if (rightAmmoText != null)
        {
            rightAmmoText.text = $"Right Ammo: {ammoCount}";
        }
    }

    // Update the player's health UI text
    private void UpdateHealthUI(int health)
    {
        if (playerHealthText != null)
        {
            playerHealthText.text = $"Health: {health}";
        }
    }

    private void OnDestroy()
    {
        if (leftGunController != null)
        {
            leftGunController.OnAmmoChanged.RemoveListener(UpdateLeftAmmoUI);
        }
        if (rightGunController != null)
        {
            rightGunController.OnAmmoChanged.RemoveListener(UpdateRightAmmoUI);
        }
        if (playerHealth != null)
        {
            playerHealth.onHealthChanged.RemoveListener(UpdateHealthUI);
        }
    }
}
