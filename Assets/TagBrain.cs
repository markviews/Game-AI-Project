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
    private SpriteRenderer itLabel;
    //private GameObject Light1;

    public void Start(){ 
        //Light1 = gameObject.transform.GetChild(1).gameObject;
        rb = gameObject.GetComponent<Rigidbody2D>(); 
        rb.constraints = RigidbodyConstraints2D.FreezeRotation; 
        itLabel = gameObject.transform.Find("ItLabel").GetComponent<SpriteRenderer>();
        if(currentlyIt){ itLabel.enabled = true; } else { itLabel.enabled = false; }
    }
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
                itLabel.enabled = false;
                //Light1.SetActive(false);
            }
        }
    }

    public void getTagged(){
        currentlyIt = true;
        itLabel.enabled = true;
        freezeRemaining = freezeDuration;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        //Light1.SetActive(true);
    }
}
