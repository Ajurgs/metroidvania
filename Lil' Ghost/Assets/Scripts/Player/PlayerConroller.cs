using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(PlayerInfo))]
public class PlayerConroller : RaycastController
{
    public PlayerInfo player;
    [HideInInspector]
    public Vector2 playerInput;

    void Awake()
    {
        player = GetComponent<PlayerInfo>();
        collisions.faceDirection = 1;
    }
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
    }

    public void Move(Vector3 velocity, Vector2 input,bool isPhasing, bool standingOnPlatform = false)
    {
        
        UpdateRaycastOrigins();
        collisions.Reset();
        collisions.velocityOld = velocity;
        playerInput = input;
        phasing = isPhasing;

        if (velocity.y < 0)
        {
            DescendSlope(ref velocity);
        }
        
        HorizontalCollisions(ref velocity);
        VerticalCollisions(ref velocity);
       

        transform.Translate(velocity);
        if (standingOnPlatform)
        {
            collisions.below = true;
        }

    }
    public void MoveKnockback(Vector3 velocity, bool standingOnPlatform = false)
    {
        UpdateRaycastOrigins();
        collisions.Reset();
        collisions.velocityOld = velocity;


        if (velocity.y < 0)
        {
            DescendSlope(ref velocity);
        }

        HorizontalCollisions(ref velocity);
        VerticalCollisions(ref velocity);

        transform.Translate(velocity);
        if (standingOnPlatform)
        {
            collisions.below = true;
        }
    }
    public void MoveOnPlatform(Vector3 velocity, bool standingOnPlatform = false)
    {
        UpdateRaycastOrigins();
        collisions.Reset();
        collisions.velocityOld = velocity;

        if (velocity.y < 0)
        {
            DescendSlope(ref velocity);
        }

        HorizontalCollisions(ref velocity);
        VerticalCollisions(ref velocity);


        transform.Translate(velocity);
        if (standingOnPlatform)
        {
            collisions.below = true;
        }
    }
}
