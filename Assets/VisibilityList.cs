using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisibilityList : MonoBehaviour
{
    
    public bool isVisible(Vector2 target, Transform eye){
        Vector3 targetLoc = new Vector3(target.x, target.y, 0);
        RaycastHit2D ray = Physics2D.Raycast(targetLoc, eye.position - targetLoc);
        //Debug.DrawLine(transform.position, ray.point);
        bool seePoint = ray.transform == eye;
        return seePoint;
    }

    
}
