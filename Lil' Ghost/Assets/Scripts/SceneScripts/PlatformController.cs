using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlatformController : RaycastController
{

    public LayerMask passengerMask;
    public bool IsElevator;
    public float speed;
    public bool cyclic;
    public float waitTime;
    int fromWaypointIndex;
    float percentBetweenWaypoints;
    float nextMoveTime;

    bool hasBeenCalled;
    bool isMoving;
    
    public Vector3[] localWaypoints;
    public bool showEndPointShadows;
    Vector3[] globalWaypoints;
    List<PassengerMovement> passengerMovement;
    Dictionary<Transform, PlayerConroller> passengerControllerDictonary = new Dictionary<Transform, PlayerConroller>();
    Dictionary<Transform, PlayerInfo> passengerPlayerDictonary = new Dictionary<Transform, PlayerInfo>();
    // Use this for initialization
    public override void Start()
    {
        base.Start();
        int scaleX = (int)Mathf.Round(transform.localScale.x);
        verticalRayCount = scaleX * 5;
        CalculateRaycastSpacing();
        UpdateRaycastOrigins();
        globalWaypoints = new Vector3[localWaypoints.Length];
        for (int i = 0; i < localWaypoints.Length; i++)
        {
            globalWaypoints[i] = localWaypoints[i] + transform.position;
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        UpdateRaycastOrigins();
        Vector3 velocity = CalculatePlatformMovement();

        CalculatePassengerMovement(velocity);

        MovePassengers(true);
        transform.Translate(velocity);
        MovePassengers(false);
    }
    void MovePassengers(bool beforeMovePlatform)
    {
        foreach (PassengerMovement passenger in passengerMovement)
        {
            if (!passengerControllerDictonary.ContainsKey(passenger.transform))
            {
                passengerControllerDictonary.Add(passenger.transform, passenger.transform.GetComponent<PlayerConroller>());
            }
            if (!passengerPlayerDictonary.ContainsKey(passenger.transform))
            {
                passengerPlayerDictonary.Add(passenger.transform, passenger.transform.GetComponent<PlayerInfo>());
            }
            if (passenger.moveBeforePlatform == beforeMovePlatform)
            {
                //if (passengerPlayerDictonary[passenger.transform].climbingLadder)
                //{
                //    passengerPlayerDictonary[passenger.transform].climbingLadder = false;
                //}
                passengerControllerDictonary[passenger.transform].MoveOnPlatform(passenger.velocity, passenger.standingOnPlatform);
            }
        }
    }

    Vector3 CalculatePlatformMovement()
    {
        if (!IsElevator)
        {
            if (Time.time < nextMoveTime)
            {
                return Vector3.zero;
            }
            fromWaypointIndex %= globalWaypoints.Length;
            int toWaypointIndex = (fromWaypointIndex + 1) % globalWaypoints.Length;
            float distanceBetweenWaypoints = Vector3.Distance(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex]);
            percentBetweenWaypoints += Time.deltaTime * speed / distanceBetweenWaypoints;
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
        else
        {
            if (!hasBeenCalled && !isMoving)
            {
                return Vector3.zero;
            }
            fromWaypointIndex %= globalWaypoints.Length;
            int toWaypointIndex = (fromWaypointIndex + 1) % globalWaypoints.Length;
            float distanceBetweenWaypoints = Vector3.Distance(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex]);
            percentBetweenWaypoints += Time.deltaTime * speed / distanceBetweenWaypoints;
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
                GameControl.control.Shake(.1f, .1f);
                isMoving = false;
                hasBeenCalled = false;
            }

            return newPos - transform.position;
        }
    }
    void CallElevator()
    {
        if(isMoving == false)
        {
            hasBeenCalled = true;
            isMoving = true;
        }
        
    }
    void CalculatePassengerMovement(Vector3 velocity)
    {
        HashSet<Transform> movedPassengers = new HashSet<Transform>();
        passengerMovement = new List<PassengerMovement>();
        float directionX = Mathf.Sign(velocity.x);
        float directionY = Mathf.Sign(velocity.y);

        // vertically moving platform
        if (velocity.y != 0)
        {
            float rayLength = Mathf.Abs(velocity.y) + SkinWidth;
            for (int i = 0; i < verticalRayCount; i++)
            {
                Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
                rayOrigin += Vector2.right * (verticalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, passengerMask);
                Debug.DrawRay(rayOrigin, Vector2.up * directionY * 1, Color.red);
                if (hit)
                {
                    if (!movedPassengers.Contains(hit.transform))
                    {
                        movedPassengers.Add(hit.transform);
                        float pushX = (directionY == 1) ? velocity.x : 0;
                        float pushY = velocity.y - (hit.distance - SkinWidth) * directionY;

                        passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), directionY == 1, true));
                    }

                }
            }
        }

        // horizontally moving platform
        if (velocity.x != 0)
        {
            float rayLength = Mathf.Abs(velocity.x) + SkinWidth;
            for (int i = 0; i < horizontalRayCount; i++)
            {
                Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
                rayOrigin += Vector2.up * (horizontalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, passengerMask);

                if (hit)
                {
                    if (!movedPassengers.Contains(hit.transform))
                    {
                        movedPassengers.Add(hit.transform);
                        float pushX = velocity.x - (hit.distance - SkinWidth) * directionX;
                        float pushY = 0;

                        passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), false, true));
                    }

                }

            }
        }
        // player on top of a horizontaly or downward moving platform
        if (directionY == -1 || velocity.y == 0 && velocity.x != 0)
        {
            float rayLength = SkinWidth * 2;

            for (int i = 0; i < verticalRayCount; i++)
            {
                Vector2 rayOrigin = raycastOrigins.topLeft + Vector2.right * (verticalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, rayLength, passengerMask);

                if (hit)
                {
                    if (!movedPassengers.Contains(hit.transform))
                    {
                        movedPassengers.Add(hit.transform);
                        float pushX = velocity.x;
                        float pushY = velocity.y;

                        passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), true, false));
                    }

                }
            }
        }
    }

    struct PassengerMovement
    {
        public Transform transform;
        public Vector3 velocity;
        public bool standingOnPlatform;
        public bool moveBeforePlatform;

        public PassengerMovement(Transform _transform, Vector3 _velocity, bool _standingOnPlatform, bool _moveBeforePlatform)
        {
            transform = _transform;
            velocity = _velocity;
            standingOnPlatform = _standingOnPlatform;
            moveBeforePlatform = _moveBeforePlatform;
        }
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
