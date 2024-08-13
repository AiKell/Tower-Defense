using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class enemy_base : MonoBehaviour
{
    //-----------------Enemy stats-----------------
    public float speed;
    public int health;
    public int goldValue;
    public int damage;

    //-----------------Spline info-----------------
    private SplineContainer splineContainer;
    private float splineLength;
    public float progress = 0f;

    //--------------GameEvents--------------
    public GameEvent onGoldChange;
    public GameEvent onHealthChange;

    // Start is called before the first frame update
    void Start()
    {
        splineContainer = FindObjectOfType<SplineContainer>();
        splineLength = splineContainer.CalculateLength();
    }

    // Update is called once per frame
    void Update()
    {
        // Move enemy along the spline
        progress += speed * Time.deltaTime / splineLength;
        progress = Mathf.Clamp01(progress);
        Vector3 currentPosition = splineContainer.EvaluatePosition(progress);
        transform.position = currentPosition;

        // Check if enemy finished the track
        if (progress >= 1f){
            // Subtract health from player USE EVENT SYSTEM
            onHealthChange.Raise(this, -damage);

            // Destroy enemy
            Destroy(gameObject);
        }

        // Check if enemy was killed
        if (health <= 0){
            // Add gold to player USE EVENT SYSTEM
            onGoldChange.Raise(this, goldValue);

            // Destroy enemy
            Destroy(gameObject);
        }
    }
}
