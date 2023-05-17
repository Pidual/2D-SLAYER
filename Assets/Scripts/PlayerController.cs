using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour{

    private float movementInputDirection;
    private Rigidbody2D rb;
    private Animator anim;
   
  
    public float movementSpeed = 10.0f;
    public float jumpForce = 18.0f;
    public float groundCheckRadius;

    public bool isFacingRigth = true;
    public bool isGrounded;
    public bool isWalking;
    public bool canJump;
    public LayerMask whatIsGround;
    public Transform groundCheck;


    private bool isDashing;
    private float dashTimeLeft;
    private float lastImageXpos;
    private float lastDash = -100;
    public float dashTime; 
    public float dashSpeed;
    public float distanceBetweenImages;
    public float dashCoolDown;


    // Start is called before the first frame update
    void Start(){
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        UpdateAnimations();
    }

    // Update is called once per frame
    void Update(){
        CheckInput();
        CheckMovmentDirection();
        UpdateAnimations();
        CheckIfCanJump();
    }
    private void FixedUpdate(){
        ApplyMovement();
        CheckSurroundings();
    }

    private void CheckIfCanJump() {
        if (isGrounded && rb.velocity.y <= 0){
            canJump = true;
        }
        else {
            canJump = false;
        }
    }
    private void CheckMovmentDirection() {
        if (isFacingRigth && movementInputDirection < 0)
        {
            Flip();
        }
        else if (!isFacingRigth && movementInputDirection > 0) {
            Flip();
        }
        if (rb.velocity.x != 0)
        {
            isWalking = true;
        }
        else {
            isWalking = false;
        }
    }

    private void UpdateAnimations() {
        anim.SetBool("isWalking", isWalking);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("yVelocity", rb.velocity.y);
    }
    private void CheckInput(){
        movementInputDirection = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump")) {
            Jump();
        }

        if (Input.GetButtonDown("Dash")) {
            AttempToDash();
        }
    }

    private void AttempToDash() {
        isDashing = true;
        dashTimeLeft = dashTime;
        lastDash = Time.time;

        PlayerAfterImagePool.Instance.GetFromPool();
        lastImageXpos = transform.position.x;
    }

    private void CheckDash() {
        if (isDashing) {
            if (dashTimeLeft > 0) {
                //canFlip = false;
                //canFlip = false;
                //rb.velocity = new Vector2(dashSpeed * facingDirection, rb.velocity.y);
                dashTime -= Time.deltaTime;
                if (Mathf.Abs(transform.position.x - lastImageXpos) > distanceBetweenImages){
                    PlayerAfterImagePool.Instance.GetFromPool();
                    lastImageXpos = transform.position.x;
                }
            }
           //if (dashTimeLeft <= 0 || isTouchingWall) {
                isDashing = false;
                //canMove = true;
                //canFlip = true;
            //}
            
        }
    }

    private void CheckSurroundings() {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius,whatIsGround);
    }

    private void ApplyMovement(){
        rb.velocity = new Vector2(movementSpeed*movementInputDirection, rb.velocity.y);
    }

    private void Jump() {
        if (canJump) {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
       
    }
    private void Flip() {
        isFacingRigth = !isFacingRigth;
        transform.Rotate(0.0f,180.0f, 0.0f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
