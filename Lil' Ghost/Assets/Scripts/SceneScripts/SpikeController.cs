using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class SpikeController : MonoBehaviour
{
    public Vector3 returnPoint;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Enemy")
        {
            // kill
            other.SendMessage("HitSpike");
        }
        else if(other.gameObject.tag == "Player")
        {
            // take damage
            other.SendMessage("HitSpike");
            //move to the return point
            other.SendMessage("MoveToPoint", returnPoint);
        }
    }
}
