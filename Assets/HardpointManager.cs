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
    public enum HardpointType
    {
        side_left,
        side_right,
        bottom
    }

    [Serializable]
    public class Hardpoint
    {
        [SerializeField] public Transform hardpoint = null;
        [SerializeField] public HardpointType type = HardpointType.side_left;
        [SerializeField] public Equipable equipped = null;
    }

    public InputAction fireAction;
    public InputAction equipAction;
    public InputAction scrollAction;
    public List<InputAction> hardpointSelectActions = new List<InputAction>();

    [SerializeField] public List<Hardpoint> hardpoints = new List<Hardpoint>();
    [SerializeField] public List<Equipable> equipables = new List<Equipable>();

    public TextMeshProUGUI _canEquipText;
    public HardpointList _hardpointList;
    private Equipable nearestEquipable = null;
    public int selectedHardpoint = 0;

    // Start is called before the first frame update
    void Start()
    {
        fireAction.Enable();
        equipAction.Enable();
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

        if (fireAction.ReadValue<float>() > 0)
        {
            TryFireSelected();
        }
    }

    public void TryEquipNearest(InputAction.CallbackContext context){
        if (context.performed)
        {
            if (nearestEquipable != null)
            {
                TryEquip(nearestEquipable, selectedHardpoint);
            }
        }

        
    }

    public void UpdateUI(){
        CheckEquipables();

        //loop though hardpointlist and update icon colors
        int i = 0;
        foreach (HardpointList.HardpointItem entry in _hardpointList.hardpointItems)
        {
            if (i == selectedHardpoint)
            {
                entry.bg.color = Color.red;
            }
            else{
                entry.bg.color = Color.white;
            }
            
            if (hardpoints[i].equipped != null)
            {
                entry.icon.color = Color.green;
            }
            else
            {
                entry.icon.color = Color.black;
            }
            i++;
        }
    }

    public void ScrollHardpoints(InputAction.CallbackContext context){
        if (context.performed)
        {
            if (context.ReadValue<float>() < 0)
            {
                selectedHardpoint--;
            }
            else
            {
                selectedHardpoint++;
            }

            if (selectedHardpoint < 0)
            {
                selectedHardpoint = hardpoints.Count - 1;
            }
            else if (selectedHardpoint >= hardpoints.Count)
            {
                selectedHardpoint = 0;
            }
        }
    }

    private void SelectHardpoint(InputAction.CallbackContext context){
        if (context.performed)
        {
            int index = hardpointSelectActions.IndexOf(context.action);
            SelectHardpoint(index);
        }
    }

    public void SelectHardpoint(int index){
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

    public void TryEquip(Equipable _equipable, int index){
        if (index < 0 || index >= hardpoints.Count)
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
