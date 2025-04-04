using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovingDash : MonoBehaviour
{
    public CharacterController controller;
    public float dashSpeed;
    public float dashTime;
    public float dashCooldown;
    public ParticleSystem dashParticles;
    public AudioSource dashAudio;

    private Vector3 moveDirection = Vector3.zero;
    private bool isDashing = false;
    private bool canDash = true;

    public InputActionReference customDashReference;
    private InputAction dashAction;

    private void Start()
    {
        dashAction = customDashReference.action;
        dashAction.started += OnDashStarted;
        dashAction.canceled += OnDashCanceled;
        dashAction.Enable();
    }

    private void OnDashStarted(InputAction.CallbackContext context)
    {
        if (canDash && !isDashing && transform.localScale.magnitude <= 2.5f)
        {
            StartCoroutine(Dash());
        }
    }

    private void OnDashCanceled(InputAction.CallbackContext context)
    {
        if (isDashing)
        {
            StopCoroutine(Dash());
            moveDirection = Vector3.zero;
            isDashing = false;
        }
    }

    IEnumerator Dash()
    {
        isDashing = true;
        canDash = false;
        float startTime = Time.time;
        moveDirection = Camera.main.transform.forward;

        if (dashAudio != null)
        {
            dashAudio.Play();
        }

        while (Time.time < startTime + dashTime)
        {
            controller.Move(moveDirection * dashSpeed * Time.deltaTime);
            if (dashParticles != null)
            {
                if (!dashParticles.isPlaying)
                {
                    dashParticles.Play();
                }
            }
            yield return null;
        }
        moveDirection = Vector3.zero;
        isDashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
}