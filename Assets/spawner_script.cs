using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class wave_manager : MonoBehaviour
{   
    public List<Wave_base> waves;
    private int currentWave = 0;

    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(SendWave());
        //currentWave++;
    }

    public void setWave(Component Sender, object data){
        currentWave = (int) data - 1;
        //Debug.Log("wave set to:" + currentWave);
    }

    public void SendNextWave(){
        //Debug.Log("Send wave step 2");
        StartCoroutine(SendWave());
        currentWave++;
    }

    IEnumerator SendWave(){
        foreach(group enemy_group in waves[currentWave].enemies){
            StartCoroutine(SpawnGroup(enemy_group.enemy, enemy_group.internal_delay, enemy_group.num_enemies));
            yield return new WaitForSeconds(enemy_group.external_delay);
        }
    }

    IEnumerator SpawnGroup(GameObject enemy, float delay, float num){
        for(int i=0;i<num;i++){
            Instantiate(enemy, transform.position, transform.rotation);
            yield return new WaitForSeconds(delay);
        }
    }
}