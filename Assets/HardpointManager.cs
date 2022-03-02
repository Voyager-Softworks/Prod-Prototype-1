using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
using UnityEditor;
#endif

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
    [SerializeField] public List<Equipable> equipables = new List<Equipable>();

    public LockOnTargeter _lockOnTargeter;

    public TextMeshProUGUI _canEquipText;
    public TextMeshProUGUI _currentEquipText;
    public HardpointList _hardpointList;
    private Equipable nearestEquipable = null;
    public int selectedHardpoint = 0;
    public float unequipTime = 1.0f;
    private float unequipTimer = 0.0f;

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
    }

    // Update is called once per frame
    void Update()
    {
        UpdateUI();
        CheckFiring();
        CheckUnequip();
        UpdateLockon();

    }

    public void UpdateLockon()
    {
        if (_lockOnTargeter.lockonTarget != null)
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
                            wpn.Lock(_lockOnTargeter.lockonTarget);
                        }
                    }
                }
            }
        }
    }

    private void CheckUnequip()
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

    private void CheckFiring()
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
        //loop though hardpointlist and update icon colors
        int i = 0;
        foreach (HardpointList.HardpointItem entry in _hardpointList.hardpointItems)
        {
            entry.number.color = Color.red;
            entry.icon.transform.localScale = Vector3.one;

            if (hardpoints[i].equipped != null)
            {
                entry.icon.color = Color.green;
            }
            else
            {
                entry.icon.color = Color.black;
            }

            if (nearestEquipable != null)
            {
                if (nearestEquipable.possibleLocations.Contains(hardpoints[i].location))
                {
                    entry.number.color = Color.green;
                }
            }

            if (i == selectedHardpoint)
            {
                entry.bg.color = Color.yellow;

                if (unequipTimer > 0.0f)
                {
                    entry.icon.color = Color.red;
                    //shrink icon
                    entry.icon.transform.localScale = new Vector3(1.0f, Mathf.Lerp(1.0f, 0.0f, unequipTimer/unequipTime), 1.0f);
                }
            }
            else
            {
                entry.bg.color = Color.white;
            }

            i++;
        }
    }

    private void DisplaySelected()
    {
        if (hardpoints[selectedHardpoint].equipped != null)
        {
            _currentEquipText.text = hardpoints[selectedHardpoint].equipped.name;
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

                if (weapon != null)
                {
                    weapon.TryShoot();
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
        if (index < 0 || index >= hardpoints.Count)
        {
            return;
        }

        if (!_equipable.possibleLocations.Contains(hardpoints[index].location))
        {
            return;
        }

        if (hardpoints[index].equipped != null)
        {
            hardpoints[index].equipped.Unequip();
        }

        hardpoints[index].equipped = _equipable;
        _equipable.Equip(hardpoints[index].hardpoint);
    }

    public void TryUnequipHardpoint(int index) {
        if (hardpoints[index].equipped != null)
        {
            hardpoints[index].equipped.Unequip();
            hardpoints[index].equipped = null;
        }
    }

    private void CheckEquipables()
    {
        //sort equipables by distance
        equipables.Sort((x, y) => Vector3.Distance(transform.position, x.transform.position).CompareTo(Vector3.Distance(transform.position, y.transform.position)));

        nearestEquipable = null;
        _canEquipText.text = "";
        //check if any equipables are nearby
        foreach (Equipable eq in equipables)
        {

            if (eq.isEquipped)
            {
                continue;
            }

            if (Vector3.Distance(eq.transform.position, transform.position) < 10.0f)
            {
                nearestEquipable = eq;
                _canEquipText.text = "[F] Equip " + eq.name;
                break;
            }
        }
    }

    public void AddEquipable(Equipable equipable)
    {
        if (equipables.Contains(equipable))
        {
            return;
        }

        equipables.Add(equipable);
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
