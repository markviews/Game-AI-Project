using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System.ComponentModel;
public class VisibilityList : MonoBehaviour
{
    List<Vector3> visNodePositions;
    List<Vector3> hiddenNodePositions;
    GridGraph gg;

    void Start(){
        gg = AstarPath.active.data.gridGraph;
        visNodePositions = new List<Vector3>();
        hiddenNodePositions = new List<Vector3>();
    }

    public bool isVisible(Vector3 target, Transform eye){
        RaycastHit2D ray = Physics2D.Raycast(target, eye.position - target);
        //Debug.DrawLine(transform.position, ray.point);
        bool seePoint = ray.transform == eye;
        return seePoint;
    }

    void OnDrawGizmosSelected(){
        // Draw a small yellow sphere at each visible point
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

    public void generateVisLists(Transform eye){
        //Generates a list of nodes visible to eye
        visNodePositions.Clear();
        hiddenNodePositions.Clear();
        gg.GetNodes(node => {
            // Here is a node
            Vector3 nodePos = (Vector3)node.position;
            //Debug.Log("I found a node at position " + nodePos);
            if(node.Walkable == false){
                return;
            }
            if(isVisible(nodePos, eye)){
                visNodePositions.Add(nodePos);
            } else {
                hiddenNodePositions.Add(nodePos);
            }
        });
    } 

    //----- Include this code in GetNodes to print data regarding the node -----
    // string output = "";
    // foreach(PropertyDescriptor descriptor in TypeDescriptor.GetProperties(node)){
    //     string name = descriptor.Name;
    //     object value = descriptor.GetValue(node);
    //     output += (name + '=' + value + "\n");
    // }
    // Debug.Log(output);
}
