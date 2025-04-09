using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class SocketWithTagCheck : XRSocketInteractor
{
    public string targetTag = string.Empty;

    public override bool CanHover(IXRHoverInteractable interactable)
    {
        return base.CanHover(interactable) && MatchUsingTag(interactable as XRBaseInteractable);
    }

    public override bool CanSelect(IXRSelectInteractable interactable)
    {
        return base.CanSelect(interactable) && MatchUsingTag(interactable as XRBaseInteractable);
    }

    private bool MatchUsingTag(XRBaseInteractable interactable)
    {
        // If interactable is null (cast failed), return false
        if (interactable == null) return false;

        return interactable.CompareTag(targetTag);
    }
}
