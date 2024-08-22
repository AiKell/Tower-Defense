using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Unity.VisualScripting;
using UnityEngine.Tilemaps;

// maybe use this later
interface IInteractable {
    public void Interact();
}

public class ui_manager : MonoBehaviour
{
    // Object references
    //public GameObject towerPrefab;
    public GameObject indicator;
    public GameObject rangeIndicator;

    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI healthText;

    public GameObject towerPanel;
    public GameObject shopPanel;

    public TextMeshProUGUI upgrade1Text;
    public TextMeshProUGUI upgrade2Text;

    public TextMeshProUGUI upgrade1Value;
    public TextMeshProUGUI upgrade2Value;
    public TextMeshProUGUI rangeValue;

    public GameEvent onTowerSelect;
    public GameEvent onGoldChange;
    public GameEvent onWaveSend;
    
    public GameEvent onTowerDamageUpgrade;
    public GameEvent onTowerAttackRateUpgrade;

    private int numSpawned = 0;


    //debug stuff
    public TMP_InputField waveInput;
    public GameEvent onWaveSet;

    // Start is called before the first frame update
    void Start()
    {
        indicator.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {   

        //----------------- Read Mouse Clicks -----------------
        if (Input.GetMouseButtonDown(0))
        {
            //Debug.Log("Mouse clicked at: " + Camera.main.ScreenToWorldPoint(Input.mousePosition));
            // see if the click was on a tower
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            // if the hit object is a tower, show the upgrade panel
            if (hit.collider != null && hit.collider.gameObject.tag == "Tower")
            {
                //Debug.Log("Clicked on " + hit.collider.gameObject.name);
                ToggleInfoPanel(hit.collider.gameObject);
                onTowerSelect.Raise(this, hit.collider.gameObject);
            }

            // Maybe add something to deselect the tower if the user clicks off of it
            // Having problems becuase if the user clicks on a button or the panel it will incorrectly deselect the tower
        }
    }

    // ----------------- Game Event Listeners -----------------
    public void UpdateMoney(Component sender, object data)
    {
        Debug.Log("updating money");
        moneyText.text = (Int32.Parse(moneyText.text) + (int) data).ToString();
    }

    public void UpdateHealth(Component sender, object data)
    {
        Debug.Log("updating health");
        healthText.text = (Int32.Parse(healthText.text) + (int) data).ToString();
    }

    // ----------------- Tower Upgrade Panel Functions-----------------
    private GameObject selectedTower;
    public void ToggleInfoPanel(GameObject tower)
    {
        //Debug.Log("Toggle Info Panel");
        if(tower == selectedTower){
            towerPanel.SetActive(false);
            selectedTower = null;
            return;
        }

        // Set tower and panel as active
        towerPanel.SetActive(true);
        selectedTower = tower;

        // Update displayed stats
        UpdateDisplayedStats();
        
        UpdatePanelPositions();
    }

    public void ToggleRange()
    {
        selectedTower.GetComponent<tower_base>().ToggleRange();
    }

    public void SelectPriority(int priority)
    {
        Debug.Log("Priority: " + priority);
        selectedTower.GetComponent<tower_base>().SetPriority(priority);
    }

    public void UpgradeDamage()
    {
        if(Int32.Parse(moneyText.text) < selectedTower.GetComponent<tower_base>().upgrade1Cost){
            Debug.Log("Not enough money");
            return;
        }
        onGoldChange.Raise(this, -selectedTower.GetComponent<tower_base>().upgrade1Cost);
        onTowerDamageUpgrade.Raise(this, selectedTower);
        UpdateDisplayedStats();
    }

    public void UpgradeAttackRate()
    {
        if(Int32.Parse(moneyText.text) < selectedTower.GetComponent<tower_base>().upgrade2Cost){
            Debug.Log("Not enough money");
            return;
        }
        onGoldChange.Raise(this, -selectedTower.GetComponent<tower_base>().upgrade2Cost);
        onTowerAttackRateUpgrade.Raise(this, selectedTower);
        UpdateDisplayedStats();
    }

    private void UpdatePanelPositions()
    {
        if (shopPanel.activeSelf && towerPanel.activeSelf)
        {
            shopPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -150);
            towerPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -18);
        }
        else if (shopPanel.activeSelf)
        {
            shopPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -150);
        }
        else if (towerPanel.activeSelf)
        {
            towerPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -150);
        }
    }

    private void UpdateDisplayedStats(){
        // Update Text
        upgrade1Text.text = selectedTower.GetComponent<tower_base>().upgrade1Name;
        upgrade2Text.text = selectedTower.GetComponent<tower_base>().upgrade2Name;

        // Update values
        upgrade1Value.text = selectedTower.GetComponent<tower_base>().damage.ToString();
        upgrade2Value.text = selectedTower.GetComponent<tower_base>().attackRate.ToString();
        rangeValue.text = selectedTower.GetComponent<tower_base>().range.ToString();
    }

    // ----------------- UI Top Level Buttons -----------------
    public void ToggleShopPanel()
    {
        shopPanel.SetActive(!shopPanel.activeSelf);
        UpdatePanelPositions();
    }

    public void OnSendWaveButtonClick()
    {
        onWaveSend.Raise(this, null);
    }

    public void OnSetWave(string wave)
    {
        //Debug.Log(waveInput.text);
        bool success = int.TryParse(waveInput.text, out int number);

        if (success)
        {
            onWaveSet.Raise(this, number);
        }
        else
        {
            Debug.LogWarning("Invalid wave input");
        }
    }

    // ----------------- Tower Placement -----------------
    public void OnTowerPlaceButtonClick(GameObject towerPrefab)
    {
        StartCoroutine(PlaceTower(towerPrefab));
    }

    private IEnumerator PlaceTower(GameObject towerPrefab)
    {   
        indicator.SetActive(true);
        rangeIndicator.SetActive(true);
        rangeIndicator.transform.localScale = new Vector3(towerPrefab.GetComponent<tower_base>().range * 2, towerPrefab.GetComponent<tower_base>().range * 2, 1);
        Vector3 mousePos;

        // Wait until the user clicks the left mouse button
        while (!Input.GetMouseButtonDown(0))
        {
            mousePos = Input.mousePosition;
            mousePos.z = 10;

            indicator.transform.position = Camera.main.ScreenToWorldPoint(mousePos);
            rangeIndicator.transform.position = Camera.main.ScreenToWorldPoint(mousePos);

            yield return null;

            if(Input.GetMouseButtonDown(1)){
                indicator.SetActive(false);
                rangeIndicator.SetActive(false);

                yield break;
            }
        }

        // Get the position of the click
        mousePos = Input.mousePosition;
        mousePos.z = 10; // You might need to adjust this value based on your camera setup
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);

        // Check if the tower is being placed on a valid tile ie. not on the top tilemap
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);
        if (hit.collider != null && (hit.collider.gameObject.tag == "Invalid Ground" || hit.collider.gameObject.tag == "Tower"))
        {
            Debug.Log("Invalid placement");
            indicator.gameObject.SetActive(false);
            rangeIndicator.gameObject.SetActive(false);
            yield break;
        }

        // Check if the user has enough money to place the tower
        if(Int32.Parse(moneyText.text) < towerPrefab.GetComponent<tower_base>().cost){
            Debug.Log("Not enough money");
            indicator.gameObject.SetActive(false);
            rangeIndicator.gameObject.SetActive(false);
            yield break;
        }
        onGoldChange.Raise(this, -towerPrefab.GetComponent<tower_base>().cost);

        

        // Instantiate the tower at the clicked position
        Instantiate(towerPrefab, worldPos, Quaternion.identity);
        towerPrefab.name = "Tower " + numSpawned++;

        indicator.SetActive(false);
        rangeIndicator.SetActive(false);
    }
}
