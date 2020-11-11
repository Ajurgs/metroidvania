using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class ShadowBoxConroller : MonoBehaviour
{
    public BounderyManager boundery;
    public Renderer Renderer;
    public BoxCollider2D boxCollider;


    private void Start()
    {
        Renderer = GetComponent<Renderer>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            Renderer.enabled = false;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Renderer.enabled = true;
        }
    }
}
