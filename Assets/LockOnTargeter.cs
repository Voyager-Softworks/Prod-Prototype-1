using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class LockOnTargeter : MonoBehaviour
{
    public Sprite normalReticle, hoverReticle, lockReticle;
    public Image reticle;
    public Transform lockonTarget;

    public InputAction lockOnAction;

    List<ITargetLockWeapon> weapons = new List<ITargetLockWeapon>();

    public void RegisterLockOnListener(ITargetLockWeapon wpn)
    {
        weapons.Add(wpn);
    }

    public enum ReticleState
    {
        Normal,
        Hover,
        Locked
    }

    public ReticleState currentState;

    // Start is called before the first frame update
    void Start()
    {
        lockOnAction.Enable();
        lockOnAction.performed += OnLockOnAttempt;
        currentState = ReticleState.Normal;
    }

    void OnLockOnAttempt(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (currentState == ReticleState.Hover)
            {
                currentState = ReticleState.Locked;
                CheckForLockon();
            }
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case ReticleState.Normal:
                if(CheckIfHovering())
                {
                    currentState = ReticleState.Hover;
                    reticle.sprite = hoverReticle;
                }
                break;
            case ReticleState.Hover:
                if(CheckIfHovering())
                {
                    currentState = ReticleState.Hover;
                }
                else
                {
                    currentState = ReticleState.Normal;
                    reticle.sprite = normalReticle;
                }
                break;
            case ReticleState.Locked:
                if (lockonTarget == null)
                {
                    currentState = ReticleState.Normal;
                    foreach (ITargetLockWeapon wpn in weapons)
                    {
                        wpn.Unlock();
                    }
                    reticle.sprite = normalReticle;
                }
                break;
        }
    }

    //Raycast from the camera and check if an object is in range
    public void CheckForLockon()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 500.0f))
        {
            if (hit.transform.tag == "Enemy")
            {
                lockonTarget = hit.transform;
                reticle.sprite = lockReticle;
                currentState = ReticleState.Locked;
                foreach (ITargetLockWeapon wpn in weapons)
                    {
                        wpn.Lock(lockonTarget);
                    }
            }
            else
            {
                lockonTarget = null;
                reticle.sprite = normalReticle;
                currentState = ReticleState.Normal;
            }
        }
        else
        {
            lockonTarget = null;
            reticle.sprite = normalReticle;
            currentState = ReticleState.Normal;
        }
    }

    public bool CheckIfHovering()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 500.0f))
        {
            if (hit.transform.tag == "Enemy")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }
}
