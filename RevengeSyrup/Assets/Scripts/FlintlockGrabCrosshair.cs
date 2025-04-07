using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
public class FlintlockGrabCrosshair : MonoBehaviour
{
    private XRGrabInteractable interactable;
    private CrosshairVR crosshairVR;

    private void Awake()
    {
        interactable = GetComponent<XRGrabInteractable>();
        crosshairVR = GetComponent<CrosshairVR>();
    }

    private void OnEnable()
    {
        interactable.selectEntered.AddListener(OnGrab);
        interactable.selectExited.AddListener(OnRelease);
    }

    private void OnDisable()
    {
        interactable.selectEntered.RemoveListener(OnGrab);
        interactable.selectExited.RemoveListener(OnRelease);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        crosshairVR.ShowCrosshair(true);
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        crosshairVR.ShowCrosshair(false);
    }
}
