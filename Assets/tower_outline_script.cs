using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tower_outline_script : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
    }

    // move the outline to the tower's position
    public void SetPosition(Component sender, object data)
    {   
        GameObject tower = (GameObject)data;
        Vector3 newPosition = tower.transform.position;

        // if the tower clicked is the same as the one that was clicked before, hide the outline
        if(gameObject.transform.position == newPosition && gameObject.GetComponent<SpriteRenderer>().enabled == true){
            gameObject.GetComponent<SpriteRenderer>().enabled = false;
            return;
        }

        // move the outline to the tower's position
        transform.position = newPosition;
        gameObject.GetComponent<SpriteRenderer>().enabled = true;
    }
}
