using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System;

public class EnemyHealth : MonoBehaviour
{
    [Serializable]
    private class WeaponDrop
    {
        public GameObject weapon;
        public float chance;
    }

    public UnityEvent onDeath;

    public float startingHealth = 100f;
    float health;

    public Image healthBar;

    bool isDead = false;

    [Header("Drops")]
    public GameObject p_scrappablePrefab;
    [SerializeField] private List<WeaponDrop> weaponDrops = new List<WeaponDrop>();

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

    public void DropScrappable(){
        GameObject scrap = Instantiate(p_scrappablePrefab, transform.position, Quaternion.identity, null);
    }

    public void DropWeapons(){
        foreach (WeaponDrop drop in weaponDrops)
        {
            if (UnityEngine.Random.Range(0, 100) < drop.chance)
            {
                Instantiate(drop.weapon, transform.position, Quaternion.identity, null);
            }
        }
    }
}