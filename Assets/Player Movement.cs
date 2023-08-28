using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    public float speed;
    [SerializeField] private float itSpeed, notItSpeed;
    private Rigidbody2D rb;
    private Vector2 moveDir = Vector2.zero;
    private TagBrain playerTagBrain;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
        playerTagBrain = gameObject.GetComponent<TagBrain>();
    }

    void Update() {    
        if(playerTagBrain.currentlyIt){
            speed = itSpeed;
        } else {
            speed = notItSpeed;
        }

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        moveDir = new Vector2(moveX, moveY).normalized;
    }

    void FixedUpdate() {
        rb.velocity = new Vector2(moveDir.x * speed, moveDir.y * speed);
    }
}