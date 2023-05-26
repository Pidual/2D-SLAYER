using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using MonteCarloLibrary;

public class PlayerController : MonoBehaviour{

    private float movementInputDirection; //Es un numero si es negativo es a la izquierda positivo a la derecha
    private Rigidbody2D rb; //el ridigbody del personaje (le da gravedad y permite que se mueva en el espacio)
    private Animator anim; //El animator (controla las animaciones)
    private bool canMove = true; //Booleano que habilita o no el movimiento
    private bool canFlip =true; //Boleano que habilita o no si se puede girar (mirar de izquierda a derecha o viceversa)
    private bool canDash = true; //Booleano que permite si hacemos un dash o no
    private bool canAttack = true; //Booleano que permite si atacamos o no
    private int facingDirection = 1; //La direccion a la que miramos esta en 1 positivo para ver a la derecha.
    public bool isFacingRigth = true; //Booleano que nos indica a que direccion estamos viendo.
    public bool isGrounded; //Booleano que nos indica si estamos tocando el piso
    public bool isWalking; //Booleano que nos indica si nos estamos moviendo
    private bool isDashing = false; //Booleano para ver si estamos realizando un dash
    public bool canJump; //Booleano que indica si podemos saltar

    public float movementSpeed = 10.0f; //Velocidad de movimiento
    public float jumpForce = 18.0f; //Fuerza del salto
    public float groundCheckRadius; //Es el radio del circulito de aca abajo ↓↓↓↓↓↓↓↓↓
    public Transform groundCheck; //Es un circulito que si toca el piso checa si estamos tocando el suelo

    public LayerMask whatIsGround; //Indica que layer es el piso (asignada desde el editor de unity la layer es "Ground")
    
    // UN SERIALIZED FIELD es un tag que le ponemos para poder asignarle el valor desde el editor de Unity
    [SerializeField] private AudioSource dashSoundEffect; //Sonido del dash
    [SerializeField] private AudioSource walkSoundEffect; //Sonido de caminar
    [SerializeField] private AudioSource jumpSoundEffect; //Sonido de saltar
    [SerializeField] private AudioSource atackSoundEffect; //Sonido de atacar


    private float dashTimeLeft; //Tiempo que falta hasta que se acabe el dash
    private float lastImageXpos; //La ultima posicion de la imagen que se utiliza para darle un efecto bacano al dash
    private float lastDash = -100; //Tiempo asignado para iniciar un dash
    public float dashTime;  //Tiempo de duracion de un dash
    public float dashSpeed; //lo rapido que se mueve en el dash
    public float distanceBetweenImages; //Imagenes sacadas de un pool para que el dash se vea bacanano (no es importante entender esto)
    public float dashCoolDown; //Cooldown de dash

    [SerializeField] private float vida;

    [SerializeField] private Transform controladorGolpe; //Una hitbox para ver a que le pegamos
    [SerializeField] private Transform controladorGolpeDash; //Una hitbox para ver a que le pegamos con el dash
    [SerializeField] private float radioGolpe; //Radio de las hitbox de arriba
    [SerializeField] private float danoGolpe; //Daño de los golpes
    [SerializeField] private float tiempoEntreAtaques; //La velocidad de ataque
    [SerializeField] private float tiempoSiguienteAtaque; //Ni idea HJAHAHHAHAHA

    // Start es como un constructor se ejecuta apenas inicia el juego
    void Start(){
        rb = GetComponent<Rigidbody2D>(); //asignamos el rididbody
        anim = GetComponent<Animator>(); //Le asignamos el animator
        UpdateAnimationsAndSounds(); //Actualizamos por primera vez las animaciones y sonidos
    }

    // Update is called once per frame 
    void Update(){ //Se ejecuta en la cantidad de fps (Esto explica por que GTA V no se puede jugar a mas de 240 fps por que se buggea)
        CheckInput(); //Revisa que teclas precionamos
        CheckIfCanJump(); //Revisa si podemos saltar
        CheckMovmentDirection(); //
        CheckDash();
    }
    private void FixedUpdate(){ //Este metodo se ejecuta siempre 60 veces por segundo sin exepcion
        ApplyMovement(); //Aplica el movimiento
        CheckSurroundings(); //Revisa si estamos tocando el piso
        UpdateAnimationsAndSounds(); //Animaciones y sonidos :P
    }

