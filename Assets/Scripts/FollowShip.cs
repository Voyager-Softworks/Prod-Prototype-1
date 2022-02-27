using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowShip : MonoBehaviour
{
    public GameObject ship;
    public GameObject followPoint;
    public Vector3 initialOffset;

    // Start is called before the first frame update
    void Start()
    {
        initialOffset = transform.position - ship.transform.position;
        followPoint.transform.position = transform.position;
    }

    private void FixedUpdate() {
        float followSpeed = ship.GetComponent<Rigidbody>().velocity.magnitude;
        if (followSpeed < 10.0f) followSpeed = 10.0f;

        //lerp to the ship
        transform.position = Vector3.Lerp(transform.position, followPoint.transform.position, Time.deltaTime * followSpeed);

        float rotateSpeed = (ship.GetComponent<Rigidbody>().angularVelocity.magnitude) * Mathf.Rad2Deg;
        if (rotateSpeed > 0) rotateSpeed = 10.0f;
        //rotate to the ship
        transform.rotation = Quaternion.Lerp(transform.rotation, ship.transform.rotation, Time.deltaTime * rotateSpeed);
    }
}
