using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class Projectile : MonoBehaviour
{

    public GameObject hitPrefab;

    public Material[] decals;

    public float lifetime;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        lifetime -= Time.deltaTime;
        if(lifetime <= 0.0f)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            // Do damage to player
        }
        else if(collision.gameObject.tag == "Enemy")
        {
            // Do damage to enemy
        }
        
        
        GameObject hit = Instantiate(hitPrefab, transform.position, Quaternion.FromToRotation(Vector3.up, collision.contacts[0].normal));
        hit.transform.parent = collision.transform;
        hit.GetComponentInChildren<DecalProjector>().material = decals[Random.Range(0, decals.Length)];
        Destroy(hit, 20.0f);
        Destroy(gameObject);
    }

    
}
