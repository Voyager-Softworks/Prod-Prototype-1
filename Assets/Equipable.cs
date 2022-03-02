using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipable : MonoBehaviour
{
    public enum EquipableType
    {
        rangedWeapon,
        equipment
    }

    public EquipableType type = EquipableType.rangedWeapon;
    public List<HardpointManager.HardpointLocation> possibleLocations = new List<HardpointManager.HardpointLocation>();
    public bool isEquipped = false;

    public HardpointManager _hardpointManager;

    // Start is called before the first frame update
    void Start()
    {
        if (_hardpointManager == null)
        {
            _hardpointManager = FindObjectOfType<HardpointManager>();
        }

        if (_hardpointManager != null)
        {
            _hardpointManager.AddEquipable(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Equip(Transform hardpoint)
    {
        if (isEquipped)
        {
            return;
        }

        transform.parent = hardpoint;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        isEquipped = true;
    }

    public void Unequip()
    {
        if (!isEquipped)
        {
            return;
        }

        transform.parent = null;
        isEquipped = false;

        ITargetLockWeapon[] lockon = GetComponents<ITargetLockWeapon>();

        if (lockon.Length > 0)
        {
            foreach (ITargetLockWeapon wpn in lockon)
            {
                wpn.Unlock();
            }
        }
    }
}
