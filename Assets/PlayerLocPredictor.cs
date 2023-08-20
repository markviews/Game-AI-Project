using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocPredictor : MonoBehaviour
{
    public Transform player;
    public VisibilityScript visScript;
    public Vector3 predictedPos;

    // Start is called before the first frame update
    void Start()
    {
        predictedPos = new Vector3();
    }

    // Update is called once per frame
    void Update()
    {
        if(visScript.isVisible(transform.position, player)){
            //Debug.Log("I can see the player!");
            predictedPos = player.position;
        }

        else {
            //Predict player location
        }
    }
}
