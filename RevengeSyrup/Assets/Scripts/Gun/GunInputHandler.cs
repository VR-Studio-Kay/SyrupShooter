using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public class GunInputHandler : MonoBehaviour
{
    [Tooltip("The input action used to trigger firing.")]
    public InputActionProperty activateAction;

    private GunControllerPlayer gunController;

    private void OnEnable()
    {
        activateAction.action.performed += OnActivatePerformed;
        activateAction.action.Enable();
    }

    private void OnDisable()
    {
        activateAction.action.performed -= OnActivatePerformed;
        activateAction.action.Disable();
    }

    private void Start()
    {
        gunController = GetComponent<GunControllerPlayer>();
        if (!gunController) Debug.LogWarning("GunControllerPlayer not found.");
    }

    private void OnActivatePerformed(InputAction.CallbackContext context)
    {
        gunController?.TryFire();
    }
}
