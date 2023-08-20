using Unity.VisualScripting;
using UnityEngine;

public class Follow : MonoBehaviour {

    public bool memory = true;
    public Transform player;
    public Transform target;
    public VisibilityList visList;

    [Tooltip("How many fixedUpdates will pass between refreshes of the visibility list")]
    //Turn this up if performance starts to suffer
    public int visRefreshDelay = 10;
    public int visRefreshCountdown = 0;

    void Start(){
        visList = GameObject.Find("Pathfinding").GetComponent<VisibilityList>();
    }

    void FixedUpdate() {

        if (memory == false) {
            target.position = player.position;
            return;
        }

        visRefreshCountdown -= 1;
        if(visRefreshCountdown <= 0){
            visList.generateVisLists(player);
            visRefreshCountdown = visRefreshDelay;
        }

        // Check if we have LOS to player
        bool seePlayer = visList.isVisible(transform.position, player);

        if (seePlayer) {
            target.position = player.position;

        } else {
            // enemy can't see player

            // if enemy is near target (not moving soon)
            double dist = Vector3.Distance(transform.position, target.position);
            if (dist < 0.3f) {

                // select new target location


            }

        }

    }
}
