using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockonIndicator : MonoBehaviour, ITargetLockWeapon
{
    public Transform lockonTarget;
    public GameObject indicator;
    // Start is called before the first frame update
    void Start()
    {
        FindObjectOfType<LockOnTargeter>().RegisterLockOnListener(this);
    }

    public void Lock(Transform target)
    {
        lockonTarget = target;
        
    }

    public void Unlock()
    {
        lockonTarget = null;
        
    }

    // Update is called once per frame
    void Update()
    {

        if (lockonTarget != null)
        {
            indicator.SetActive(true);
            transform.rotation = Quaternion.LookRotation(lockonTarget.position - transform.position);
        }
        else
        {
            indicator.SetActive(false);
        }
    }
}
