using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

//manages the equipping, unequipping, and usage of hardpoints (weapons and equipment)
public class HardpointManager : MonoBehaviour
{
    public enum HardpointLocation
    {
        side,
        bottom
    }

    [Serializable]
    public class Hardpoint
    {
        [SerializeField] public Transform hardpoint = null;
        [SerializeField] public HardpointLocation location = HardpointLocation.side;
        [SerializeField] public Equipable equipped = null;
    }

    public InputAction fireAction;
    public InputAction equipAction;
    public InputAction unequipAction;
    public InputAction scrollAction;
    public List<InputAction> hardpointSelectActions = new List<InputAction>();

    [SerializeField] public List<Hardpoint> hardpoints = new List<Hardpoint>();
    [SerializeField] public List<Equipable> allEquipables = new List<Equipable>();

    public LockOnTargeter _lockOnTargeter;

    public TextMeshProUGUI _canEquipText;
    public TextMeshProUGUI _currentEquipText;
    public Image nearestEquipableIcon;
    public HardpointList _hardpointList;
    private Equipable nearestEquipable = null;
    public int selectedHardpoint = 0;
    public float unequipTime = 1.0f;
    private float unequipTimer = 0.0f;
    public float equipDistance = 10.0f;

    public AudioClip equipSound;
    public AudioClip unequipSound;
    public AudioClip failEquipSound;

