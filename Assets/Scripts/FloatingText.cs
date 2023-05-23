using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingText : MonoBehaviour{

    public float tiempoEnPantalla = 2;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update(){
        tiempoEnPantalla -= Time.deltaTime;
        if (tiempoEnPantalla <= 0) {
            Destroy(this.gameObject);
        }
    }
}
