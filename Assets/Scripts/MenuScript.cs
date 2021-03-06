using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        //unlock and unhide cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void QuitGame() {
        Application.Quit();
    }

    public void LoadMenu() {
        SceneManager.LoadScene(0);
    }

    public void LoadGame() {
        SceneManager.LoadScene(1);
    }

    public void LoadWin() {
        SceneManager.LoadScene(2);
    }

    public void LoadLose() {
        SceneManager.LoadScene(3);
    }
}
