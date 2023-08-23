using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocPredictor : MonoBehaviour
{
    public Transform player;
    public VisibilityScript visScript;
    public Vector3 predictedPos;
    private CircleCollider2D circleCol;

    private LocationTree predictionTree;
    private float updateFreq = 0.5f; //MEASURED IN SECONDS
    private float updateCountdown = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        predictedPos = new Vector3();
        //We need to be able to turn this off when we cast rays at the player so the rays don't hit ourselves
        circleCol = gameObject.GetComponent<CircleCollider2D>();

        predictionTree = new LocationTree();
    }

    // Update is called once per frame
    void Update()
    {
        circleCol.enabled = false;
        if(visScript.isVisible(transform.position, player)){
            //Debug.Log("I can see the player!");
            predictedPos = player.position;
            circleCol.enabled = true;
            predictionTree.clearTree(predictedPos);
            return;
        }
        circleCol.enabled = true;

        //This part only runs if you cannot see the player
        updateCountdown -= Time.deltaTime;
        if(updateCountdown <= 0.0f){
            updateCountdown = updateFreq;
            predictedPos = predictionTree.updateTree(Time.deltaTime);
        }

        predictedPos = visScript.closestHiddenNode(predictedPos, gameObject.transform);
    }





    //This internal class holds information about where the player could possibly be given their last known location and time
    private class LocationTree {
        public LocationNode lastKnownLoc; //Root node
        public float timeLastSeen;
        public List<LocationNode> leaves;

        public LocationTree(){
            lastKnownLoc = new LocationNode(new Vector3());
            timeLastSeen = 0.0f;
            leaves = new List<LocationNode>();
        }

        public class LocationNode {
            //Represents a location the player might be. Has children, probability of player being here
            //Probability will have to take into account whether player is It/Not-It
            //Branches represent paths that the player might have taken
            public Vector3 location;
            public double probability;
            List<LocationNode> children;

            public LocationNode(Vector3 loc){ 
                location = loc; 
                probability = 1.0f; 
                children = new List<LocationNode>();
            }
        }

        public Vector3 updateTree(float deltaTime){
            //TODO: Expand each of the leaves
            
            cullVisible();

            Vector3 curPrediction = new Vector3();
            float bestProbability = 0.0f;
            //TODO: Possibly factor in probabilities of nearby nodes 
            foreach(LocationNode node in leaves){
                if (node.probability > bestProbability) {
                    curPrediction = node.location;
                }
            }
            return curPrediction;
        }

        void expandLeaf(float deltaTime){
            //Expands a leaf node (possible location) with up to 9 child nodes representing directions the player might have gone in deltaTime
        }

        void cullVisible(/*Some kind of argument? probably Vector3 where the AI agent is*/){
            //Culls branches that would result in the player being in a location you can currently see
        }
    
        public void clearTree(Vector3 realLoc){
            //Called when you can see the player
            lastKnownLoc = new LocationNode(realLoc);
            timeLastSeen = Time.time;
            leaves.Clear();
            leaves.Add(lastKnownLoc);
        }
    }
}