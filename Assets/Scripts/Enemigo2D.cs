using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemigo2D : MonoBehaviour
{
    public int rutina; //Un numero que indica el estado del enemigo
    public float cronometro; //NO SE JAJAJA
    public Animator ani; // El animator UwU
    public int direccion; //Supongo que es la direccion del enemigo
    public float speed_walk; //Velocidad de caminar del enemigo
    public float speed_run; //Velocidad de correr del enemigo
    public GameObject target; //Target es el player por el editor de unity le pasamos el GameObject
    public bool atacando; //Booleano que permita atacar o no
    [SerializeField] private float vida; //Vida
    public float rango_vision; //El rango de vision
    public float rango_ataque; //El rango de ataque
    public GameObject rango; // El boxcolider donde si el player se queda parado lo ven
    public GameObject Hit; //El boxcolider donde si el jugador se queda parado le hacen da�o
  
    // Start is called before the first frame update
    void Start(){
        ani = GetComponent<Animator>(); //Animator para el enemigo
        target = GameObject.Find("Player"); //Asigna el objetivo en este caso el jugador :p
    }

    //Pues es un if con un switch para ver que hace le enemigo
    public void Comportamientos(){
        if(Mathf.Abs(transform.position.x - target.transform.position.x) > rango_vision && !atacando){
            ani.SetBool("run", false);
            cronometro += 1 * Time.deltaTime;
            if (cronometro >= 4){
                rutina = Random.Range(0, 2); //AJAJAJJAJAJAJAJAJAJ un numero del 0 al 2 JAJAJJAJAJAJAJAJAJ
                cronometro = 0;
            }
            switch (rutina){ //Si el enemigo esta en 0 se queda quieto
                case 0: //dejar de caminar
                    ani.SetBool("walk", false);
                    break;

                case 1: //Caminar en una direccion aleatorea
                    direccion = Random.Range(0, 2); //Aca deberia haber un montecarlo pero pues jua jua jua
                    rutina++; //Hace que entre a case 2
                    break;

                case 2: //Con la 
                    switch (direccion){
                        case 0: //camina hacia la izquierda
                            transform.rotation = Quaternion.Euler(0, 0, 0);
                            transform.Translate(Vector3.right * speed_walk * Time.deltaTime);
                            break;

                        case 1: //Camina hacia la derecha
                            transform.rotation = Quaternion.Euler(0, 180, 0);
                            transform.Translate(Vector3.right * speed_walk * Time.deltaTime);
                            break;
                    }
                    ani.SetBool("walk", true);
                    break;
            }
        }
        else{
            if (Mathf.Abs(transform.position.x - target.transform.position.x) > rango_ataque && !atacando) //Si 
            {
                if (transform.position.x < target.transform.position.x)
                {
                    ani.SetBool("walk", false);
                    ani.SetBool("run", true);
                    transform.Translate(Vector3.right * speed_run * Time.deltaTime);
                    transform.rotation = Quaternion.Euler(0, 0, 0);
                    ani.SetBool("attack", false);
                }
                else
                {
                    ani.SetBool("walk", false);
                    ani.SetBool("run", true);
                    transform.Translate(Vector3.right * speed_run * Time.deltaTime);
                    transform.rotation = Quaternion.Euler(0, 180, 0);
                    ani.SetBool("attack", false);
                }
            }
            else{
                if (!atacando){
                    if (transform.position.x < target.transform.position.x){
                        transform.rotation = Quaternion.Euler(0, 0, 0);
                    }
                    else{
                        transform.rotation = Quaternion.Euler(0, 180, 0);
                    }
                    ani.SetBool("walk", false);
                    ani.SetBool("run", false);
                }
            }
        }
    }

    //Cuando un ataque acaba revisa si el jugador sigue en el rango de vision
    public void Final_Ani(){
        ani.SetBool("attack", false);
        atacando = false;
        rango.GetComponent<BoxCollider2D>().enabled = true;
    }
    //Este metodo es llamado desde el panel de animaciones cuando aparece la espadita del esqueleto aparece una hitbox 
    public void ColliderWeaponTrue(){
        Hit.GetComponent<BoxCollider2D>().enabled = true;
    }

    //Este metodo es llamado desde el panel de animaciones cuando se esta desapareciendo la espadita del esqueleto desaparece la hitbox 
    public void ColliderWeaponFalse(){
        Hit.GetComponent<BoxCollider2D>().enabled = false;
    }

    //este metodo es llamado por player cuando enemigo esta dentro del hitbox de un ataque
    public void TakeDamage(float dano){
        vida -= dano;
        if (vida <= 0){
            ani.SetTrigger("Death");
            GetComponent<Rigidbody2D>().gravityScale = 0;
            GetComponent<Rigidbody2D>().mass = 0;
            GetComponent<BoxCollider2D>().enabled = false;
        }
    }

    //Cada uptade se actualiza las cosas que hacer el enemigo
    void Update(){
        Comportamientos();
    }
}
