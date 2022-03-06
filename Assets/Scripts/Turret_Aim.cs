using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret_Aim : MonoBehaviour, ITargetLockWeapon
{

    public Transform rotatingPart;
    
    public Transform targetLock;
    public Rigidbody targetRigidbody;
    public bool doAutoShoot = false;
    public bool doAutoAim = true;
    public float rotateSpeed;

    public float aimLeadMultiplier;

    public float maxRotateAngle;

    public Ranged_Weapon weapon;

    public bool alternativeForwardVector = false;

    public void Lock(Transform target)
    {
        targetLock = target;
        targetRigidbody = target.GetComponent<Rigidbody>();
    }

    public void Unlock()
    {
        targetLock = null;
        targetRigidbody = null;
    }


    // Start is called before the first frame update
    void Start()
    {
        // if(transform.parent && transform.parent.tag == "Player")
        // {
        //     FindObjectOfType<LockOnTargeter>().RegisterLockOnListener(this);
        // }
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Aim() && doAutoShoot) weapon.TryShoot();
    }

    public void ToggleAutoShoot(){
        SetAutoShoot(!doAutoShoot);
    }
    public void SetAutoShoot(bool value){
        doAutoShoot = value;
    }

    public void ToggleAutoAim(){
        SetAutoAim(!doAutoAim);
    }
    public void SetAutoAim(bool value){
        doAutoAim = value;
    }

    //Aim towards the target if it is in range and the angle is not too large
    public bool Aim()
    {
        if(!doAutoAim) return false;

        

        if (targetLock != null && targetRigidbody != null)
        {
            Vector3 targetDir = (targetLock.position+ (targetRigidbody.velocity * ((targetLock.position-rotatingPart.position).magnitude/weapon.projectileSpeed))) - rotatingPart.position;
            float angle = Vector3.Angle(targetDir, (alternativeForwardVector ? this.transform.up : this.transform.forward));
            if (angle < maxRotateAngle)
            {
                targetDir.Normalize();
                Quaternion targetRotation = Quaternion.LookRotation(targetDir);
                rotatingPart.rotation = Quaternion.Slerp(rotatingPart.rotation, targetRotation, rotateSpeed * Time.deltaTime);
                return true;
            }
            else
            {
                Quaternion targetRotation = Quaternion.LookRotation((alternativeForwardVector ? this.transform.up : this.transform.forward));
                rotatingPart.rotation = Quaternion.Slerp(rotatingPart.rotation, targetRotation, rotateSpeed * Time.deltaTime);
                return false;
            }
        }
        else
        {
            Quaternion targetRotation = Quaternion.LookRotation((alternativeForwardVector ? this.transform.up : this.transform.forward));
            rotatingPart.rotation = Quaternion.Slerp(rotatingPart.rotation, targetRotation, rotateSpeed * Time.deltaTime);
            return false;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(rotatingPart.position, (alternativeForwardVector ? rotatingPart.transform.up : rotatingPart.transform.forward) * 100.0f);
    }
}
