using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Avoid : MonoBehaviour
{
    public bool memory = true;
    public Transform player;
    public Transform target;
    public VisibilityScript VScript;
    List<Vector3> VNodePos;
    List<Vector3> HNodePos;
    public PlayerLocPredictor playerPredict;

    public bool debugMode = true;
    [Tooltip("How many fixedUpdates will pass between refreshes of the visibility list")]
    //Turn this up if performance starts to suffer
    public int visRefreshDelay = 10;
    public int visRefreshCountdown = 0;

    // Start is called before the first frame update
    void Start()
    {
        VScript = GameObject.Find("Pathfinding").GetComponent<VisibilityScript>();
        playerPredict = gameObject.GetComponent<PlayerLocPredictor>();
    }

    void FixedUpdate()
    {

        if (memory == false)
        { 
            return;
        }

        visRefreshCountdown -= 1;
        if (visRefreshCountdown <= 0)
        {
            (VNodePos, HNodePos) = VScript.generateVisLists(gameObject.transform);
            visRefreshCountdown = visRefreshDelay;
        }

        target.position = VScript.furthestHiddenNode(playerPredict.predictedPos, target);
    }
    // Update is called once per frame
    void OnDrawGizmosSelected()
    {
        // Draw a small yellow sphere at each visible point
        if (!debugMode) { return; }
        if (Application.isPlaying)
        {
            if (VNodePos != null)
            {
                Gizmos.color = Color.yellow;
                foreach (Vector3 nodePos in VNodePos)
                {
                    Gizmos.DrawSphere(nodePos, 0.1f);
                }
            }
            if (HNodePos != null)
            {
                Gizmos.color = Color.white;
                foreach (Vector3 nodePos in HNodePos)
                {
                    Gizmos.DrawSphere(nodePos, 0.1f);
                }
            }
        }
    }
}
