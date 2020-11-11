using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundController : MonoBehaviour
{
    private void OnBecameInvisible()
    {
        GetComponent<Renderer>().enabled = false;
    }
    private void OnBecameVisible()
    {
        GetComponent<Renderer>().enabled = true;
    }
}
