using UnityEngine;
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class AutoAttachGun : MonoBehaviour
{
    private XRGrabInteractable grabInteractable;
    private XRDirectInteractor rightHandInteractor;
    private XRDirectInteractor leftHandInteractor;

    void Start()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();

        GameObject rightControllerObj = GameObject.Find("Right Controller");
        GameObject leftControllerObj = GameObject.Find("Left Controller");

        if (rightControllerObj != null)
            rightHandInteractor = rightControllerObj.GetComponent<XRDirectInteractor>();
        if (leftControllerObj != null)
            leftHandInteractor = leftControllerObj.GetComponent<XRDirectInteractor>();

        if (grabInteractable != null && (rightHandInteractor != null || leftHandInteractor != null))
        {
            grabInteractable.selectEntered.AddListener(OnGunGrabbed);
            StartCoroutine(AttachGunNextFrame());
        }
        else
        {
            Debug.LogError("Missing XRGrabInteractable or both XRDirectInteractors.");
        }
    }

    private IEnumerator AttachGunNextFrame()
    {
        yield return null; // Wait one frame to allow interaction system to initialize

        XRDirectInteractor handToAttach = rightHandInteractor != null ? rightHandInteractor : leftHandInteractor;

        if (handToAttach == null)
        {
            Debug.LogWarning("No available hand to attach the gun.");
            yield break;
        }

        // Ensure the attach transform is set for proper alignment
        grabInteractable.attachTransform = handToAttach.attachTransform;

        // Force release if the hand is already grabbing something
        if (handToAttach.hasSelection)
        {
            handToAttach.interactionManager.SelectExit(handToAttach, handToAttach.firstInteractableSelected);
        }

        // Safely cast to interfaces
        IXRSelectInteractor interactor = handToAttach as IXRSelectInteractor;
        IXRSelectInteractable interactable = grabInteractable as IXRSelectInteractable;

        if (interactor != null && interactable != null && handToAttach.interactionManager != null)
        {
            handToAttach.interactionManager.SelectEnter(interactor, interactable);
            Debug.Log("Gun grabbed automatically by " + handToAttach.name);
        }
        else
        {
            Debug.LogError("Failed to cast interactor or interactable, or interactionManager is missing.");
        }
    }

    private void OnGunGrabbed(SelectEnterEventArgs args)
    {
        Debug.Log("Gun has been grabbed by " + args.interactorObject.transform.name);
    }
}
