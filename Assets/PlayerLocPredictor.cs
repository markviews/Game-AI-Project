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
        //We need to be able to turn the collider off when we cast rays at the player so the rays don't hit ourselves
        circleCol = gameObject.GetComponent<CircleCollider2D>();
        predictionTree = new LocationTree(gameObject.transform, visScript);
    }

    // Update is called once per frame
    void Update()
    {
        circleCol.enabled = false;
        if(visScript.isVisible(gameObject.transform.position, player)){
            //Debug.Log("I can see the player!");
            predictedPos = player.position;
            circleCol.enabled = true;
            predictionTree.clearTree(predictedPos);
            return;
        }
        circleCol.enabled = true;

        //This part only runs if you cannot see the player
        
        if(Input.GetKeyDown(KeyCode.P)){
            // Debug.Log(transform.position);
            // Debug.Log(
            //     Physics2D.OverlapCircle(transform.position, 6.0f, 1 << 6) != null
            // );
            predictionTree.updateTree(0.3f);
        } 
        
        // updateCountdown -= Time.deltaTime;
        // if(updateCountdown <= 0.0f){
        //     updateCountdown = updateFreq;
        //     predictedPos = predictionTree.updateTree(Time.deltaTime);
        // }

        //predictedPos = visScript.closestHiddenNode(predictedPos, gameObject.transform);
    }



    //This internal class holds information about where the player could possibly be given their last known location and time
    //There's only one of these per PlayerLocPredictor, so this doesn't really have to be its own class, but this felt like it made the most sense
    private class LocationTree {
        public LocationNode lastKnownLoc; //Root node
        public float timeLastSeen;
        public List<LocationNode> leaves;
        public Transform transform; //The transform of the object 
        public VisibilityScript visScript;
        List <Vector3> directions; 
        public float playerSpeed;
        Vector3 curPrediction;
        public float playerRadius = 0.5f; //The radius of the player circle, determines how close you can get to walls

        public LocationTree(Transform tf, VisibilityScript vs){
            transform = tf;
            visScript = vs;
            lastKnownLoc = new LocationNode(new Vector3(), null, Vector3.zero);
            timeLastSeen = 0.0f;
            leaves = new List<LocationNode>();
            directions = new List<Vector3>() { 
                Vector3.zero,
                new Vector3(0, 1, 0), new Vector3(0, -1, 0), new Vector3(-1, 0, 0), new Vector3(1, 0, 0), //Up down left right
                new Vector3(0.7071f, 0.7071f, 0), new Vector3(-0.7071f, 0.7071f, 0), new Vector3(0.7071f, -0.7071f, 0), new Vector3(-0.7071f, -0.7071f, 0) //Diagonals
            };
            playerSpeed = GameObject.Find("player").GetComponent<PlayerMovement>().speed;
        }

        public class LocationNode {
            //Represents a location the player might be. Has children, probability of player being here
            //Probability will have to take into account whether player is It/Not-It
            //Branches represent paths that the player might have taken
            public Vector3 location;
            public float probability;
            public List<LocationNode> children;
            public LocationNode parent;
            public Vector3 parentDir;

            public LocationNode(Vector3 loc, LocationNode par, Vector3 parDir){ 
                location = loc; 
                probability = 100.0f; 
                children = new List<LocationNode>();
                parent = par;
                parentDir = parDir;
            }
        }

        public Vector3 updateTree(float deltaTime){
            List<LocationNode> leavesToRemove = new List<LocationNode>();
            List<LocationNode> leavesToAdd = new List<LocationNode>();
            foreach(LocationNode leaf in leaves){
                //Expands a leaf node (possible location) with up to 9 child nodes representing directions the player might have gone in deltaTime
                leavesToRemove.Add(leaf);
                if(leaf.probability < 1)

                foreach (Vector3 dir in directions){
                    if(dir == leaf.parentDir && dir != Vector3.zero) { continue; }

                    Vector3 newLoc = leaf.location + dir * playerSpeed * deltaTime;

                    //Don't create a new leaf if it involves running into a wall
                    bool ranIntoSomething = Physics2D.OverlapCircle(newLoc, playerRadius, 1 << 6) != null; // 1 << 6 = layermask for Walls 
                    if(ranIntoSomething){ continue; }
                    LocationNode newLeaf = new LocationNode(
                        newLoc,
                        leaf,
                        -dir
                    );
                    leaf.children.Add(newLeaf);
                    leavesToAdd.Add(newLeaf);
                }
                
                float probChunk = leaf.probability / (leaf.children.Count + 1); //Divide the probability of the current node into chunks to be distributed (unevenly) to the children 
                foreach(LocationNode child in leaf.children){
                    child.probability = probChunk;
                }
                if(leaf.parent != null){
                    leaf.parent.probability += probChunk / 3;
                }
            }
            foreach(LocationNode leaf in leavesToAdd)    { leaves.Add   (leaf); }
            foreach(LocationNode leaf in leavesToRemove) { leaves.Remove(leaf); }

            cullVisible();

            //Select current most likely location
            float bestProbability = 0.0f;
            //------------ TODO: Possibly factor in probabilities of nearby nodes ------------
            foreach(LocationNode node in leaves){
                if (node.probability > bestProbability) {
                    curPrediction = node.location;
                }
            }

            visualizeTree();

            return curPrediction;
        }

        void visualizeTree(){
            Queue queue = new Queue();
            queue.Enqueue(lastKnownLoc);
            while (queue.Count != 0){
                LocationNode curNode = (LocationNode)queue.Dequeue();
                foreach(LocationNode child in curNode.children){
                    queue.Enqueue(child);
                    Debug.DrawLine(curNode.location, child.location, (Color.Lerp(Color.white, Color.black, child.probability / 100.0f)), 5f);
                }
            }
        }

        void cullVisible(){
            //Culls branches that place the player in a location we can currently see
            bool needToRepeat = false;
            do {
                List<LocationNode> leavesToRemove = new List<LocationNode>();
                foreach(LocationNode leaf in leaves){
                    if(leaf.parent != null){ //Don't bother with the root node
                        if(visScript.isVisible(leaf.location, transform)){
                            leavesToRemove.Add(leaf); //Remove the leaf from leaves
                            leaf.parent.children.Remove(leaf); //Remove the leaf as a child of its parent

                            //Go up the tree, and if an ancestor of the seen node has no children, that node is also visible
                            if(leaf.parent.children.Count == 0){
                                leavesToRemove.Add(leaf.parent);
                            }
                        }
                    }
                }
            } while (needToRepeat);
        }
    
        public void clearTree(Vector3 realLoc){
            //Called when you can see the player
            lastKnownLoc = new LocationNode(realLoc, null, Vector3.zero);
            timeLastSeen = Time.time;
            leaves.Clear();
            leaves.Add(lastKnownLoc);
        }
    }
}


/*
2d array potentialLocs
-Function to get list of all adjacent nodes
-Function to get list of adjacent hidden nodes
-Function to initialize and delete walls
-Function to visualize
-Function to propogate probability to nearby cells

foreach node
  if(visible and adj node is hidden or vice-versa){
    raycast and update visibility
    if visible probability = 0
  }
  if node !visible {
    propogate 
  }*/