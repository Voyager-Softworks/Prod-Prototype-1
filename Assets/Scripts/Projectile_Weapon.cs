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
        if(GetComponentInParent<Rigidbody>())
        {
            Vector3 parentVel = GetComponentInParent<Rigidbody>().velocity;
            //project parentVel to the projectile's forward vector
            Vector3 vel = Vector3.Project(parentVel, projectile.transform.forward);
            projectile.GetComponent<Rigidbody>().velocity += vel;


            //projectile.GetComponent<Rigidbody>().velocity += transform.parent.parent.parent.GetComponentInChildren<Rigidbody>().velocity;
        }
        fireSource.Play();
        if (anim != null)
        {
            anim.SetTrigger("Fire");
        }
        Destroy(projectile, 5.0f);
    }
}
