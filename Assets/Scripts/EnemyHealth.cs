using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System;

public class EnemyHealth : MonoBehaviour
{
    [Serializable]
    private class MinMax
    {
        [SerializeField] public int min = 0;
        [SerializeField] public int max = 1;
    }

    public UnityEvent onDeath;

    public float startingHealth = 100f;
    float health;

    public Image healthBar;

    bool isDead = false;

    [Header("Scrap Drops")]
    public GameObject p_scrapPrefab;
    [SerializeField] private MinMax m_scrapCount = new MinMax();
    [SerializeField] private MinMax m_scrapValue = new MinMax();

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

    public void DropScrap(){
        int realScrapCount = UnityEngine.Random.Range(m_scrapCount.min, m_scrapCount.max + 1);
        for (int i = 0; i < realScrapCount; i++)
        {
            GameObject scrap = Instantiate(p_scrapPrefab, transform.position, Quaternion.identity, null);
            scrap.GetComponent<ScrapPickup>().scrapValue = UnityEngine.Random.Range(m_scrapValue.min, m_scrapValue.max);
        }
    }
}