    // Start is called before the first frame update
    void Start()
    {
        fireAction.Enable();
        equipAction.Enable();
        unequipAction.Enable();
        scrollAction.Enable();
        foreach (InputAction action in hardpointSelectActions)
        {
            action.Enable();
        }

        Equipable[] eqs = GameObject.FindObjectsOfType<Equipable>();
        foreach (Equipable eq in eqs)
        {
            AddEquipable(eq);
        }

        //fireAction.performed += TryFireSelected;
        equipAction.performed += TryEquipNearest;
        scrollAction.performed += ScrollHardpoints;
        foreach (InputAction action in hardpointSelectActions)
        {
            action.performed += SelectHardpoint;
        }

        if (nearestEquipableIcon == null)
        {
            nearestEquipableIcon = GameObject.Find("NearestEquipableIcon").GetComponent<Image>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateUI();
        CheckFiringAction();
        CheckUnequipAction();
        UpdateLockon();

    }

    public void UpdateLockon()
    {
        
        foreach (Hardpoint h in hardpoints)
        {
            if (h.equipped != null)
            {
                ITargetLockWeapon[] lockon = h.equipped.GetComponents<ITargetLockWeapon>();

                if (lockon.Length > 0)
                {
                    foreach (ITargetLockWeapon wpn in lockon)
                    {
                        if (_lockOnTargeter.lockonTarget != null) wpn.Lock(_lockOnTargeter.lockonTarget);
                        else wpn.Unlock();
                    }
                }
            }
        }
    }

    private void CheckUnequipAction()
    {
        if (unequipAction.ReadValue<float>() > 0 && nearestEquipable == null && hardpoints[selectedHardpoint].equipped != null)
        {
            unequipTimer += Time.deltaTime;

            if (unequipTimer > unequipTime)
            {
                TryUnequipHardpoint(selectedHardpoint);
                unequipTimer = 0.0f;
            }
        }
        else unequipTimer = 0.0f;
    }

    private void CheckFiringAction()
    {
        if (fireAction.ReadValue<float>() > 0)
        {
            TryFireSelected();
        }
    }

    public void UpdateUI()
    {
        CheckEquipables();
        DisplaySelected();
        UpdateEquipmentUI();
    }

    private void UpdateEquipmentUI()
    {
        Color c_black = new Color(0, 0, 0, 0.5f);
        Color c_white = new Color(1, 1, 1, 0.25f);
        Color c_red = new Color(1, 0, 0, 0.95f);
        Color c_green = new Color(0, 1, 0, 0.95f);
        Color c_yellow = new Color(1, 1, 0, 0.95f);

        //loop though hardpointlist and update icon colors
        int i = 0;
        foreach (HardpointList.HardpointItem entry in _hardpointList.hardpointItems)
        {
            entry.number.color = c_black;
            entry.icon.transform.localScale = Vector3.one;

            if (hardpoints[i].equipped != null)
            {
                entry.icon.color = c_green;
                entry.icon.sprite = hardpoints[i].equipped.iconImage;
            }
            else
            {
                entry.icon.color = c_black;
                entry.icon.sprite = null;
            }

            if (nearestEquipable != null)
            {
                if (nearestEquipable.possibleLocations.Contains(hardpoints[i].location))
                {
                    entry.number.color = c_green;
                }
            }

            if (i == selectedHardpoint)
            {
                entry.bg.color = c_white;

                if (unequipTimer > 0.0f)
                {
                    entry.icon.color = c_red;
                    //shrink icon
                    entry.icon.transform.localScale = new Vector3(1.0f, Mathf.Lerp(1.0f, 0.0f, unequipTimer/unequipTime), 1.0f);
                }
            }
            else
            {
                entry.bg.color = c_black;
            }

            i++;
        }
    }

    private void DisplaySelected()
    {
        if (hardpoints[selectedHardpoint].equipped != null)
        {
            _currentEquipText.text = hardpoints[selectedHardpoint].equipped.equipableName;

            if (hardpoints[selectedHardpoint].equipped.GetComponent<Ranged_Weapon>() != null)
            {
                Ranged_Weapon wpn = hardpoints[selectedHardpoint].equipped.GetComponent<Ranged_Weapon>();
                int ammo = wpn.GetCurrentAmmo();
                float reloadTimeLeft = wpn.GetRemainingReloadTime();

                if (ammo > 0) {
                    _currentEquipText.text += "\nAMMO (" + hardpoints[selectedHardpoint].equipped.GetComponent<Ranged_Weapon>().GetCurrentAmmo() + ")";
                }
                else
                {
                    _currentEquipText.text += "\nRELOADING (" + reloadTimeLeft.ToString("0.0") + "s)";
                }
                
            }
        }
        else
        {
            _currentEquipText.text = "NONE SELECTED";
        }
    }

    public void ScrollHardpoints(InputAction.CallbackContext context){
        int copySHP = selectedHardpoint;

        if (context.performed)
        {
            if (context.ReadValue<float>() < 0)
            {
                copySHP--;
            }
            else
            {
                copySHP++;
            }

            if (copySHP < 0)
            {
                copySHP = hardpoints.Count - 1;
            }
            else if (copySHP >= hardpoints.Count)
            {
                copySHP = 0;
            }
        }

        SelectHardpoint(copySHP);
    }

    private void SelectHardpoint(InputAction.CallbackContext context){
        if (context.performed)
        {
            int index = hardpointSelectActions.IndexOf(context.action);
            SelectHardpoint(index);
        }
    }

    public void SelectHardpoint(int index){
        unequipTimer = 0.0f;
        selectedHardpoint = index;
        selectedHardpoint = Mathf.Clamp(selectedHardpoint, 0, hardpoints.Count - 1);
    }

    // private void TryFireSelected(InputAction.CallbackContext context){
    //     if (context.performed)
    //     {
    //         TryFireSelected();
    //     }
    // }

    public void TryFireSelected(){
        if (hardpoints[selectedHardpoint].equipped != null)
        {
            if (hardpoints[selectedHardpoint].equipped.type == Equipable.EquipableType.rangedWeapon)
            {
                Ranged_Weapon weapon = (Ranged_Weapon)hardpoints[selectedHardpoint].equipped.GetComponent<Ranged_Weapon>();
                Equipable eq = weapon.GetComponent<Equipable>();
                string name = eq.equipableName;

                List<Ranged_Weapon> weapons = new List<Ranged_Weapon>();
                //check other hardpoints for same name, if so, try fire
                foreach (Hardpoint h in hardpoints)
                {
                    if (h.equipped != null)
                    {
                        if (h.equipped.equipableName == name)
                        {
                            if (h.equipped.GetComponent<Ranged_Weapon>() != null)
                            {
                                weapons.Add(h.equipped.GetComponent<Ranged_Weapon>());
                            }
                        }
                    }
                }

                foreach (Ranged_Weapon w in weapons)
                {
                    w.TryShoot();
                }
            }
        }
    }

    private void TryEquipNearest(InputAction.CallbackContext context){
        if (context.performed)
        {
            if (nearestEquipable != null)
            {
                TryEquip(nearestEquipable, selectedHardpoint);
            }
        }
    }

    public void TryEquip(Equipable _equipable, int index){
        if ((index < 0 || index >= hardpoints.Count) || !_equipable.possibleLocations.Contains(hardpoints[index].location))
        {
            GetComponent<AudioSource>().PlayOneShot(failEquipSound);
            return;
        }

        if (hardpoints[index].equipped != null)
        {
            hardpoints[index].equipped.Unequip();
        }

        hardpoints[index].equipped = _equipable;
        _equipable.Equip(hardpoints[index].hardpoint);

        GetComponent<Animator>().SetTrigger("Equip");
        GetComponent<AudioSource>().PlayOneShot(equipSound);
    }

    public void TryUnequipHardpoint(int index) {
        if (hardpoints[index].equipped != null)
        {
            hardpoints[index].equipped.Unequip();
            hardpoints[index].equipped = null;
            GetComponent<Animator>().SetTrigger("Eject");
            GetComponent<AudioSource>().PlayOneShot(unequipSound);
        }
    }

    private void CheckEquipables()
    {
        Color c_yellow = new Color(1.0f, 1.0f, 0.0f, 0.95f);
        Color c_fadedYellow = new Color(1.0f, 1.0f, 0.0f, 0.1f);

        //check for any disabled or destroyed equipables
        RemoveInvalidEquipables();

        //sort equipables by distance
        //sort scrappables by distance
        allEquipables.Sort((x, y) => Vector3.Distance(transform.position, x.transform.position).CompareTo(Vector3.Distance(transform.position, y.transform.position)));

        nearestEquipable = null;
        Equipable tempNearest = null;
        _canEquipText.text = "[F] EQUIP ...";
        _canEquipText.color = c_fadedYellow;
        //check if any equipables are nearby
        foreach (Equipable eq in allEquipables)
        {

            if (eq.isEquipped)
            {
                continue;
            }

            if (tempNearest == null) tempNearest = eq;

            if (Vector3.Distance(eq.transform.position, transform.position) < equipDistance)
            {
                nearestEquipable = eq;
                _canEquipText.text = "[F] Equip " + eq.equipableName;
                _canEquipText.color = c_yellow;
                break;
            }
        }

        //update pos of icon on screen
        if (tempNearest != null && nearestEquipableIcon != null)
        {
            nearestEquipableIcon.gameObject.SetActive(true);

            //get dot
            Vector3 dir = tempNearest.transform.position - Camera.main.transform.position;
            float dot = Vector3.Dot(Camera.main.transform.forward, dir);

            //if forwards, set position on screen
            if (dot > 0)
            {
                nearestEquipableIcon.transform.position = Camera.main.WorldToScreenPoint(tempNearest.transform.position);
            }
            //if behind, disable
            else
            {
                nearestEquipableIcon.gameObject.SetActive(false);
            }
        }
        else
        {
            nearestEquipableIcon.gameObject.SetActive(false);
        }
    }

    private void RemoveInvalidEquipables()
    {
        for (int i = allEquipables.Count - 1; i >= 0; i--)
        {
            if (allEquipables[i] == null || !allEquipables[i].enabled)
            {
                allEquipables.RemoveAt(i);
            }
        }
    }

    public void AddEquipable(Equipable equipable)
    {
        //check if already exists
        if (allEquipables.Contains(equipable))
        {
            return;
        }

        allEquipables.Add(equipable);
    }

    public void RemoveEquipable(Equipable equipable)
    {
        if (allEquipables.Contains(equipable))
        {
            allEquipables.Remove(equipable);
        }
    }

    #if UNITY_EDITOR
    [CustomEditor(typeof(HardpointManager))]
    public class HardpointManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            HardpointManager manager = (HardpointManager)target;

            // //list of hardpoints
            // for (int i = 0; i < manager.hardpoints.Count; i++)
            // {
            //     manager.hardpoints[i].hardpoint = (Transform)EditorGUILayout.ObjectField("Hardpoint " + i, manager.hardpoints[i].hardpoint, typeof(Transform), true);
            //     manager.hardpoints[i].type = (HardpointType)EditorGUILayout.EnumPopup("Type", manager.hardpoints[i].type);
            // }
        }
    }
    #endif
}
