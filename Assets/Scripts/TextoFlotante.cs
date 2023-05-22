using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextoFlotante : MonoBehaviour{

    public float TiempoEnPantalla = 2;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update(){
        TiempoEnPantalla -= Time.deltaTime;
        if (TiempoEnPantalla <= 0) {
            Destroy(this.gameObject);
        }
    }
}
