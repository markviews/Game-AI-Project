using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagBrain : MonoBehaviour
{
    [SerializeField]
    public bool currentlyIt;
    // How many fixedUpdates the agent will freeze when tagged
    private int freezeDuration = 250;
    public int freezeRemaining;
    private Rigidbody2D rb;
    private SpriteRenderer itLabel, mySprite;
    public bool vizMode = false;
    public bool isVisible = true; //Visible to the other 'player'

    public void Start(){ 
        rb = gameObject.GetComponent<Rigidbody2D>(); 
        rb.constraints = RigidbodyConstraints2D.FreezeRotation; 
        itLabel = gameObject.transform.Find("ItLabel").GetComponent<SpriteRenderer>();
        mySprite = gameObject.GetComponent<SpriteRenderer>();
        if(currentlyIt){ itLabel.enabled = true; } else { itLabel.enabled = false; }
    }

    public void Update(){
        if(vizMode || isVisible) { //The game is currently set to our POV OR opponent can see us
            mySprite.enabled = true; 
            if(currentlyIt){ 
                itLabel.enabled = true; 
            } else { 
                itLabel.enabled = false; 
            }
        } else {
            mySprite.enabled = false;
            itLabel.enabled = false;
        }
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
            }
        }
    }

    public void getTagged(){
        currentlyIt = true;
        itLabel.enabled = true;
        freezeRemaining = freezeDuration;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
    }
}
