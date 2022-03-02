using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_Weapon : Ranged_Weapon, ITargetLockWeapon
{
    public GameObject projectilePrefab;
    public AudioSource fireSource;
    
    public Transform targetLock;

    public void Lock(Transform target)
    {
        targetLock = target;
    }
    void Start()
    {
        // if(transform.parent && transform.parent.tag == "Player")
        // {
        //     FindObjectOfType<LockOnTargeter>().RegisterLockOnListener(this);
        // }
    }
    public void Unlock()
    {
        targetLock = null;
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
        Destroy(projectile, 5.0f);
    }
}
