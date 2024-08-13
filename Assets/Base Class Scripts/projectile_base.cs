using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class projectile_base : MonoBehaviour
{   
    // these are set by the parent
    public GameObject target;
    public GameObject parentTower;

    // reference to the parent tower script
    public tower_base tower_script;

    // direction to the target
    private Vector3 direction;

    // Start is called before the first frame update
    void Start()
    {
        tower_script = parentTower.GetComponent<tower_base>();
        direction = target.transform.position - transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // move towards the target
        float distanceThisFrame = tower_script.projectileSpeed * Time.deltaTime;
        transform.Translate(direction.normalized * distanceThisFrame, Space.World);

        if(transform.position.x > Camera.main.orthographicSize * Camera.main.aspect || transform.position.x < -Camera.main.orthographicSize * Camera.main.aspect || transform.position.y > Camera.main.orthographicSize || transform.position.y < -Camera.main.orthographicSize){
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision){
        if(collision.gameObject.tag == "Enemy"){
            collision.gameObject.GetComponent<enemy_script>().health -= tower_script.damage;
            Destroy(gameObject);
        }
    }
}