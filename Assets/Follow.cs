using Unity.VisualScripting;
using UnityEngine;

public class Follow : MonoBehaviour {

    public bool memory = true;
    public Transform player;
    public Transform target;

    void FixedUpdate() {

        if (memory == false) {
            target.position = player.position;
            return;
        }

        // Check if we have LOS to player
        RaycastHit2D ray = Physics2D.Raycast(transform.position, player.position - transform.position);
        bool seePlayer = ray.transform == player;
        //Debug.DrawLine(transform.position, ray.point);

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
