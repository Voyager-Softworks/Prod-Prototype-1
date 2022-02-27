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
        indicator.SetActive(true);
    }

    public void Unlock()
    {
        lockonTarget = null;
        indicator.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.LookRotation(lockonTarget.position - transform.position);
        if (lockonTarget != null)
        {
            indicator.SetActive(true);
        }
        else
        {
            indicator.SetActive(false);
        }
    }
}
