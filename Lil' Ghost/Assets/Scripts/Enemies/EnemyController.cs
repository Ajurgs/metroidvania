using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : RaycastController
{
    public bool EdgeDetectionActive = true;
    public bool hitEdge = false;
    void Awake()
    {
        collisions.faceDirection = 1;
    }
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Move(Vector3 velocity , bool standingOnPlatform = false)
    {
        UpdateRaycastOrigins();
        collisions.Reset();
        collisions.velocityOld = velocity;


        if (velocity.y < 0)
        {
            DescendSlope(ref velocity);
        }

        if (EdgeDetectionActive)
        {
            EdgeDetection(ref velocity);
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
    public void MoveWithOutEdgeDetection(Vector3 velocity, bool standingOnPlatform = false)
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
    void EdgeDetection(ref Vector3 velocity)
    {
        float directionX = collisions.faceDirection;
        float rayLength = 2 * SkinWidth;
        Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
        RaycastHit2D dropCheck = Physics2D.Raycast(rayOrigin, Vector2.down, rayLength, collisionMask);

        Debug.DrawRay(rayOrigin, Vector2.down * rayLength, Color.green);

        if (!dropCheck)
        {
            velocity.x = 0;
            collisions.faceDirection *= -1;
        }
    }
}
