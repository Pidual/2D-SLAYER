using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using RandomWalkLibrary;
public class FlyingEnemy : MonoBehaviour{

    [SerializeField]private float speed;
    
    [SerializeField] float range;
    [SerializeField] float maxDistance;
    Vector2 wayPoint;

    private bool chase = false;
    public Transform startingPoint;
    private GameObject player;
   

    void Start(){
        player = GameObject.FindGameObjectWithTag("Player");
        SetNewDestination();
    }

    void Update(){
        transform.position = Vector2.MoveTowards(transform.position, wayPoint, speed * Time.deltaTime);// un punto
        if (Vector2.Distance(transform.position, wayPoint) < range) {
            SetNewDestination();
        }
        //if (player == null)
            //return;
        //if (chase = true)
            //Chase();
        //else
            //Goto start
        Flip();
    }

    void SetNewDestination() { //Le da un nuevo 
       Tuple<float,float> tupla = RandomWalk.PerformRandomWalk(Tuple.Create(transform.position.x, transform.position.y), 4);
       wayPoint = new Vector2(tupla.Item1, tupla.Item2);
    }
    private void Chase() {
        transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
    }

    private void Flip(){
        if (transform.position.x > player.transform.position.x)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
}
