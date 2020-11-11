using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(BoxCollider2D))]
public class SpikeReturnPoint : MonoBehaviour
{
    public SpikeController effectSpike;
    public Vector3 returnPoint;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Player")
        {
            effectSpike.returnPoint = returnPoint + transform.position;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        float size = .3f;
        Vector3 globalWaypointPos = returnPoint + transform.position;
        Gizmos.DrawLine(globalWaypointPos - Vector3.up * size, globalWaypointPos + Vector3.up * size);
        Gizmos.DrawLine(globalWaypointPos - Vector3.left * size, globalWaypointPos + Vector3.left * size);
    }
}
