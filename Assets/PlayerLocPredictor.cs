using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocPredictor : MonoBehaviour
{
    public Transform player;
    public VisibilityScript visScript;
    public Vector3 predictedPos;
    private CircleCollider2D circleCol;

    // Start is called before the first frame update
    void Start()
    {
        predictedPos = new Vector3();
        circleCol = gameObject.GetComponent<CircleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        circleCol.enabled = false;
        if(visScript.isVisible(transform.position, player)){
            //Debug.Log("I can see the player!");
            predictedPos = player.position;
            circleCol.enabled = true;
            return;
        }
        circleCol.enabled = true;

        predictedPos = visScript.closestHiddenNode(predictedPos, gameObject.transform);
    }
}
