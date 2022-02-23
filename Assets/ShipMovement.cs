using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShipMovement : MonoBehaviour
{
    public InputAction movementAction;
    public InputAction rollAction;
    public InputAction boostAction;
    public InputAction lookAction;

    public float forwardSpeed = 10f;
    public float turnSpeed = 10f;
    public float rollSpeed = 10f;
    public float strafeSpeed = 10f;
    public float verticalSpeed = 10f;
    public float maxSpeed = 10f;
    public float minSpeed = 0f;

    // Start is called before the first frame update
    void Start()
    {
        movementAction.Enable();
        rollAction.Enable();
        boostAction.Enable();
        lookAction.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
    }

    public void Movement() {
        //GET VALS:
        //move vec
        Vector3 movement = movementAction.ReadValue<Vector3>();
        //roll val
        float roll = rollAction.ReadValue<float>();
        //boost val
        float boost = boostAction.ReadValue<float>();


        //SET VALS:
        //calc individual movement values
        float forward = movement.z * forwardSpeed * (1 + boost);
        float strafe = movement.x * strafeSpeed;
        float vertical = movement.y * verticalSpeed;

        //calc total movement
        Vector3 totalMovement = new Vector3(strafe, vertical, forward);


        //MOVEMENT:
        //rotate ship
        transform.RotateAround(transform.position, transform.forward, roll * rollSpeed * Time.deltaTime);

        //move ship
        transform.Translate(totalMovement * Time.deltaTime, Space.Self);
    }

    public void Looking() {
        //GET VALS:
        //look vec
        Vector2 look = lookAction.ReadValue<Vector2>();

        //SET VALS:
        //calc individual look values
        float yaw = look.x * turnSpeed;
        float pitch = look.y * turnSpeed;

        //calc total look
        Vector3 totalLook = new Vector3(0, yaw, pitch);

        //LOOKING:
        //rotate ship
        transform.Rotate(totalLook, Space.Self);
    }
}
