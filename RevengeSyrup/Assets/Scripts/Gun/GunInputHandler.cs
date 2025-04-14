using UnityEngine;
using UnityEngine.InputSystem;

public class GunInputHandler : MonoBehaviour
{
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
    }

    private void OnActivatePerformed(InputAction.CallbackContext context)
    {
        gunController.TryFire();
    }
}
