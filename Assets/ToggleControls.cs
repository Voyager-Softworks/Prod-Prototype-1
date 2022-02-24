using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class ToggleControls : MonoBehaviour
{
    public InputAction toggleAction;

    public GameObject o_controls;

    // Start is called before the first frame update
    void Start()
    {
        toggleAction.Enable();

        toggleAction.performed += ToggleControlsUI;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToggleControlsUI(InputAction.CallbackContext context)
    {
        Debug.Log("Toggle Controls UI");

        o_controls.SetActive(!o_controls.activeSelf);
    }
}
