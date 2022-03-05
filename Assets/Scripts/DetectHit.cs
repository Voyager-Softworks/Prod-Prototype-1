using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DetectHit : MonoBehaviour
{
    public string[] tagsToDetect;

    public UnityEvent onHit;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision other) {
        foreach(string tag in tagsToDetect){
            if(other.gameObject.tag == tag){
                onHit.Invoke();
            }
        }
    }
}
