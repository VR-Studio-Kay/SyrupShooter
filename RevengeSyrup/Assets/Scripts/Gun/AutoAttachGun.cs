using UnityEngine;
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
            // Attach event listener
            grabInteractable.selectEntered.AddListener(OnGunGrabbed);

            // Choose which hand to use
            XRDirectInteractor handToAttach = rightHandInteractor != null ? rightHandInteractor : leftHandInteractor;

            // Safely cast to interfaces and perform SelectEnter
            IXRSelectInteractor interactor = handToAttach as IXRSelectInteractor;
            IXRSelectInteractable interactable = grabInteractable as IXRSelectInteractable;

            if (interactor != null && interactable != null && handToAttach.interactionManager != null)
            {
                handToAttach.interactionManager.SelectEnter(interactor, interactable);
                Debug.Log("Gun grabbed automatically by " + handToAttach.name);
            }
            else
            {
                Debug.LogError("Failed to cast or interactionManager is missing.");
            }
        }
        else
        {
            Debug.LogError("Missing XRGrabInteractable or both XRDirectInteractors.");
        }
    }

    private void OnGunGrabbed(SelectEnterEventArgs args)
    {
        Debug.Log("Gun has been grabbed by " + args.interactorObject.transform.name);
    }
}
