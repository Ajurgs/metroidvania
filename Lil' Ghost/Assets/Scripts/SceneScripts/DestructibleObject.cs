using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(BoxCollider2D))]
public class DestructibleObject : MonoBehaviour
{

    public int hitsLeft;
    public Material threeHits;
    public Material twoHits;
    public Material oneHit;

    public bool indestructable;
    public bool destroyed = false;

    public float bounceAmount = 1;
    private void Start()
    {
        if (destroyed)
        {
            DestroyObject();
        }
        else
        {
            GetComponent<MeshRenderer>().material = threeHits;
        }
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void TakeDamage(int amount)
    {
        if (!indestructable)
        {
            hitsLeft -= amount;
            if (hitsLeft == 2)
            {
                GetComponent<MeshRenderer>().material = twoHits;
            }
            else if (hitsLeft == 1)
            {
                GetComponent<MeshRenderer>().material = oneHit;
            }
            else if (hitsLeft <= 0)
            {
                GameControl.control.Shake(.15f, .1f);
                DestroyObject();
            }
        }
        
    }
    void DestroyObject()
    {
        Destroy(this.gameObject);
    }
   
    
}
