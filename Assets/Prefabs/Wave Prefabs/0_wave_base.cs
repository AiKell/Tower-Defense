using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct group{
    public GameObject enemy;
    public int num_enemies;
    public float internal_delay;
    public float external_delay;
}

public class Wave_base : MonoBehaviour
{   
    public List<group> enemies; // GameObject is the enemy prefab, float is the delay until the next enemy spawns, int is num times to repeat
}
