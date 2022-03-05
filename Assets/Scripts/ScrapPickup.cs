using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrapPickup : MonoBehaviour
{
    public int scrapValue = 10;
    public AnimationCurve attractForce;
    public ShipHealth _playerhealth;
    private bool lockedIn = false;

    // Start is called before the first frame update
    void Start()
    {
        _playerhealth = FindObjectOfType<ShipHealth>();

        if (_playerhealth == null)
        {
            Debug.LogError("Player health not found");
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (_playerhealth == null) return;

        //move towards player if close enough
        float distance = Vector3.Distance(transform.position, _playerhealth.transform.position);
        float maxDist = attractForce.keys[attractForce.length - 1].time;
        Vector3 direction = (_playerhealth.transform.position - transform.position).normalized;

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
        if (_playerhealth == null) return;

        if (other.gameObject.tag == "Player")
        {
            _playerhealth.AddHealth(scrapValue);
            Destroy(gameObject);
        }
    }
}
