using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingMissile : MonoBehaviour
{
    public float damage = 30.0f;
    public float thrust = 10.0f;
    public float targetDrag = 2.0f;
    public float turnSpeed = 10.0f;
    public float secondsOfFuel = 5.0f;

    public Transform targetLock;

    public float thrusterDelay = 0.5f;
    float thrusterTimer = 0.0f;

    public GameObject explosionPrefab;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<Rigidbody>().drag = Mathf.Lerp(GetComponent<Rigidbody>().drag, targetDrag, Time.deltaTime);

        if(thrusterTimer < thrusterDelay)
        {
            thrusterTimer += Time.deltaTime;
        }
        else
        {
            if(secondsOfFuel > 0.0f)
            {
                secondsOfFuel -= Time.deltaTime;
            if (targetLock != null)
            {
                Vector3 targetDir = (targetLock.position - transform.position).normalized;
                
                    targetDir += (transform.forward* Vector3.Dot(targetDir, GetComponent<Rigidbody>().velocity))/2.0f;
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetDir), turnSpeed * Time.deltaTime);
                    
                    
                    
                
            }
            GetComponent<Rigidbody>().AddForce(transform.forward * thrust);
            }
            else
            {
                GameObject explosion = Instantiate(explosionPrefab, transform.position, transform.rotation); 
        
                Destroy(gameObject);
            }

        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<ShipHealth>().TakeDamage(damage);
        }
        else if(collision.gameObject.tag == "Enemy")
        {
            // Do damage to enemy
        }
        //spawn explosion prefab
        GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.FromToRotation(Vector3.up, collision.contacts[0].normal) );
        explosion.transform.parent = collision.transform;
        Destroy(gameObject);
    }
}
