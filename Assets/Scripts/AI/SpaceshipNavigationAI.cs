using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceshipNavigationAI : MonoBehaviour
{
    Vector3 flightVector;
    public Transform targetPosition;

    Rigidbody rb;

    public float avoidanceRange;

    public float thrust;

    public float maxThrust;

    public float minThrust;

    public AnimationCurve thrustCurve;

    public float turnSpeed = 3.0f;

    //Set Flight Vector
    void SetFlightVector()
    {
        if((targetPosition.position - transform.position).magnitude < avoidanceRange) return;
        flightVector = targetPosition.position - transform.position;
    }

    
    void RollToFlightVector()
    {
        Vector3 rollVector = Vector3.ProjectOnPlane(flightVector, transform.forward).normalized;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(transform.forward, rollVector), Time.deltaTime * 1.0f);
    }

    void PitchToFlightVector()
    {
        
    }
    

    void RotateToFlightVector()
    {
        Vector3 rollVector = Vector3.ProjectOnPlane(flightVector, transform.forward).normalized;
        Vector3 pitchVector = Vector3.ProjectOnPlane(flightVector, transform.right).normalized;
        Vector3 yawVector = Vector3.ProjectOnPlane(flightVector, transform.up).normalized;

        float dot = Vector3.Dot(transform.up, rollVector);

        Quaternion roll = Quaternion.LookRotation(transform.forward, rollVector);
        Quaternion pitch = Quaternion.LookRotation(pitchVector, Vector3.Cross(pitchVector, transform.right));
        Quaternion yaw = Quaternion.LookRotation(yawVector, transform.up);

        Quaternion yawPitch = Quaternion.Slerp(yaw, pitch, Mathf.Abs(dot));



        transform.rotation = Quaternion.Slerp(transform.rotation,  Quaternion.Slerp(yawPitch,roll,0.5f), Time.deltaTime * turnSpeed);
    }



    //Detect if there is an obstacle in the way of the flight vector and adjust the flight vector to steer away from it
    void AvoidObstacles()
    {
        
        RaycastHit hit;
        if (Physics.Raycast(transform.position, flightVector, out hit, avoidanceRange * thrustCurve.Evaluate(Vector3.Distance(transform.position, targetPosition.position)/1000.0f)))
        {
            //If the raycast hit something, adjust the flight vector to steer away from it
            flightVector = (Vector3.Reflect(flightVector, hit.normal) + flightVector);
        }
        
        
    }

    

    public void SetTarget(Transform target)
    {
        targetPosition = target;
    }


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        SetFlightVector();
        AvoidObstacles();
        RotateToFlightVector();
        //Move forward
        rb.velocity = (transform.forward * Time.deltaTime * thrustCurve.Evaluate(Vector3.Distance(transform.position, targetPosition.position)/1000.0f) * maxThrust);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + flightVector);

        Vector3 rollVector = Vector3.ProjectOnPlane(flightVector, transform.forward).normalized;
        Vector3 pitchVector = Vector3.ProjectOnPlane(flightVector, transform.right).normalized;
        Vector3 yawVector = Vector3.ProjectOnPlane(flightVector, transform.up).normalized;

        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + rollVector*2.0f);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + pitchVector*2.0f);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + yawVector*2.0f);
    }
}
