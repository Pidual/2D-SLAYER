using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEnemigo2D : MonoBehaviour{
    [SerializeField] private float Daño;

    void OnTriggerEnter2D(Collider2D coll){
        if (coll.CompareTag("Player")){
            coll.transform.GetComponent<PlayerController>().TakeDamage(Daño);
        }
    }
}
