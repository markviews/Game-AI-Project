using Unity.VisualScripting;
using UnityEngine;

public class Follow : MonoBehaviour {

    public bool memory = true;
    public Transform player;
    public Transform target;
    public VisibilityScript visScript;
    public PlayerLocPredictor playerPredictor;

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
            visScript.generateVisLists(player);
            visRefreshCountdown = visRefreshDelay;
        }

        target.position = playerPredictor.predictedPos;
    }
}
