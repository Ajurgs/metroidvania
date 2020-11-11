using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class PhaseableObject : MonoBehaviour
{

    BoxCollider2D boxCollider;
    new Rigidbody2D rigidbody;
    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        rigidbody = GetComponent<Rigidbody2D>();
        boxCollider.isTrigger = true;
        rigidbody.bodyType = RigidbodyType2D.Static;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            other.GetComponent<PlayerInfo>().inWall = true; 
            
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            other.GetComponent<PlayerInfo>().inWall = false;
        }
    }
}
