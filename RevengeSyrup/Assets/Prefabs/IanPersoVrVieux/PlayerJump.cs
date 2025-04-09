using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
 
public class PlayerJump : MonoBehaviour
{
    //Utiliser ce tutoriel : https://www.youtube.com/watch?v=Xf2eDfLxcB8
 
    [SerializeField] private InputActionProperty jumpButton;
    [SerializeField] private float jumpHeigt = 3f;
    [SerializeField] private CharacterController cc;
    [SerializeField] private LayerMask groundLayers;
 
    private bool timer;
 
    private float gravity = Physics.gravity.y;
 
    private Vector3 movement;
 
    private void Update()
    {
        bool _IsGrounded = IsGrounded();
 
        if (jumpButton.action.WasPressedThisFrame() && _IsGrounded)
        {
            StartCoroutine("sautTest");
            Jump();
           
        }
 
        
 
        if (timer == true)
        {
            movement.y += gravity * Time.deltaTime;
            cc.Move(movement * Time.deltaTime);
 
        }

 
    }
        private void Jump()
    {
        movement.y = Mathf.Sqrt(jumpHeigt * -3.0f * gravity);
    }
 
    private bool IsGrounded()
    {
        return Physics.CheckSphere(transform.position, 0.2f, groundLayers);
    }
 
    public IEnumerator sautTest()
    {
        timer = true;
        yield return new WaitForSeconds(2f);
        timer = false; 
        yield break; 
    }
}

/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJump : MonoBehaviour
{
    //Utiliser ce tutoriel : https://www.youtube.com/watch?v=Xf2eDfLxcB8

    [SerializeField] private InputActionProperty jumpButton;
    [SerializeField] private float jumpHeigt = 3f;
    [SerializeField] private CharacterController cc;
    [SerializeField] private LayerMask groundLayers;

    private float gravity = Physics.gravity.y;

    private Vector3 movement;

    private void Update()
    {
        bool _IsGrounded = IsGrounded();

        if (jumpButton.action.WasPressedThisFrame() && _IsGrounded)
        {
            Jump();
        }

        movement.y += gravity * Time.deltaTime;

        cc.Move(movement * Time.deltaTime);
    }
    private void Jump()
    {
        movement.y = Mathf.Sqrt(jumpHeigt * -3.0f * gravity);
    }

    private bool IsGrounded()
    {
        return Physics.CheckSphere(transform.position, 0.2f, groundLayers);
    }
}*/
