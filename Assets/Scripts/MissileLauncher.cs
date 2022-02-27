using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileLauncher : Ranged_Weapon, ITargetLockWeapon
{
    public GameObject projectilePrefab;
    public AudioSource fireSource;

    public Transform targetLock;

    public void Lock(Transform target)
    {
        targetLock = target;
    }

    public void Unlock()
    {
        targetLock = null;
    }

    void Start()
    {
        if(transform.parent.tag == "Player")
        {
            FindObjectOfType<LockOnTargeter>().RegisterLockOnListener(this);
        }
    }
    
    public override void Fire()
    {
        GameObject projectile = Instantiate(projectilePrefab, muzzlePositions[currentMuzzle].position, muzzlePositions[currentMuzzle].rotation);
        projectile.GetComponent<Rigidbody>().velocity = projectile.transform.forward * projectileSpeed;
        fireSource.Play();
        if (anim != null)
        {
            anim.SetTrigger("Fire");
        }
        projectile.GetComponent<HomingMissile>().targetLock = targetLock;
        
    }
}
