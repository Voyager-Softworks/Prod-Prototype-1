using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileLauncher : Ranged_Weapon, ITargetLockWeapon
{
    public GameObject projectilePrefab;
    public AudioSource fireSource;

    public Transform targetLock;

    public bool doAutoShoot = false;

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
        
    }

    public void Update()
    {
        if(doAutoShoot && targetLock != null)
        {
            
            TryShoot();
        }
    }
    
    public override void Fire()
    {
        GameObject projectile = Instantiate(projectilePrefab, muzzlePositions[currentMuzzle].position, muzzlePositions[currentMuzzle].rotation);
        projectile.GetComponent<Rigidbody>().velocity = projectile.transform.forward * projectileSpeed;
        if(transform.parent.parent.parent.GetComponentInChildren<Rigidbody>())
        {
            projectile.GetComponent<Rigidbody>().velocity += transform.parent.parent.parent.GetComponentInChildren<Rigidbody>().velocity;
        }
        fireSource.Play();
        if (anim != null)
        {
            anim.SetTrigger("Fire");
        }
        projectile.GetComponent<HomingMissile>().targetLock = targetLock;
        
    }
}
