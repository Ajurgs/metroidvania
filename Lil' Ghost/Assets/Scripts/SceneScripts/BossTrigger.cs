using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTrigger : MonoBehaviour
{
    [SerializeField]
    private CharacterInfo boss;


    void OnEnable()
    {
        if (boss.alive)
        {
            GameControl.ResetOnRest += Reset;
        }
           
    }
    void OnDisable()
    {
        if(boss.alive)
        {
            GameControl.ResetOnRest -= Reset;
        }
        if(!boss.alive)
        {
            GameControl.ResetOnRest -= Reset;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player" && boss != null)
        {
            Debug.Log("Boss Start");
            boss.SendMessage("Activate");
            boss.target = other.gameObject;
            GetComponent<BoxCollider2D>().enabled = false;
        }
        
    }


    private void Reset()
    {
        GetComponent<BoxCollider2D>().enabled = true;
        boss.target = null;
    }

}
