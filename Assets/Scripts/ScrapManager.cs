using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;

//manages scrap values, usgae, and UI
public class ScrapManager : MonoBehaviour
{
    public InputAction scrapItemAction;
    public InputAction toggleAutoHealAction;
    public InputAction healAction;

    public int maxScrap = 100;
    public int currentScrap = 10;
    public bool autoHeal = false;
    public float scrapToHealthRate = 1.0f;

    public List<Scrappable> allScrapables = new List<Scrappable>();

    public TextMeshProUGUI _canScrapText;
    public TextMeshProUGUI _scrapInfoText;
    public TextMeshProUGUI _scrapText;
    public TextMeshProUGUI _autoHealText;

    public Image nearestScrappableIcon;

    public Scrappable nearestScrappable;
    public float scrapTime = 1.0f;
    private float scrapTimer = 0.0f;
    public float scrapDistance = 10.0f;

    public AudioClip scrapSound;
    public AudioClip healSound;
    public AudioClip failHealSound;
    public AudioClip collectScrapSound;

    private ShipHealth shipHealth;

    // Start is called before the first frame update
    void Start()
    {
        scrapItemAction.Enable();
        toggleAutoHealAction.Enable();
        healAction.Enable();

        toggleAutoHealAction.performed += ToggleAutoHeal;
        healAction.performed += TryHeal;

        //add all scrapables to list
        foreach (Scrappable s in FindObjectsOfType<Scrappable>())
        {
            AddScrappable(s);
        }

        if (shipHealth == null)
        {
            shipHealth = GetComponent<ShipHealth>();
        }

        if (nearestScrappableIcon == null)
        {
            nearestScrappableIcon = GameObject.Find("NearestScrappableIcon").GetComponent<Image>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        CheckScrappables();
        CheckScrapAction();
        UpdateUI();
    }

    public void UpdateUI(){
        Color c_red = new Color(1.0f, 0.0f, 0.0f, 0.5f);
        Color c_fadedRed = new Color(1.0f, 0.0f, 0.0f, 0.1f);
        Color c_yellow = new Color(1.0f, 1.0f, 0.0f, 0.95f);
        Color c_fadedYellow = new Color(1.0f, 1.0f, 0.0f, 0.1f);
        Color c_green = new Color(0.0f, 1.0f, 0.0f, 0.5f);

        _scrapText.text = currentScrap.ToString();

        _scrapInfoText.text = "SCRAPPING\nNOTHING";
        _scrapInfoText.color = c_fadedRed;

        _canScrapText.text = "[G] SCRAP ...";
        _canScrapText.color = c_fadedYellow;

        if (nearestScrappable != null)
        {
            _canScrapText.text = "[G] SCRAP " + nearestScrappable.scrapName;
            _canScrapText.color = c_yellow;

            if (scrapTimer > 0)
            {
                _scrapInfoText.text = "SCRAPPING\n" + nearestScrappable.scrapName + " " + ((scrapTimer / scrapTime) * 100).ToString("0") + "%";
                _scrapInfoText.color = c_green;
            }
        }

        if (autoHeal)
        {
            _autoHealText.text = "AUTO HEAL\nON";
            _autoHealText.color = c_green;
        }
        else
        {
            _autoHealText.text = "AUTO HEAL\nOFF";
            _autoHealText.color = c_fadedRed;
        }
    }

    private void ToggleAutoHeal(InputAction.CallbackContext context){
        if (context.performed){
            ToggleAutoHeal();
        }
    }

    public void ToggleAutoHeal(){
        SetAutoHeal(!autoHeal);
    }

    public void SetAutoHeal(bool _autoHeal){
        autoHeal = _autoHeal;
    }

    private void TryHeal(InputAction.CallbackContext context){
        if (context.performed){
            TryHeal();
        }
    }

    public void TryHeal(){
        //use scrap to heal
        float healthNeededToFull = shipHealth.maxHealth - shipHealth.currentHealth;
        int scrapNeededToFull = (int)(Mathf.Ceil(healthNeededToFull / scrapToHealthRate));
        scrapNeededToFull = Mathf.Max(scrapNeededToFull, 0);

        int scrapToUse = Mathf.Min(currentScrap, scrapNeededToFull);
        currentScrap = currentScrap - scrapToUse;

        shipHealth.AddHealth(scrapToUse * scrapToHealthRate);

        if (scrapToUse > 0){
            GetComponent<Animator>().SetTrigger("Repair");
            GetComponent<AudioSource>().PlayOneShot(healSound);
        }
        else{
            GetComponent<AudioSource>().PlayOneShot(failHealSound);
        }
    }

    public void CheckScrapAction()
    {
        if (scrapItemAction.ReadValue<float>() > 0 && nearestScrappable != null)
        {
            scrapTimer += Time.deltaTime;

            if (scrapTimer >= scrapTime)
            {
                scrapTimer = 0.0f;
                TryScrapNearest();
            }
        }
        else scrapTimer = 0.0f;
    }

    public void TryScrapNearest()
    {
        if (nearestScrappable != null)
        {
            TryScrap(nearestScrappable);
        }
    }

    public void TryScrap(Scrappable s)
    {
        if (s != null)
        {
            s.ScrapObject();
            GetComponent<Animator>().SetTrigger("Scrap");
            GetComponent<AudioSource>().PlayOneShot(scrapSound);
        }
    }


    //copy CheckEquipables() from HardpointManager.cs
    public void CheckScrappables()
    {

        //check for any disabled or destroyed scrapables
        RemoveInvalidScrappables();

        //sort scrappables by distance
        allScrapables.Sort(delegate (Scrappable a, Scrappable b)
        {
            return Vector3.Distance(transform.position, a.transform.position).CompareTo(Vector3.Distance(transform.position, b.transform.position));
        });

        nearestScrappable = null;
        //check if any scrapables are in range
        foreach (Scrappable s in allScrapables)
        {
            if (Vector3.Distance(transform.position, s.transform.position) <= scrapDistance)
            {
                nearestScrappable = s;
                break;
            }
        }

        //update pos of icon on screen
        if (allScrapables.Count > 0 && nearestScrappableIcon != null)
        {
            nearestScrappableIcon.gameObject.SetActive(true);

            Scrappable eq = allScrapables[0];

            //get dot
            Vector3 dir = eq.transform.position - Camera.main.transform.position;
            float dot = Vector3.Dot(Camera.main.transform.forward, dir);

            //if forwards, set position on screen
            if (dot > 0)
            {
                nearestScrappableIcon.transform.position = Camera.main.WorldToScreenPoint(eq.transform.position);
            }
            //if behind, disable
            else
            {
                nearestScrappableIcon.gameObject.SetActive(false);
            }
        }
        else
        {
            nearestScrappableIcon.gameObject.SetActive(false);
        }
    }

    public void AddScrap(int _scrap)
    {
        if (autoHeal){
            float autoHealScrapRate = scrapToHealthRate * 0.5f;

            float healthNeededToFull = shipHealth.maxHealth - shipHealth.currentHealth;
            int scrapNeededToFull = (int)(Mathf.Ceil(healthNeededToFull / autoHealScrapRate));
            scrapNeededToFull = Mathf.Max(scrapNeededToFull, 0);

            int scrapToUse = Mathf.Min(_scrap, scrapNeededToFull);
            _scrap = _scrap - scrapToUse;

            shipHealth.AddHealth(scrapToUse * autoHealScrapRate);
        }


        currentScrap += _scrap;
        if (currentScrap > maxScrap)
        {
            currentScrap = maxScrap;
        }
        else{
            GetComponent<AudioSource>().PlayOneShot(collectScrapSound);
        }
    }

    private void RemoveInvalidScrappables()
    {
        for (int i = allScrapables.Count - 1; i >= 0; i--)
        {
            if (allScrapables[i] == null || !allScrapables[i].enabled)
            {
                allScrapables.RemoveAt(i);
            }
        }
    }

    public void AddScrappable(Scrappable s){
        //check if already exists
        if (allScrapables.Contains(s)){
            return;
        }

        allScrapables.Add(s);
    }

    public void RemoveScrappable(Scrappable s){
        if (allScrapables.Contains(s)){
            allScrapables.Remove(s);
        }
    }
}
