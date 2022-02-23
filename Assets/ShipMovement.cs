using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShipMovement : MonoBehaviour
{
    public InputAction movementAction;
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
        
    }

    // Update is called once per frame
    void Update()
    {
        KeyboardMovement();
    }

    public void KeyboardMovement() {
        float horizontal = Input.GetAxis("Horizontal");
        float forward = Input.GetAxis("Forward");
        float vertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontal, vertical, forward);

        transform.Translate(movement * Time.deltaTime);
    }
}