    //Javadoc ;p
    // i
    private void CheckIfCanJump() {
        if (isGrounded){
            canJump = true;
        }
        else {
            canJump = false;
        }
    }

    //
    private void CheckMovmentDirection() {
        if (isFacingRigth && movementInputDirection < 0){
            Flip();
        }
        else if (!isFacingRigth && movementInputDirection > 0) {
            Flip();
        }
        if (Mathf.Abs(rb.velocity.x) >= 0.01f && !isDashing){
            isWalking = true;
        }
        else {
            isWalking = false;
        }
    }

     //Actualiza las animaciones y sonidos usando los booleanos declarados en el inicio del script
    private void UpdateAnimationsAndSounds() {
        anim.SetBool("isWalking", isWalking);
        //Si estamos caminando & estamos en el piso & no estamos dasheando...
        //... & el sonido de caminar no esta sonando  ← si no ponemos esto el sonido suena como cuando se crashea un juego XD
        if (isWalking && isGrounded && !isDashing && !walkSoundEffect.isPlaying) { 
            walkSoundEffect.Play();
        }
        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("yVelocity", rb.velocity.y);
        anim.SetBool("isDashing",isDashing);
    }

    //Revisa que tecla estamos presionando
    private void CheckInput(){
        movementInputDirection = Input.GetAxisRaw("Horizontal");
        if (Input.GetButtonDown("Jump")) { //Si precionamos el espacio saltamos
            Jump();
        }
        if (Input.GetButtonDown("Dash")){ //El boton para hacer el dash fue asignado desde el editor de Unity
            if (Time.time >= (lastDash + dashCoolDown) && canDash) {
                AttempToDash();
            }  
        }
        if (tiempoSiguienteAtaque > 0){
            tiempoSiguienteAtaque -= Time.deltaTime;
        }
        if (Input.GetButtonDown("Fire1") && tiempoSiguienteAtaque <= 0 && canAttack){
            Atack();
            tiempoSiguienteAtaque = tiempoEntreAtaques;
        }
    }

    //Meto super complicado para realizar un dash :P
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
                colisionador.transform.GetComponent<Enemigo2D>().TakeDamage(danoGolpe*5);
            }
        }
    }

    //Este es el metodo para hacer el dash todo cool 
    //La verdad este metodo me lo lootee de internet
//      -----|-----
//   *>=====[_] L)      lo unico que hay que saber aca es que rb.velocity es la velocidad de rigidbody 
//         -'-`-
    private void CheckDash() {
        if (isDashing) {
            if (dashTimeLeft > 0) {
                canMove = false;
                canFlip = false;
                rb.velocity = new Vector2(dashSpeed * facingDirection, rb.velocity.y); //Aplica el dash
                dashTimeLeft -= Time.deltaTime; //Es lo que controla que no hagamos un dash infinito
                if (Mathf.Abs(transform.position.x - lastImageXpos) > distanceBetweenImages){ //Esta monda es para un efecto visual no tiene importnacia
                    PlayerAfterImagePool.Instance.GetFromPool(); //Una cosita para efectos visuales
                    lastImageXpos = transform.position.x; //Efecto visual
                }
            }
           if (dashTimeLeft <= 0) { //cuando el dash acaba
                isDashing = false; 
                canMove = true;
                canFlip = true;
            }
        }
    }

      //Este metodo checa si estamos tocando el piso el persoanje en los pies tiene un circulito
      //Si este piso esta tocando el tilemap que es el piso pues pone la variable true o false
    private void CheckSurroundings() {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius,whatIsGround);
    }


    private void ApplyMovement(){ //Aplica el movimiento
        if (canMove && movementInputDirection != 0) { //Si 'canMove' es false no se podra mover
            rb.velocity = new Vector2(movementSpeed * movementInputDirection, rb.velocity.y);
        }
        
    }

    //Este metodo recibe el numero generado por el montecarlo
    public static float GetCategory(double number){
        if (number >= 0 && number <= 0.25){ //25% de hacer la mitad de daño
            return 0.5f;
        }
        else if (number > 0.25 && number <= 0.5){ //25% de hacer el 75% de daño
            return 0.75f;
        }
        else if (number > 0.5 && number <= 0.75){ //25% de hacer el daño normal
            return 1f;
        }
        else if (number > 0.75 && number <= 1){ //25% de hacer el doble de daño
            return 2f;
        }
        else
        {
            return -1f;
        }
    }

