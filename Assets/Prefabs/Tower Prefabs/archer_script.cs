using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class archer_script : tower_base
{   
    
    public int damageUpgradeInerement = 1;
    public float attackRateUpgradePercentage = 0.15f;

    // Upgrade 1
    public void UpgradeDamage(Component sender, object data){
        Debug.Log("Upgrade Damage");
        if ((GameObject)data != gameObject){
            return;
        }

        damage += damageUpgradeInerement;
        upgrade1Cost += 5;
    }

    // Upgrade 2
    public void UpgradeAttackRate(Component sender, object data){
        Debug.Log("Upgrade Attack Rate");
        if ((GameObject)data != gameObject){
            return;
        }
        attackRate -= attackRate * attackRateUpgradePercentage;
        upgrade2Cost += 5;
    }
}
