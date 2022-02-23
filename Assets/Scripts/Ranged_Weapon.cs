using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ranged_Weapon : MonoBehaviour
{
    public int clipSize;
    protected int currentAmmo;
    public float firingDelay;
    protected float firingTimer;
    public float reloadTime;
    protected float reloadTimer;
    public Transform[] muzzlePositions;
    protected int currentMuzzle;

    public float projectileSpeed;
    public void TryShoot()
    {
        if (currentAmmo > 0)
        {
            if (firingTimer <= 0)
            {
                Fire();
                currentMuzzle++;
                if(currentMuzzle >= muzzlePositions.Length)
                {
                    currentMuzzle = 0;
                }
                firingTimer = firingDelay;
                currentAmmo--;
            }
            else
            {
                firingTimer -= Time.deltaTime;
            }
        }
        else
        {
            if (reloadTimer <= 0)
            {
                Reload();
            }
        }
    }



    virtual public void Fire()
    {

    }
    public void Reload()
    {
        reloadTimer = reloadTime;
        StartCoroutine(ReloadCoroutine());
    }

    // coroutine to reload
    IEnumerator ReloadCoroutine()
    {
        while(reloadTimer > 0)
        {
            reloadTimer -= Time.deltaTime;
            yield return null;
        }
        currentAmmo = clipSize;
    }
}
