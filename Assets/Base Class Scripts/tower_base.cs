using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Base class for all towers
 * Contains all the basic tower stats and functions
 *
 * TODO: Decide what range indicator to use
 *
 *
 */


public class tower_base : MonoBehaviour
{
    //--------------Tower base stats--------------
    public float range;
    public float attackRate;
    public int damage;
    public float projectileSpeed;

    public int cost;
    public int upgrade1Cost;
    public int upgrade2Cost;

    public string upgrade1Name;
    public string upgrade2Name;

    //--------------Tower options--------------
    public enum TargetPriority { 
        First, 
        Last,
        Close,
        Strong
    };
    public TargetPriority priority;
    public bool showRange;
    public Color rangeColor = new Color(0, 0, 0, 0.1f);

    //--------------prefab references--------------
    public GameObject projectilePrefab;
    public GameObject target;
    public GameObject rangeIndicatorPrefab;

    public GameObject rangeIndicator;

    //--------------Private variables--------------
    private float timer = 0;



    // Start is called before the first frame update
    void Start()
    {   
        DrawRange(50, range + 1.5f);
        gameObject.GetComponent<LineRenderer>().enabled = false;
        //DrawRange2();
        //rangeIndicator.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // Find the target based on the priority
        switch(priority){
            case TargetPriority.First:
                target = FindFirstEnemy();
                break;
            case TargetPriority.Last:
                target = FindLastEnemy();
                break;
            case TargetPriority.Close:
                target = FindClosestEnemy();
                break;
            case TargetPriority.Strong:
                target = FindStrongestEnemy();
                break;
        }

        // Attack the target if it is in range
        if(timer < attackRate){
            timer += Time.deltaTime;
        } else if(target != null && isInRange(target)){
            Attack(target);
            timer = 0;
        } else {
            timer = 99;
        }

        /*
        // Attack the target if it is in range
        if(timer < attackRate){
            timer += Time.deltaTime;
        } else {
            //Debug.Log("Closest enemy is: " + Vector3.Distance(transform.position, target.transform.position) + " units away");
            if(target != null && isInRange(target)){
                //Debug.Log("Distance: " + Vector3.Distance(transform.position, target.transform.position));
                Attack(target);
            }
            timer = 0;
        }
        */
    }

    //--------------Tower QoL functions--------------
    void DrawRange(int steps, float radius)
    {
        LineRenderer line = gameObject.AddComponent<LineRenderer>();
        line.positionCount = steps;
        line.useWorldSpace = false;
        line.startWidth = 0.05f;
        line.endWidth = 0.05f;
        line.material.color = rangeColor;
 
        for(int currentStep=0; currentStep<steps; currentStep++)
        {
            float circumferenceProgress = (float)currentStep/(steps-1);
 
            float currentRadian = circumferenceProgress * 2 * Mathf.PI;
            
            float xScaled = Mathf.Cos(currentRadian);
            float yScaled = Mathf.Sin(currentRadian);
 
            float x = radius * xScaled;
            float y = radius * yScaled;
            float z = -1;
 
            Vector3 currentPosition = new Vector3(x,y,z);
 
            line.SetPosition(currentStep,currentPosition);
        }
    }

    
    void DrawRange2(){
        rangeIndicator = Instantiate(rangeIndicatorPrefab, transform.position, Quaternion.identity);
        rangeIndicator.transform.localScale = new Vector3(range * 2, range * 2, 1);
    }

    public void ToggleRange2(){
        rangeIndicator.SetActive(!rangeIndicator.activeSelf);
    }
    

    public void ToggleRange(){
        Debug.Log("Range: " + showRange);

        if(showRange){
            gameObject.GetComponent<LineRenderer>().enabled = false;
        } else {
            gameObject.GetComponent<LineRenderer>().enabled = true;
        }
        showRange = !showRange;
    }

    //--------------------Tower Targeting and Attacking--------------------
    public void SetPriority(int prio){
        // NEED TO ADD STRONG
        switch(prio){
            case 0:
                priority = TargetPriority.First;
                break;
            case 1:
                priority = TargetPriority.Last;
                break;
            case 2:
                priority = TargetPriority.Close;
                break;
            case 3:
                priority = TargetPriority.Strong;
                break;
            default:
                priority = TargetPriority.First;
                break;
        }
    }

    private GameObject FindClosestEnemy(){
        // Find all game objects with tag Enemy
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject closestEnemy = null;
        float minDistance = Mathf.Infinity;

        foreach(GameObject enemy in enemies){
            // Calculate the distance between the enemy and the archer
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            
            // If the enemy is within range, return the enemy
            if(distance < minDistance){
                closestEnemy = enemy;
                minDistance = distance;
            }
        }

        //Debug.Log("Closest enemy is: " + closestEnemy.name);
        return closestEnemy;
    }

    private GameObject FindStrongestEnemy(){
        // Find all game objects with tag Enemy
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject strongestEnemy = null;
        int maxHealth = 0;

        foreach(GameObject enemy in enemies){
            // If the enemy is within range, return the enemy
            if(isInRange(enemy) && enemy.GetComponent<enemy_script>().health > maxHealth){
                strongestEnemy = enemy;
                maxHealth = enemy.GetComponent<enemy_script>().health;
            }
        }

        //Debug.Log("Strongest enemy is: " + strongestEnemy.name);
        return strongestEnemy;
    }

    private GameObject FindFirstEnemy(){
        // Find all game objects with tag Enemy
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject firstEnemy = null;
        float maxProgress = 0;

        foreach(GameObject enemy in enemies){
            // If the enemy is within range, return the enemy
            if(isInRange(enemy) && enemy.GetComponent<enemy_script>().progress > maxProgress){
                firstEnemy = enemy;
                maxProgress = enemy.GetComponent<enemy_script>().progress;
                break;
            }
        }

        //Debug.Log("First enemy is: " + firstEnemy.name);
        return firstEnemy;
    }

    private GameObject FindLastEnemy(){
        // Find all game objects with tag Enemy
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject lastEnemy = null;
        float minProgress = Mathf.Infinity;

        foreach(GameObject enemy in enemies){
            // If the enemy is within range, return the enemy
            if(isInRange(enemy) && enemy.GetComponent<enemy_script>().progress < minProgress){
                lastEnemy = enemy;
                minProgress = enemy.GetComponent<enemy_script>().progress;
            }
        }

        //Debug.Log("Last enemy is: " + lastEnemy.name);
        return lastEnemy;
    }

    private bool isInRange(GameObject target){
        //Debug.Log("Distance: " + Vector3.Distance(transform.position, target.transform.position));
        return Vector3.Distance(transform.position, target.transform.position) < range;
    }

    private void Attack(GameObject target){
        //Debug.Log("Attacking " + target.name);

        // Calculate the direction to the target
        Vector2 direction = target.transform.position - transform.position;
        direction.Normalize();  // Normalize to get a unit vector
        
        // Calculate the angle in radians
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 180;

        // Create a rotation that faces the target
        Quaternion rotation = Quaternion.Euler(0, 0, angle);

        // Create arrow
        GameObject arrow = Instantiate(projectilePrefab, transform.position, rotation);

        // Set the instantiator reference and target
        arrow_script arrow_scr = arrow.GetComponent<arrow_script>();
        arrow_scr.parentTower = gameObject;
        arrow_scr.target = target;
    }

    public void damageEnemy(GameObject target){
        target.GetComponent<enemy_script>().health -= damage;
    }
}
