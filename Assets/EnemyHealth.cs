using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyHealth : MonoBehaviour
{
    public UnityEvent onDeath;

    public float startingHealth = 100f;

    bool isDead = false;

    // Start is called before the first frame update
    void Start()
    {
        WinChecker winChecker = FindObjectOfType<WinChecker>();

        if (winChecker != null)
        {
            winChecker.AddEnemy(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(float _damage){
        if (isDead) return;

        startingHealth -= _damage;
        if(startingHealth <= 0){
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