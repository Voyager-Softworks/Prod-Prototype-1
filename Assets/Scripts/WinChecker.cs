using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.SceneManagement;

public class WinChecker : MonoBehaviour
{
    public List<EnemyHealth> enemies;
    public int killsNeeded = 3;
    public int killsAchieved = 0;

    public TextMeshProUGUI missionText;

    // Start is called before the first frame update
    void Start()
    {
        EnemyHealth[] tempEnemies = FindObjectsOfType<EnemyHealth>();
        foreach(EnemyHealth enemy in enemies){
            AddEnemy(enemy);
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateUI();
        CheckWin();
    }

    public void UpdateUI(){
        missionText.text = "KILL ENEMIES\n" + killsAchieved + "/" + killsNeeded;
    }

    public void CheckWin(){
        if(killsAchieved >= killsNeeded){
            Win();
        }
    }

    public void Win(){
        Debug.Log("WIN");
        SceneManager.LoadScene(0);
    }

    public void AddEnemy(EnemyHealth _enemy){
        if (!enemies.Contains(_enemy)){
            enemies.Add(_enemy);

            _enemy.onDeath.AddListener(() => {
                killsAchieved++;
                UpdateUI();
            });
        }
    }
}
