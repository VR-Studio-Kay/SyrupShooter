using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class AutoAttachGun : MonoBehaviour
{
    private XRGrabInteractable grabInteractable;
    private XRDirectInteractor rightHandInteractor;

    void Start()
    {
        // Get the XR Grab Interactable component attached to the gun
        grabInteractable = GetComponent<XRGrabInteractable>();

        // Find the XR Direct Interactor for the right hand (adjust this if needed for the left hand)
        rightHandInteractor = GameObject.Find("Right Controller").GetComponent<XRDirectInteractor>();

        if (grabInteractable != null && rightHandInteractor != null)
        {
            // Automatically grab the gun using the selectEntered event
            grabInteractable.selectEntered.AddListener(OnGunGrabbed);

            // Simulate the grab by invoking the selectEntered event with the right hand interactor
            SelectEnterEventArgs args = new SelectEnterEventArgs();
            args.interactorObject = rightHandInteractor;  // Use the interactor directly, no need to reference GameObject

            grabInteractable.selectEntered.Invoke(args);  // Trigger the event

            Debug.Log("Gun grabbed automatically by the right hand.");
        }
        else
        {
            Debug.LogError("XRGrabInteractable or XRDirectInteractor is missing!");
        }
    }

    // Handle the gun grab logic
    private void OnGunGrabbed(SelectEnterEventArgs args)
    {
        // Handle logic for when the gun is grabbed by the hand, e.g., change the gun's position or rotation
        Debug.Log("Gun has been grabbed by the right hand.");
    }
}
