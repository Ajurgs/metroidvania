using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(FloatingEnemyContoller))]
public class FloatingEnemyInfo : EnemyInfo
{

    public FloatingEnemyContoller floatController;
    public SpriteRenderer floatRenderer;

    public float waitTime;
    int fromWaypointIndex;
    float percentBetweenWaypoints;
    float nextMoveTime;
    public bool cyclic;

    public Vector3[] localWaypoints;
    public bool showEndPointShadows;
    Vector3[] globalWaypoints;
    // Start is called before the first frame update
    public override void Start()
    {
        floatController = GetComponent<FloatingEnemyContoller>();
        floatRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        globalWaypoints = new Vector3[localWaypoints.Length];
        for (int i = 0; i < localWaypoints.Length; i++)
        {
            globalWaypoints[i] = localWaypoints[i] + transform.position;
        }
    }

    // Update is called once per frame
    public override void Update()
    {
        if (bounderyManager.active)
        {
            OnScreen = true;
        }
        else
        {
            OnScreen = false;
        }
        if (!OnScreen)
        {
            transform.position = startingPos;
            currentState = EnemyState.Active;
            return;
        }

        floatController.FloatMove(CalculateFloatMovement());
        
    }

    public override void SetKnockback(Vector2 direction)
    {
        
    }
    Vector3 CalculateFloatMovement()
    {
        if (Time.time < nextMoveTime)
        {
            return Vector3.zero;
        }
        fromWaypointIndex %= globalWaypoints.Length;
        int toWaypointIndex = (fromWaypointIndex + 1) % globalWaypoints.Length;
        float distanceBetweenWaypoints = Vector3.Distance(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex]);
        percentBetweenWaypoints += Time.deltaTime * walkSpeed / distanceBetweenWaypoints;
        Vector3 newPos = Vector3.Lerp(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex], percentBetweenWaypoints);

        if (percentBetweenWaypoints >= 1)
        {
            percentBetweenWaypoints = 0;
            fromWaypointIndex++;
            if (!cyclic)
            {
                if (fromWaypointIndex >= globalWaypoints.Length - 1)
                {
                    fromWaypointIndex = 0;
                    System.Array.Reverse(globalWaypoints);
                }
            }
            nextMoveTime = Time.time + waitTime;
        }

        return newPos - transform.position;
    }

    void OnDrawGizmos()
    {
        if (localWaypoints != null)
        {
            if (!Application.isPlaying)
            {
                localWaypoints[0] = new Vector3(0, 0, 0);
            }

            Gizmos.color = Color.red;
            float size = .3f;
            for (int i = 0; i < localWaypoints.Length; i++)
            {
                Vector3 globalWaypointPos = (Application.isPlaying) ? globalWaypoints[i] : localWaypoints[i] + transform.position;
                Gizmos.DrawLine(globalWaypointPos - Vector3.up * size, globalWaypointPos + Vector3.up * size);
                Gizmos.DrawLine(globalWaypointPos - Vector3.left * size, globalWaypointPos + Vector3.left * size);
            }
            Gizmos.color = new Color(1, 0, .5f, .5f);
            if (showEndPointShadows)
            {
                for (int i = 1; i < localWaypoints.Length; i++)
                {
                    Vector3 globalWaypointPos = (Application.isPlaying) ? globalWaypoints[i] : localWaypoints[i] + transform.position;
                    Gizmos.DrawCube(globalWaypointPos, transform.localScale);
                }
            }

        }
    }



}
