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

    protected Animator anim;

    void Start()
    {
        currentAmmo = clipSize;
        firingTimer = 0.0f;
        reloadTimer = 0.0f;
        currentMuzzle = 0;
        anim = GetComponentInChildren<Animator>();
    }

    protected void Update()
    {
        if (reloadTimer <= 0)
        {
            Reload();
        }
        else if (currentAmmo <= 0 && reloadTimer > 0.0f)
        {
            reloadTimer -= Time.deltaTime;
        }
    }

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
    }



    virtual public void Fire()
    {

    }
    public void Reload()
    {
        reloadTimer = reloadTime;
        currentAmmo = clipSize;
    }

    public int GetCurrentAmmo()
    {
        return currentAmmo;
    }

    public float GetRemainingReloadTime(){
        return reloadTimer;
    }
}
