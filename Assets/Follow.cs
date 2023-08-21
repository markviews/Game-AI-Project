using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Follow : MonoBehaviour {

    public bool memory = true;
    public Transform player;
    public Transform target;
    public VisibilityScript visScript;
    List<Vector3> visNodePositions;
    List<Vector3> hiddenNodePositions;
    public PlayerLocPredictor playerPredictor;

    public bool debugMode = true;

    [Tooltip("How many fixedUpdates will pass between refreshes of the visibility list")]
    //Turn this up if performance starts to suffer
    public int visRefreshDelay = 10;
    public int visRefreshCountdown = 0;

    void Start(){
        visScript = GameObject.Find("Pathfinding").GetComponent<VisibilityScript>();
        playerPredictor = gameObject.GetComponent<PlayerLocPredictor>();
    }

    void FixedUpdate() {

        if (memory == false) {
            target.position = player.position;
            return;
        }

        visRefreshCountdown -= 1;
        if(visRefreshCountdown <= 0){
            (visNodePositions, hiddenNodePositions) = visScript.generateVisLists(gameObject.transform);
            visRefreshCountdown = visRefreshDelay;
        }

        target.position = playerPredictor.predictedPos;
    }
    
    void OnDrawGizmosSelected(){
        // Draw a small yellow sphere at each visible point
        if(!debugMode){ return; }
        if(Application.isPlaying){
            if(visNodePositions != null){
                Gizmos.color = Color.yellow;
                foreach(Vector3 nodePos in visNodePositions){
                    Gizmos.DrawSphere(nodePos, 0.1f);
                }
            }
            if(hiddenNodePositions != null){
                Gizmos.color = Color.white;
                foreach(Vector3 nodePos in hiddenNodePositions){
                    Gizmos.DrawSphere(nodePos, 0.1f);
                }
            }
        }
    }
}