//    //Atacar miau miau
//     _________
//    / ======= \
//   / __________\      Este metodo es importante Atack() es complejo
//  | ___________ |     Al atacar hacemos aparecer una hitbox que es el array llamado objetos
//  | | -       | |     Todos los enemigos que esten en esta hitbox reciben daño 
//  | |         | |     Luego el daño es calculado con un metodo montecarlo
//  | |_________| |________________________        dependiendo del numero que uno saque en el montecarlo el daño se puede duplicar o reducir
//  \=____________/                        )
//  / """"""""""" \                       /
// / ::::::::::::: \                  =D-'
//(_________________)
    //De hecho este metodo 
    private void Atack(){
        anim.SetTrigger("Golpe"); //Animacion del espadaso 
        atackSoundEffect.Play(); //Sonido del machetazo
        //Creamos un array de objetos (aca guardamos los tags de los enemigos a los que le pegamos con la hitbox de ataque)
        Collider2D[] objetos = Physics2D.OverlapCircleAll(controladorGolpe.position, radioGolpe);
        foreach (Collider2D colisionador in objetos){//Recorremos a los enemigos que les pegamos
            if (colisionador.CompareTag("Enemigo")){ //Si le pegamos a un enemigo
                //Llamamos transform → getEnemigo → enemigo.tomardaño → IMPORTANTE: Montercarlo 
                colisionador.transform.GetComponent<Enemigo2D>(). 
                TakeDamage(GetCategory(MonteCarloGenerator.MonteCarlo(1)[0]) * danoGolpe);
                //ACA ESTA EL MONTECARLO MIAU MIAU MIAU 
            }
        }
    }

    //Saltar
    private void Jump(){
        if (canJump) {
            jumpSoundEffect.Play(); //Sonido de saltar
            rb.velocity = new Vector2(rb.velocity.x, jumpForce); //Injectamos como cocaina un vector de velocidad para "dar el efecto de un salto"
        }
       
    }

    //Gira la direccion del personaje (mirar izquierda, mirar a la derecha)
    private void Flip() {
        if (canFlip) {
            facingDirection *= -1;
            isFacingRigth = !isFacingRigth;
            transform.Rotate(0.0f, 180.0f, 0.0f);
        }
    }

    //Recibir daño este metodo se llama cuando estemos en la hitbox de un enemigo
    public void TakeDamage(float damage) {
        vida -= damage;
        anim.SetTrigger("gotHit");  //hace la anmacion de recibir daño
        if(vida <= 0) {
            Die();
        }
    }

    //Controla cuando morimos esta re buggeado
    //
    private void Die() {
        canMove = false;
        canJump = false;
        canFlip = false;
        canDash = false;
        canAttack = false;
        anim.SetTrigger("Player Death"); //Pone la animacion de morirse
        GetComponent<BoxCollider2D>().enabled = false; //Desabilita el boxcolider del player para que no reinicien la animacion de morirse
        rb.velocity = new Vector2(0, 0); //Nos detiene en el lugar que morimos
        rb.gravityScale = 0; //Quitamos la gravedad por que quitamos el colider y si no quitamos la gravedad nos caemos del mapa (Si yo se que esta re trucoteca esta linea pero ps bueno)
    }

    //Nos permite ver los hitboxes en el editor Unity
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        Gizmos.DrawWireSphere(controladorGolpe.position, radioGolpe);
        Gizmos.DrawWireSphere(controladorGolpeDash.position, radioGolpe);
    }

    //Reinicia el nivel este metodo se llama en el ultimo frame de la animacion de morirse por eso es que si uno se muere y logra cambiar la animacion
    //El nivel queda hardlocked HAHAHAHHAHAHAHAAAHDHASUJHDSJAHDJAS
    private void RestartLevel() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);//Reinicia la escena
    }
}
