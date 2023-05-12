using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour{

    private float movementInputDirection;
    private Rigidbody2D rb;
    public float movementSpeed = 10.0f;
    private bool isFacingRigth = true;
    private float jumpForce = 16.0f;
    public int healt = 10;


    // Start is called before the first frame update
    void Start(){
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update(){
        CheckInput();
        CheckMovmentDirection();
        
    }

    private void CheckMovmentDirection() {
        if (isFacingRigth && movementInputDirection < 0)
        {
            Flip();
        }
        else if (!isFacingRigth && movementInputDirection > 0) {
            Flip();
        }
    }

    private void CheckInput(){
        movementInputDirection = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump")) {
            Jump();
        }
    }

    private void FixedUpdate(){
        ApplyMovement();
    }
    private void ApplyMovement(){
        rb.velocity = new Vector2(movementSpeed*movementInputDirection, rb.velocity.y);
    }

    private void Jump() {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }
    private void Flip() {
        isFacingRigth = !isFacingRigth;
        transform.Rotate(0.0f,180.0f, 0.0f);
    }
}
