using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System.ComponentModel;
public class VisibilityScript : MonoBehaviour
{
    GridGraph gg;
    public bool debugMode = false;

    void Start(){
        gg = AstarPath.active.data.gridGraph;
    }

    public bool isVisible(Vector3 target, Transform eye){
        RaycastHit2D ray = Physics2D.Raycast(target, eye.position - target);
        //Debug.DrawLine(transform.position, ray.point);
        bool seePoint = ray.transform == eye;
        return seePoint;
    }

    public (List<Vector3>, List<Vector3>) generateVisLists(Transform eye){
        //Generates a list of nodes visible to eye
        List<Vector3> visNodePositions = new List<Vector3>();
        List<Vector3> hiddenNodePositions = new List<Vector3>();
        gg.GetNodes(node => {
            // Here is a node
            Vector3 nodePos = (Vector3)node.position;
            //Debug.Log("I found a node at position " + nodePos);
            if(node.Walkable == false){
                return;
            }
            if(isVisible(nodePos, eye)){
                visNodePositions.Add(nodePos);
                node.visible = true;
                node.turnsPossible = 0;
            } else {
                hiddenNodePositions.Add(nodePos);
                node.visible = false;
            }
            if(debugMode){
                string output = "";
                foreach(PropertyDescriptor descriptor in TypeDescriptor.GetProperties(node)){
                    string name = descriptor.Name;
                    object value = descriptor.GetValue(node);
                    output += (name + '=' + value + "\n");
                }
                Debug.Log(output);
            }
        });
        return (visNodePositions, hiddenNodePositions);
    } 

    public Vector3 closestNodeLoc(List<Vector3> nodePositions, Vector3 startPoint){
        if (nodePositions == null || nodePositions.Count == 0)
        {
            Debug.Log("Vector list cannot be null or empty.");
            return startPoint;
        }

        Vector3 closestVector = nodePositions[0];
        float closestDistance = 100000;

        for (int i = 1; i < nodePositions.Count; i++)
        {
            float distance = Math.Abs(nodePositions[i].x - startPoint.x) + Math.Abs(nodePositions[i].y - startPoint.y);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestVector = nodePositions[i];
            }
        }

        return closestVector;
    }

    public Vector3 closestHiddenNodeLoc(Vector3 startPoint, Transform eye){
        //Finds the closest node to startPoint that is not visible to eye
        List<Vector3> visNodePositions = new List<Vector3>();
        List<Vector3> hiddenNodePositions = new List<Vector3>();
        (visNodePositions, hiddenNodePositions) = generateVisLists(eye);
        return closestNodeLoc(hiddenNodePositions, startPoint);
    }

    public Vector3 furthestNodeLoc(List<Vector3> nodePositions, Vector3 startPoint)
    {
        if (nodePositions == null || nodePositions.Count == 0)
        {
            return startPoint;
        }

        Vector3 furthestVector = nodePositions[0];
        float furthestDistance = 0;
        for (int i = 1; i< nodePositions.Count; i++)
        {
            float distance = Math.Abs(nodePositions[i].x - startPoint.x) + Math.Abs(nodePositions[i].y - startPoint.y);
            if (distance > furthestDistance)
            {
                furthestDistance = distance;
                furthestVector = nodePositions[i];
            }
        }
        return furthestVector;
    }

    public Vector3 furthestHiddenNode(Vector3 startPoint, Transform eye)
    {
        List<Vector3> VNodePositions = new List<Vector3>();
        List<Vector3> HNodePositions = new List<Vector3>();
        (VNodePositions, HNodePositions) = generateVisLists(eye);
        return furthestNodeLoc(HNodePositions, startPoint);
    }

    // public GraphNode closestHiddenNode(Vector3 startPoint){
    //     GraphNode closest = null;
    //     float distance = Single.PositiveInfinity;
    //     gg.GetNodes(node => {
    //         if(node.visible){ return; }
    //         if(Vector3.Distance(startPoint, (Vector3)node.position) < distance){
    //             closest = node;
    //         }
    //     });
    //     return closest;
    // }

    //----- Include this code in GetNodes to print data regarding the node -----
    // string output = "";
    // foreach(PropertyDescriptor descriptor in TypeDescriptor.GetProperties(node)){
    //     string name = descriptor.Name;
    //     object value = descriptor.GetValue(node);
    //     output += (name + '=' + value + "\n");
    // }
    // Debug.Log(output);
}
