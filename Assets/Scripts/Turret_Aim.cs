using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret_Aim : MonoBehaviour
{

    public Transform rotatingPart;
    
    public Transform targetLock;
    public Rigidbody targetRigidbody;
    public float rotateSpeed;

    public float aimLeadMultiplier;

    public float maxRotateAngle;

    public Ranged_Weapon weapon;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Aim();
        weapon.TryShoot();
    }

    //Aim towards the target if it is in range and the angle is not too large
    public void Aim()
    {
        if (targetLock != null && targetRigidbody != null)
        {
            Vector3 targetDir = (targetLock.position+ (targetRigidbody.velocity * ((targetLock.position-rotatingPart.position).magnitude/weapon.projectileSpeed))) - rotatingPart.position;
            float angle = Vector3.Angle(targetDir, this.transform.up);
            if (angle < maxRotateAngle)
            {
                targetDir.Normalize();
                Quaternion targetRotation = Quaternion.LookRotation(targetDir);
                rotatingPart.rotation = Quaternion.Slerp(rotatingPart.rotation, targetRotation, rotateSpeed * Time.deltaTime);
            }
        }
        else
        {
            rotatingPart.rotation = Quaternion.Slerp(rotatingPart.rotation, Quaternion.identity, rotateSpeed * Time.deltaTime);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(rotatingPart.position, rotatingPart.forward * 100.0f);
    }
}
