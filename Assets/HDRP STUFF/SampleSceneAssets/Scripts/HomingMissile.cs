using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingMissile : MonoBehaviour
{
    public float thrust = 10.0f;
    public float turnSpeed = 10.0f;
    public float secondsOfFuel = 5.0f;

    public Transform targetLock;

    public float thrusterDelay = 0.5f;
    float thrusterTimer = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(thrusterTimer < thrusterDelay)
        {
            thrusterTimer += Time.deltaTime;
        }
        else
        {
            if (targetLock != null)
            {
                Vector3 targetDir = targetLock.position - transform.position;
                
                
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetDir), turnSpeed * Time.deltaTime);
                    
                    GetComponent<Rigidbody>().AddForce(transform.forward * thrust);
                    
                
            }
        }
    }
}
