using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour{



    private float movementInputDirection;
    private Rigidbody2D rb;
    private Animator anim;
    private bool canMove = true;
    private bool canFlip =true;
    private int facingDirection = 1;
  
    public float movementSpeed = 10.0f;
    public float jumpForce = 18.0f;
    public float groundCheckRadius;

    public bool isFacingRigth = true;
    public bool isGrounded;
    public bool isWalking;
    public bool canJump;
    public LayerMask whatIsGround;
    public Transform groundCheck;
    [SerializeField] private AudioSource dashSoundEffect;
    [SerializeField] private AudioSource walkSoundEffect;
    [SerializeField] private AudioSource jumpSoundEffect;
    [SerializeField] private AudioSource atackSoundEffect;
    private bool isDashing = false;
    private float dashTimeLeft;
    private float lastImageXpos;
    private float lastDash = -100;
    public float dashTime; 
    public float dashSpeed; //lo rapido que se mueve en el dash
    public float distanceBetweenImages;
    public float dashCoolDown;

    [SerializeField] private Transform controladorGolpe;
    [SerializeField] private Transform controladorGolpeDash;
    [SerializeField] private float radioGolpe;
    [SerializeField] private float danoGolpe;
    [SerializeField] private float tiempoEntreAtaques;
    [SerializeField] private float tiempoSiguienteAtaque;

    // Start is called before the first frame update
    void Start(){
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        UpdateAnimationsAndSounds();
        Debug.Log(" Player created");
    }

    // Update is called once per frame
    void Update(){
        CheckInput();
        CheckIfCanJump();
        CheckMovmentDirection();
        UpdateAnimationsAndSounds();
        CheckDash();
    }
    private void FixedUpdate(){
        ApplyMovement();
        CheckSurroundings();
    }

    private void CheckIfCanJump() {
        if (isGrounded){
            canJump = true;
        }
        else {
            canJump = false;
        }
    }
    private void CheckMovmentDirection() {
        if (isFacingRigth && movementInputDirection < 0){
            Flip();
        }
        else if (!isFacingRigth && movementInputDirection > 0) {
            Flip();
        }
        if (rb.velocity.x != 0 && !isDashing){
            isWalking = true;
        }
        else {
            isWalking = false;
        }
    }

    private void UpdateAnimationsAndSounds() {
        anim.SetBool("isWalking", isWalking);
        if (isWalking && isGrounded && !isDashing && !walkSoundEffect.isPlaying) { 
            walkSoundEffect.Play();
        }
        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("yVelocity", rb.velocity.y);
        anim.SetBool("isDashing",isDashing);
    }
    private void CheckInput(){
        movementInputDirection = Input.GetAxisRaw("Horizontal");
        if (Input.GetButtonDown("Jump")) {
            Jump();
        }
        if (Input.GetButtonDown("Dash")){
            if (Time.time >= (lastDash + dashCoolDown)) {
                AttempToDash();
            }  
        }
        if (tiempoSiguienteAtaque > 0){
            tiempoSiguienteAtaque -= Time.deltaTime;
        }
        if (Input.GetButtonDown("Fire1") && tiempoSiguienteAtaque <= 0){
            Atack();
            tiempoSiguienteAtaque = tiempoEntreAtaques;
        }
    }

    private void AttempToDash() {
        isDashing = true;
        if (isGrounded) dashSoundEffect.Play();
        dashTimeLeft = dashTime;
        lastDash = Time.time;
        PlayerAfterImagePool.Instance.GetFromPool();
        lastImageXpos = transform.position.x;
        Collider2D[] objetos = Physics2D.OverlapCircleAll(controladorGolpeDash.position, radioGolpe);
        foreach (Collider2D colisionador in objetos)
        {
            if (colisionador.CompareTag("Enemigo"))
            {
                colisionador.transform.GetComponent<Enemigo>().TomarDano(danoGolpe*3);
            }
        }
    }

    private void CheckDash() {
        if (isDashing) {
            if (dashTimeLeft > 0) {
                canMove = false;
                canFlip = false;
                rb.velocity = new Vector2(dashSpeed * facingDirection, rb.velocity.y);
                dashTimeLeft -= Time.deltaTime;
                if (Mathf.Abs(transform.position.x - lastImageXpos) > distanceBetweenImages){
                    PlayerAfterImagePool.Instance.GetFromPool();
                    lastImageXpos = transform.position.x;
                }
            }
           if (dashTimeLeft <= 0) {
                Debug.Log("Acabo el dash");
                isDashing = false;
                canMove = true;
                canFlip = true;
            }
        }
    }

    private void CheckSurroundings() {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius,whatIsGround);
    }

    private void ApplyMovement(){ //Aplica el movimiento
        if (canMove && movementInputDirection != 0) { //Si 'canMove' es false no se podra mover
            rb.velocity = new Vector2(movementSpeed * movementInputDirection, rb.velocity.y);
        }
        
    }

    private void Atack(){
        anim.SetTrigger("Golpe");
        atackSoundEffect.Play();
        Collider2D[] objetos = Physics2D.OverlapCircleAll(controladorGolpe.position, radioGolpe);
        foreach (Collider2D colisionador in objetos){
            if (colisionador.CompareTag("Enemigo")){
                colisionador.transform.GetComponent<Enemigo>().TomarDano(danoGolpe);
            }
        }
    }
    private void Jump() {
        if (canJump) {
            jumpSoundEffect.Play();
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
       
    }
    private void Flip() {
        if (canFlip) {
            facingDirection *= -1;
            isFacingRigth = !isFacingRigth;
            transform.Rotate(0.0f, 180.0f, 0.0f);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        Gizmos.DrawWireSphere(controladorGolpe.position, radioGolpe);
        Gizmos.DrawWireSphere(controladorGolpeDash.position, radioGolpe);
    }
}
