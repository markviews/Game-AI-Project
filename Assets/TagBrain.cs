using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagBrain : MonoBehaviour
{
    [SerializeField]
    public bool currentlyIt;
    // How many fixedUpdates the agent will freeze when tagged
    public int freezeDuration = 50;
    public int freezeRemaining;
    private Rigidbody2D rb;

    public void Start(){ rb = gameObject.GetComponent<Rigidbody2D>(); rb.constraints = RigidbodyConstraints2D.FreezeRotation; }
    public void FixedUpdate(){
        if(freezeRemaining > 0){
            freezeRemaining--;
        } 

        if(freezeRemaining <= 0){
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.tag == "TagPlayer")
        {
            TagBrain otherPlayerBrain = other.gameObject.GetComponent<TagBrain>();

            if(currentlyIt && freezeRemaining <= 0){
                otherPlayerBrain.getTagged();
                currentlyIt = false;
            }
        }
    }

    public void getTagged(){
        currentlyIt = true;
        freezeRemaining = freezeDuration;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
    }
}
