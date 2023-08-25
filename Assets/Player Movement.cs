using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    public float speed;
    private Rigidbody2D rb;
    private Vector2 moveDir = Vector2.zero;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update() {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        moveDir = new Vector2(moveX, moveY).normalized;
    }

    void FixedUpdate() {
        rb.velocity = new Vector2(moveDir.x * speed, moveDir.y * speed);
    }
}