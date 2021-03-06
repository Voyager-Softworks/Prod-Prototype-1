using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrapPickup : MonoBehaviour
{
    public int scrapValue = 10;
    public AnimationCurve attractForce;
    public ScrapManager _scrapManager;
    private bool lockedIn = false;

    // Start is called before the first frame update
    void Start()
    {
        _scrapManager = FindObjectOfType<ScrapManager>();

        if (_scrapManager == null)
        {
            Debug.LogError("Player health not found");
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (_scrapManager == null) return;

        //move towards player if close enough
        float distance = Vector3.Distance(transform.position, _scrapManager.transform.position);
        float maxDist = attractForce.keys[attractForce.length - 1].time;
        Vector3 direction = (_scrapManager.transform.position - transform.position).normalized;

        //catch scrap and drag in if close enough
        if (distance <= maxDist)
        {
            lockedIn = true;

            float force = attractForce.Evaluate(distance);
            GetComponent<Rigidbody>().AddForce(direction * force);
        }

        //if locked in, try keep up with player (easier to "catch" scrap)
        if (lockedIn){
            Vector3 moveVel = direction * (distance);

            if (GetComponent<Rigidbody>().velocity.magnitude < moveVel.magnitude)
            {
                GetComponent<Rigidbody>().velocity = moveVel;
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (_scrapManager == null) return;

        if (other.gameObject.tag == "Player")
        {
            _scrapManager.AddScrap(scrapValue);
            Destroy(gameObject);
        }
    }
}
