using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocPredictor : MonoBehaviour
{
    public Transform player;
    public VisibilityScript visScript;
    public Vector3 predictedPos;
    private BoxCollider2D boxCol;

    // Start is called before the first frame update
    void Start()
    {
        predictedPos = new Vector3();
        boxCol = gameObject.GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        boxCol.enabled = false;
        if(visScript.isVisible(transform.position, player)){
            //Debug.Log("I can see the player!");
            predictedPos = player.position;
            boxCol.enabled = true;
            return;
        }
        boxCol.enabled = true;

        predictedPos = visScript.closestHiddenNode(predictedPos, gameObject.transform);
    }
}
