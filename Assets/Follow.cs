using Unity.VisualScripting;
using UnityEngine;

public class Follow : MonoBehaviour {

    public bool memory = true;
    public Transform player;
    public Transform target;
    public VisibilityList visList;

    void Start(){
        visList = GameObject.Find("Pathfinding").GetComponent<VisibilityList>();
    }

    void FixedUpdate() {

        if (memory == false) {
            target.position = player.position;
            return;
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
