using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ShipHealth : MonoBehaviour
{
    public float maxHealth = 100;
    public float currentHealth = 100;
    public bool isDead = false;

    public Image healthBar;
    public TextMeshProUGUI healthText;

    private float deathTimer = 0.0f;
    private float deathTime = 3.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckHealth();

        UpdateUI();
    }

    private void CheckHealth()
    {
        CheckForDeath();

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }

    private void CheckForDeath()
    {
        if ((!isDead && currentHealth <= 0) || (isDead && currentHealth > 0))
        {
            Die();
        }

        if (isDead){
            deathTimer += Time.deltaTime;
            if (deathTimer >= deathTime){
                SceneManager.LoadScene("Lose");
            }
        }
    }
    public void UpdateUI(){
        healthBar.rectTransform.sizeDelta = new Vector2((currentHealth / maxHealth) * 500 - 510, healthBar.rectTransform.sizeDelta.y);
        healthText.text = (currentHealth / maxHealth * 100).ToString("0") + "%";
    }

    public void AddHealth(float _amount){
        currentHealth += _amount;

        if (currentHealth > maxHealth){
            currentHealth = maxHealth;
        }
    }

    public void TakeDamage(float _dmg){
        if (isDead) return;

        currentHealth -= _dmg;
        if(currentHealth <= 0){
            Die();
        }
    }

    public void Die(){
        isDead = true;
        currentHealth = 0;

        GetComponent<ShipMovement>().enabled = false;
        GetComponent<ShipWeapons>().c_hardpointManager.enabled = false;
    }

    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.tag == "EnemyBullet"){
            TakeDamage(1);
        }
    }
}
