using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class logic_manager : MonoBehaviour
{
    public int startingHealth;
    public int startingMoney;

    [SerializeField]
    private int currentHealth;
    [SerializeField]
    private int currentMoney;

    public GameEvent onEnemyKill;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = startingHealth;
        currentMoney = startingMoney;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // ----------------- Game Events -----------------
    public void UpdateHealth(Component sender, object data)
    {
        currentHealth += (int) data;
    }

    public void UpdateMoney(Component sender, object data)
    {
        currentMoney += (int) data;
    }
    
}
