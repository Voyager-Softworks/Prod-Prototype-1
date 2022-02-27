using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITargetLockWeapon 
{
    public abstract void Lock(Transform target);
    
    public abstract void Unlock();
    

}
