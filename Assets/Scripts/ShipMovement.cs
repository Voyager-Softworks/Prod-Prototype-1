using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class ShipMovement : MonoBehaviour
{
    public InputAction movementAction;
    public InputAction rollAction;
    public InputAction boostAction;
    public InputAction dampenerAction;
    public InputAction lookAction;

    public AnimationCurve trottleCurve;
    public float currentThrottle = 0.0f;

    public float strafeSpeed = 20.0f;
    public float rollSpeed = 5.0f;

    public bool isDampened = false;
    public float dampeningTime = 1.0f;
    public Vector2 minmaxLinearDampen = new Vector2(0.0f, 1.5f);
    public Vector2 minmaxAngularDampen = new Vector2(0.5f, 1.0f);

    public GameObject o_camera;
    public GameObject o_hull;
    public GameObject o_wings;

    public GameObject ui_speedometer;
    public GameObject ui_throttleBar;
    public TextMeshProUGUI ui_dampenText;

    [SerializeField] private Transform t_shootspot;
    public GameObject p_bullet;

    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        //lock hide cursor 
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        rb = GetComponent<Rigidbody>();

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

        //update UI
        UpdateUI();
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
        
        rb.AddForce(transform.forward * trottle);

        //strafe calcs
        float strafe = movement.x * strafeSpeed;
        
        rb.AddForce(transform.right * strafe);

        //elevate calcs
        float elevate = movement.y * strafeSpeed;

        rb.AddForce(transform.up * elevate);

        //roll calcs
        float rollForce = roll * rollSpeed;
        rb.AddTorque(transform.forward * rollForce);

        CheckDampen();
    }

    public void ToggleDampen(InputAction.CallbackContext context) {
        Debug.Log("Dampener toggled");
        if (context.performed) {
            isDampened = !isDampened;
        }
    }

    public void CheckDampen() {
        //update linear drag
        rb.drag = Mathf.Lerp(rb.drag, isDampened ? minmaxLinearDampen.y : minmaxLinearDampen.x, Time.deltaTime * dampeningTime);

        //update angular drag
        rb.angularDrag = Mathf.Lerp(rb.angularDrag, isDampened ? minmaxAngularDampen.y : minmaxAngularDampen.x, Time.deltaTime * dampeningTime);

        //Boost Drag System:
        float boost = boostAction.ReadValue<float>();
        if (boost > 0.0f){
            Vector3 velocity = rb.velocity;

            //forward speed
            float newZVel = Vector3.Project(velocity, transform.forward).magnitude;
            if (newZVel <= 0.0f) {
                newZVel = newZVel * (1 - Time.deltaTime * rb.drag);
            }

            //strafe speed
            float newXVel = Vector3.Project(velocity, transform.right).magnitude;
            newXVel = newXVel * (1 - Time.deltaTime * rb.drag);

            //elevate speed
            float newYVel = Vector3.Project(velocity, transform.up).magnitude;
            newYVel = newYVel * (1 - Time.deltaTime * rb.drag);

            Vector3 newVel = new Vector3(newXVel, newYVel, newZVel);

            //un-project velocity back to world space
            rb.velocity = transform.TransformDirection(newVel);
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
        rb.AddTorque(transform.up * look.x * 0.25f);
        rb.AddTorque(transform.right * look.y * 0.25f);
    }

    public void UpdateUI() {
        //get velocity
        Vector3 velocity = rb.velocity;

        //get forward component of velocity
        float speed = Vector3.Project(velocity, transform.forward).magnitude;

        //update speedometer
        ui_speedometer.GetComponent<TextMeshProUGUI>().text = speed.ToString("F0") + " m/s";

        //update throttle bar
        ui_throttleBar.GetComponent<RectTransform>().sizeDelta = new Vector2(Mathf.Abs(currentThrottle) * 200 - 200, 25);

        Color niceGreen = Color.green / 1.5f;
        Color niceRed = Color.red / 1.5f;

        //update colour of throttle bar
        ui_throttleBar.GetComponent<Image>().color = currentThrottle > 0 ? niceGreen : niceRed;

        //update dampen text
        ui_dampenText.text = isDampened ? "DAMPENED" : "UN-DAMPENED";
        ui_dampenText.fontStyle = isDampened ? FontStyles.Normal : FontStyles.Underline;
        ui_dampenText.color = isDampened ? niceGreen : niceRed - new Color(0, 0, 0, 0.5f);
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
            Vector3 targetVel = Vector3.zero;
            RaycastHit hit;
            if (Physics.Raycast(o_camera.transform.position, o_camera.transform.forward, out hit)) {
                hitpos = hit.point;

                if (hit.rigidbody) {
                    targetVel = hit.rigidbody.velocity;
                }
            }

            //calc bullet direction
            Vector3 direction = hitpos - t_shootspot.position;

            //set initial temp velocity
            Vector3 tempVel = direction.normalized * 50.0f + GetComponent<Rigidbody>().velocity;
            //set real velocity
            bullet.GetComponent<Rigidbody>().velocity = direction.normalized * tempVel.magnitude;
            //add target vel if it exists
            bullet.GetComponent<Rigidbody>().velocity += targetVel;

            //destroy bullet after 10 seconds
            Destroy(bullet, 30f);
        }
    }
}
