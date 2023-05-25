using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class ChangeLevel : MonoBehaviour
{

    public int level;
    private void OnTriggerEnter2D(Collider2D other) {
        Debug.Log("ENTRO");
        if(other.gameObject.tag == "Player")
        {
            SceneManager.LoadScene(level);
           
        }
    }
}
