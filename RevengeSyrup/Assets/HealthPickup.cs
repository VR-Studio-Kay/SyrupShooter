using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class HealthPickupVR : MonoBehaviour
{
    [Header("Healing Settings")]
    public int healAmount = 25;
    public AudioClip pickupSound;

    private XRGrabInteractable grabInteractable;

    void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();

        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(OnSelectEntered);
        }
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        // Try to get the PlayerHealth component from the interactor's root (the hand grabbing it)
        var interactor = args.interactorObject.transform;
        PlayerHealth playerHealth = interactor.GetComponentInParent<PlayerHealth>();

        if (playerHealth != null && playerHealth.CurrentHealth < 100)
        {
            playerHealth.Heal(healAmount);

            // Play sound at pickup location
            if (pickupSound != null)
                AudioSource.PlayClipAtPoint(pickupSound, transform.position);

            Destroy(gameObject); // Destroy the health pickup
        }
        else
        {
            Debug.Log("[HealthPickupVR] No healing applied (player full health or not found).");
        }
    }

    private void OnDestroy()
    {
        // Clean up listener if destroyed
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnSelectEntered);
        }
    }
}
