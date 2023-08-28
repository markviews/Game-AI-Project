using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class PlayerLocPredictor : MonoBehaviour
{
    public Transform player;
    public Transform predictionMarker;
    public VisibilityScript visScript;
    public Vector3 predictedPos;
    private CircleCollider2D circleCol;

    private TagBrain playerTagBrain, enemyTagBrain;

    //This is a measure of approximately how many fixedUpdates it takes for a player to move the distance
    //between graphNodes. This determines how often we propogate his possible positions when we can't see him
    private int fixed_updates_btwn_nodes; 
    private int node_travel_countdown;

    private GridGraph gg;

    public bool canSeePlayer = false;
    public float ageThreshold;
    public float itAgeThresh = 0.8f;
    public float notItAgeThresh = 0.0f;

    private PlayerMovement pMovement;
    private float playerSpeed = 0f;

    private AIPath enemyPathfinder;

    // Start is called before the first frame update
    void Start()
    {
        ageThreshold = itAgeThresh;
        gg = AstarPath.active.data.gridGraph;
        predictedPos = new Vector3();
        predictionMarker = GameObject.Find("Prediction_Marker").transform;
        //We need to be able to turn the collider off when we cast rays at the player so the rays don't hit ourselves
        circleCol = gameObject.GetComponent<CircleCollider2D>();
       
        enemyPathfinder = gameObject.GetComponent<AIPath>();

        playerTagBrain = GameObject.Find("player").GetComponent<TagBrain>();
        enemyTagBrain  = gameObject.GetComponent<TagBrain>();

        pMovement = GameObject.Find("player").GetComponent<PlayerMovement>();
        calculateFUBN();
        node_travel_countdown = fixed_updates_btwn_nodes;
    }

    void calculateFUBN(){
        float pSpeed = pMovement.speed;
        if(playerSpeed == pSpeed){ return; }

        playerSpeed = pSpeed;
        float distBtwNodes = gg.nodeSize;
        float nodesPerSec =  playerSpeed / distBtwNodes;
        fixed_updates_btwn_nodes = (int)Math.Floor(50 / nodesPerSec);
        Debug.Log("Calculating:\nplayerSpeed = " + playerSpeed + 
            "\ndistBtwNodes = " + distBtwNodes +
            "\nnodesPerSec = " + nodesPerSec +
            "\nFUBN = " + fixed_updates_btwn_nodes);
    }

    void FixedUpdate()
    {
        circleCol.enabled = false;
        if(visScript.isVisible(gameObject.transform.position, player)){
            if(!canSeePlayer){
                Debug.Log("Gained sight of the player!");
            }
            predictedPos = player.position;
            predictionMarker.position = predictedPos;
            circleCol.enabled = true;
            canSeePlayer = true;

            gg.GetNodes(node => {
                node.playerCouldBeHere = false;
            });

            playerTagBrain.isVisible = true;
            enemyTagBrain.isVisible = true;
            
            return;
        }
        circleCol.enabled = true;

        //From this point on only runs if you cannot see the player
        
        playerTagBrain.isVisible = false;
        enemyTagBrain.isVisible = false;

        //If you could see the player last frame
        if(canSeePlayer){
            Debug.Log("Lost sight of the player!");
            GraphNode graphStart = closestHiddenNode(predictedPos);
            graphStart.playerCouldBeHere = true;
            node_travel_countdown = 0;
            Debug.Log("Setting graph start to " + (Vector3)graphStart.position);
        }
        canSeePlayer = false;

        node_travel_countdown--;
        if(node_travel_countdown <= 0){
            updateNodes();
            node_travel_countdown = fixed_updates_btwn_nodes;
            Vector3 bestSpot = getBestSpotToLook();
            if(bestSpot == Vector3.zero){
                predictedPos = visScript.closestHiddenNodeLoc(predictedPos, gameObject.transform);
            } else {
                predictedPos = getBestSpotToLook();
            }
            predictionMarker.position = predictedPos;
        }

        if(Input.GetKeyDown(KeyCode.P)){
            updateNodes();
        } 
    }

    void Update(){ //This key check should go in AIPath but it didn't work when I put it in there and I don't have time to care
        if(Input.GetKeyDown(KeyCode.O)){
            enemyPathfinder.canMove = true;
        }

        if(enemyTagBrain.currentlyIt){
            enemyPathfinder.maxSpeed = 4;
        } else {
            enemyPathfinder.maxSpeed = 3.5f;
        }
    }

    Vector3 getBestSpotToLook(){
        int longestSpot = 0;
        gg.GetNodes(node => {
            if (node.turnsPossible > longestSpot) { longestSpot = node.turnsPossible; }
        });

        GraphNode nearest = null;
        float distance = Single.PositiveInfinity; 
        gg.GetNodes(node => {
            if(!node.Walkable || node.visible || !node.playerCouldBeHere){ return; }
            float curDist = Vector3.Distance((Vector3)node.position, transform.position);
            if(curDist < distance && node.turnsPossible >= (float)((float)longestSpot * ageThreshold)){
                nearest = node;
                distance = curDist;
            }
        });
        if(nearest != null){
            return (Vector3)nearest.position;
        } else {
            Debug.Log("Equals null!!!");
            return Vector3.zero;
        }
    }

    void updateNodes(){
        gg.GetNodes(node => { node.propogatePossibility(); });
        gg.GetNodes(node => { node.propogateConfirm(); });
        gg.GetNodes(node => { node.visualize(); });
    }

    public GraphNode closestHiddenNode(Vector3 startPoint){
        GraphNode closest = null;
        float distance = Single.PositiveInfinity;
        gg.GetNodes(node => {
            if(node.visible || node.Walkable == false){ return; }
            float curDist = Vector3.Distance(startPoint, (Vector3)node.position); 
            if(curDist < distance){
                closest = node;
                distance = curDist;
            }
        });
        //if(closest == null) { Debug.Log ("XXXXXXXXXXXXXX Closest is null!"); }
        return closest;
    }



























// THIS STUFF IS NOT CURRENTLY USED!!

    // //This internal class holds information about where the player could possibly be given their last known location and time
    // //There's only one of these per PlayerLocPredictor, so this doesn't really have to be its own class, but this felt like it made the most sense
    // private class LocationTree {
    //     public LocationNode lastKnownLoc; //Root node
    //     public float timeLastSeen;
    //     public List<LocationNode> leaves;
    //     public Transform transform; //The transform of the object 
    //     public VisibilityScript visScript;
    //     List <Vector3> directions; 
    //     public float playerSpeed;
    //     Vector3 curPrediction;
    //     public float playerRadius = 0.5f; //The radius of the player circle, determines how close you can get to walls

    //     public LocationTree(Transform tf, VisibilityScript vs){
    //         transform = tf;
    //         visScript = vs;
    //         lastKnownLoc = new LocationNode(new Vector3(), null, Vector3.zero);
    //         timeLastSeen = 0.0f;
    //         leaves = new List<LocationNode>();
    //         directions = new List<Vector3>() { 
    //             Vector3.zero,
    //             new Vector3(0, 1, 0), new Vector3(0, -1, 0), new Vector3(-1, 0, 0), new Vector3(1, 0, 0), //Up down left right
    //             new Vector3(0.7071f, 0.7071f, 0), new Vector3(-0.7071f, 0.7071f, 0), new Vector3(0.7071f, -0.7071f, 0), new Vector3(-0.7071f, -0.7071f, 0) //Diagonals
    //         };
    //         playerSpeed = GameObject.Find("player").GetComponent<PlayerMovement>().speed;
    //     }

    //     public class LocationNode {
    //         //Represents a location the player might be. Has children, probability of player being here
    //         //Probability will have to take into account whether player is It/Not-It
    //         //Branches represent paths that the player might have taken
    //         public Vector3 location;
    //         public float probability;
    //         public List<LocationNode> children;
    //         public LocationNode parent;
    //         public Vector3 parentDir;

    //         public LocationNode(Vector3 loc, LocationNode par, Vector3 parDir){ 
    //             location = loc; 
    //             probability = 100.0f; 
    //             children = new List<LocationNode>();
    //             parent = par;
    //             parentDir = parDir;
    //         }
    //     }

    //     public Vector3 updateTree(float deltaTime){
    //         List<LocationNode> leavesToRemove = new List<LocationNode>();
    //         List<LocationNode> leavesToAdd = new List<LocationNode>();
    //         foreach(LocationNode leaf in leaves){
    //             //Expands a leaf node (possible location) with up to 9 child nodes representing directions the player might have gone in deltaTime
    //             leavesToRemove.Add(leaf);

    //             foreach (Vector3 dir in directions){
    //                 if(dir == leaf.parentDir && dir != Vector3.zero) { continue; }

    //                 Vector3 newLoc = leaf.location + dir * playerSpeed * deltaTime;

    //                 //Don't create a new leaf if it involves running into a wall
    //                 bool ranIntoSomething = Physics2D.OverlapCircle(newLoc, playerRadius, 1 << 6) != null; // 1 << 6 = layermask for Walls 
    //                 if(ranIntoSomething){ continue; }
    //                 LocationNode newLeaf = new LocationNode(
    //                     newLoc,
    //                     leaf,
    //                     -dir
    //                 );
    //                 leaf.children.Add(newLeaf);
    //                 leavesToAdd.Add(newLeaf);
    //             }
    //         }
    //         foreach(LocationNode leaf in leavesToAdd)    { leaves.Add   (leaf); }
    //         foreach(LocationNode leaf in leavesToRemove) { leaves.Remove(leaf); }

    //         cullVisible();

    //         //Select current most likely location
    //         float bestProbability = 0.0f;
    //         //------------ TODO: Possibly factor in probabilities of nearby nodes ------------
    //         foreach(LocationNode node in leaves){
    //             if (node.probability > bestProbability) {
    //                 curPrediction = node.location;
    //             }
    //         }

    //         visualizeTree();

    //         return curPrediction;
    //     }

    //     void visualizeTree(){
    //         Queue queue = new Queue();
    //         queue.Enqueue(lastKnownLoc);
    //         while (queue.Count != 0){
    //             LocationNode curNode = (LocationNode)queue.Dequeue();
    //             foreach(LocationNode child in curNode.children){
    //                 queue.Enqueue(child);
    //                 Debug.DrawLine(curNode.location, child.location, (Color.Lerp(Color.white, Color.black, child.probability / 100.0f)), 5f);
    //             }
    //         }
    //     }

    //     void cullVisible(){
    //         //Culls branches that place the player in a location we can currently see
    //         bool needToRepeat = false;
    //         do {
    //             List<LocationNode> leavesToRemove = new List<LocationNode>();
    //             foreach(LocationNode leaf in leaves){
    //                 if(leaf.parent != null){ //Don't bother with the root node
    //                     if(visScript.isVisible(leaf.location, transform)){
    //                         leavesToRemove.Add(leaf); //Remove the leaf from leaves
    //                         leaf.parent.children.Remove(leaf); //Remove the leaf as a child of its parent

    //                         //Go up the tree, and if an ancestor of the seen node has no children, that node is also visible
    //                         if(leaf.parent.children.Count == 0){
    //                             leavesToRemove.Add(leaf.parent);
    //                         }
    //                     }
    //                 }
    //             }
    //         } while (needToRepeat);
    //     }
    
    //     public void clearTree(Vector3 realLoc){
    //         //Called when you can see the player
    //         lastKnownLoc = new LocationNode(realLoc, null, Vector3.zero);
    //         timeLastSeen = Time.time;
    //         leaves.Clear();
    //         leaves.Add(lastKnownLoc);
    //     }
    // }
}