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

    public void DisableInput()
    {
        toggleAction.Disable();
    }

    private void OnDestroy() {
        DisableInput();
    }

    public void ToggleControlsUI(InputAction.CallbackContext context)
    {
        Debug.Log("Toggle Controls UI");

        o_controls.SetActive(!o_controls.activeSelf);

        //if open, unlock and unhiude cursor
        if (o_controls.activeSelf)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else 
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
