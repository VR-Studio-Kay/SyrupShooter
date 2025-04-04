using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecreaseTestHealth : MonoBehaviour
{
    [SerializeField]
    private float speed = 5;
    [SerializeField] 
    private float sense = 10;
    
    private Vector3 MoveDir;
    private float horizontalMovement = 0;
    private float verticalMovement = 0;
    void GetInputs(){
        horizontalMovement = Input.GetAxisRaw("Horizontal");
        verticalMovement = Input.GetAxisRaw("Vertical");

        MoveDir = transform.forward * verticalMovement + transform.right * horizontalMovement;
    }
    

    
    private Rigidbody rb;
    void moveRigid(){
        rb.AddForce(MoveDir.normalized * speed, ForceMode.Acceleration);
    }

    
    
    private Camera cam;
    private float mouseX = 0;
    private float mouseY = 0;
    private float xRot = 0;
    private float yRot = 0;
    void MouseLook(){
        mouseX = Input.GetAxis("Mouse Y");
        mouseY = Input.GetAxis("Mouse X");

        xRot -= mouseX * sense;
        yRot += mouseY * sense;

        xRot = Mathf.Clamp(xRot, -90, 90);

        cam.transform.localRotation = Quaternion.Euler(xRot, 0, 0);
        transform.rotation = Quaternion.Euler(0, yRot, 0);
    }
        
    private RaycastHit hit;
    void RayCastMethod(){
        if(Physics.Raycast(cam.gameObject.transform.position, cam.gameObject.transform.forward, out hit,Mathf.Infinity)){
            if(hit.transform.gameObject.TryGetComponent(out HealthBar health)){
                if(Input.GetMouseButtonDown(0)){   
                    health.TakeDamage(10);
                }
            }
        }
    }
        

    
    void Start(){
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        cam = GetComponentInChildren<Camera>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
        

    
    void Update()
    {
        RayCastMethod();
        GetInputs();
        MouseLook();
    }
    

    
    void FixedUpdate() {
        moveRigid();    
    }
}
