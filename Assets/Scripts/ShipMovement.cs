using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShipMovement : MonoBehaviour
{
    public InputAction movementAction;
    public InputAction rollAction;
    public InputAction boostAction;
    public InputAction dampenerAction;
    public InputAction lookAction;

    public AnimationCurve trottleCurve;
    public float currentThrottle = 0.0f;

    public GameObject o_camera;
    public GameObject o_hull;
    public GameObject o_wings;

    [SerializeField] private Transform t_shootspot;
    public GameObject p_bullet;

    // Start is called before the first frame update
    void Start()
    {
        //lock hide cursor 
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        //enable actions
        movementAction.Enable();
        dampenerAction.Enable();
        rollAction.Enable();
        boostAction.Enable();
        lookAction.Enable();

        //set up dampener
        dampenerAction.performed += ToggleDampen;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Movement();
        Looking();

        TESTShoot();
    }

    public void Movement() {
        //GET VALS:
        //move vec
        Vector3 movement = movementAction.ReadValue<Vector3>();
        //roll val
        float roll = rollAction.ReadValue<float>();
        //boost val
        float boost = boostAction.ReadValue<float>();

        //throttle calcs
        currentThrottle = Mathf.Lerp(currentThrottle, trottleCurve.Evaluate(movement.z), Time.deltaTime * 5.0f);

        float trottle = trottleCurve.Evaluate(currentThrottle) * 50.0f;
        
        GetComponent<Rigidbody>().AddForce(transform.forward * trottle);

        //strafe calcs
        float strafe = movement.x * 20;
        
        GetComponent<Rigidbody>().AddForce(transform.right * strafe);

        //elevate calcs
        float elevate = movement.y * 20;

        GetComponent<Rigidbody>().AddForce(transform.up * elevate);

        //roll calcs
        float rollForce = roll * 5;
        GetComponent<Rigidbody>().AddTorque(transform.forward * rollForce);


    }

    public void ToggleDampen(InputAction.CallbackContext context) {
        Debug.Log("Dampener toggled");
        if (context.performed) {
            GetComponent<Rigidbody>().drag = GetComponent<Rigidbody>().drag == 0.0f ? 0.5f : 0.0f;
        }
    }

    public void Looking() {
        if (Cursor.lockState != CursorLockMode.Locked) {
            return;
        }

        //GET VALS:
        //look vec
        Vector2 look = lookAction.ReadValue<Vector2>();
        //invert y
        look.y *= -1;

        //torque ship towards look
        GetComponent<Rigidbody>().AddTorque(transform.up * look.x * 0.25f);
        GetComponent<Rigidbody>().AddTorque(transform.right * look.y * 0.25f);
    }

    float lastShoot = 0f;
    float shootDelay = 0.1f;

    public void TESTShoot() {
        if (Mouse.current.leftButton.isPressed && Time.time - lastShoot > shootDelay) {
            lastShoot = Time.time;

            Debug.Log("Shoot");

            //instantiate bullet
            GameObject bullet = Instantiate(p_bullet, t_shootspot.position, t_shootspot.rotation);

            //raycast to find target position
            Vector3 hitpos = t_shootspot.position + t_shootspot.forward;
            RaycastHit hit;
            if (Physics.Raycast(o_camera.transform.position, o_camera.transform.forward, out hit)) {
                hitpos = hit.point;
            }

            //calc bullet direction
            Vector3 direction = hitpos - t_shootspot.position;

            //set velocity
            bullet.GetComponent<Rigidbody>().velocity = direction.normalized * 50.0f;

            //destroy bullet after 10 seconds
            Destroy(bullet, 10f);
        }
    }
}
