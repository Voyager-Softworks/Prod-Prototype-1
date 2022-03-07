using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCage : MonoBehaviour
{

    public float radius;
    

    // Update is called once per frame
    void Update()
    {
        if(Vector3.Distance(transform.position, Vector3.zero) > radius)
        {
            transform.position = (transform.position.normalized * radius);
            GetComponent<Rigidbody>().velocity = -transform.position.normalized * GetComponent<Rigidbody>().velocity.magnitude * 2.0f;
        }
        
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
