using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//class that player can interact with to consume this gameobject for scrap
public class Scrappable : MonoBehaviour
{
    [Serializable]
    private class MinMax
    {
        [SerializeField] public int min = 0;
        [SerializeField] public int max = 1;
    }
    [SerializeField] private MinMax m_scrapCount = new MinMax();
    [SerializeField] private MinMax m_scrapValue = new MinMax();

    public bool disableOnScrap = true;
    public bool destroyOnScrap = true;

    public GameObject p_scrapPrefab;

    private ScrapManager _scrapManager;

    // Start is called before the first frame update
    void Start()
    {
        if (_scrapManager == null)
        {
            _scrapManager = FindObjectOfType<ScrapManager>();
        }

        if (_scrapManager != null)
        {
            _scrapManager.AddScrappable(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ScrapObject(){
        int realScrapCount = UnityEngine.Random.Range(m_scrapCount.min, m_scrapCount.max + 1);
        for (int i = 0; i < realScrapCount; i++)
        {
            GameObject scrap = Instantiate(p_scrapPrefab, transform.position, Quaternion.identity, null);
            scrap.GetComponent<ScrapPickup>().scrapValue = UnityEngine.Random.Range(m_scrapValue.min, m_scrapValue.max);
        }

        if (destroyOnScrap)
        {
            Destroy(gameObject);
        }

        if (disableOnScrap)
        {
            _scrapManager.RemoveScrappable(this);
            this.enabled = false;
        }
    }
}
