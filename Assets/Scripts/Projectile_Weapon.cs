using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_Weapon : Ranged_Weapon
{
    public GameObject projectilePrefab;
    
    
    
    public override void Fire()
    {
        GameObject projectile = Instantiate(projectilePrefab, muzzlePositions[currentMuzzle].position, muzzlePositions[currentMuzzle].rotation);
        projectile.GetComponent<Rigidbody>().velocity = projectile.transform.forward * projectileSpeed;
        Destroy(projectile, 5.0f);
    }
}
