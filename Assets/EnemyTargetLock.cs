using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTargetLock : MonoBehaviour
{

    public Transform playerTransform;
    
    public List<ITargetLockWeapon> weapons;

    public float range;

    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform.root;
        //Find all weapons
        weapons = new List<ITargetLockWeapon>();
        foreach(ITargetLockWeapon weapon in GetComponentsInChildren<ITargetLockWeapon>()){
            weapons.Add(weapon);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector3.Distance(transform.position, playerTransform.position) < range)
        {
            foreach(ITargetLockWeapon weapon in weapons)
            {
                weapon.Lock(playerTransform);
            }
        }
        else
        {
            foreach(ITargetLockWeapon weapon in weapons)
            {
                weapon.Unlock();
            }
        }
    }
}
