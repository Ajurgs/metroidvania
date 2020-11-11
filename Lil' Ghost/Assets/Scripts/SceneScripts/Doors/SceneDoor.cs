using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SceneDoor : MonoBehaviour
{

    public int sceneToLoad;
    BoxCollider2D boxCollider;

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            Debug.Log("load scene" + sceneToLoad);
            if (sceneToLoad == 0)
            {
                Debug.Log("Error Tried to load starting scene");
            }
            else
            {
                GameControl.control.LoadScene(sceneToLoad);
            }
            
        }
    }

}
