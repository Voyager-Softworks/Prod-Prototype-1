using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    public UnityEvent onDeath;

    public float startingHealth = 100f;
    float health;

    public Image healthBar;

    bool isDead = false;

    // Start is called before the first frame update
    void Start()
    {
        WinChecker winChecker = FindObjectOfType<WinChecker>();
        health = startingHealth;
        if (winChecker != null)
        {
            winChecker.AddEnemy(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        healthBar.fillAmount = health / startingHealth;
    }

    public void TakeDamage(float _damage){
        if (isDead) return;

        health -= _damage;
        if(health <= 0){
            Die();
        }
    }

    public void Die(){
        if (isDead) return;

        isDead = true;
        onDeath.Invoke();
        Destroy(gameObject, 1.0f);
        
    }
}