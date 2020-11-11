using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class RaycastController : MonoBehaviour
{
    public const float SkinWidth = 0.015f;
    public CollisionInfo collisions;
    public LayerMask collisionMaskPhasing;
    public LayerMask collisionMask;
    public int horizontalRayCount = 4;
    public int verticalRayCount = 4;
    //public float gravity;

    // Slope Climbing
    [HideInInspector]
    public float maxClimbAngle = 50;
    [HideInInspector]
    public float maxDescendAngle =50;
    [HideInInspector]
    public bool phasing;

    [HideInInspector]
    public new BoxCollider2D collider;
    [HideInInspector]
    public new Rigidbody2D rigidbody;
    public RaycastOrigins raycastOrigins;
    [HideInInspector]
    public float horizontalRaySpacing;
    [HideInInspector]
    public float verticalRaySpacing;

    public virtual void Start()
    {
        collider = GetComponent<BoxCollider2D>();
        rigidbody = GetComponent<Rigidbody2D>();
        CalculateRaycastSpacing();
    }

    // update the origins for the raycasts
    public void UpdateRaycastOrigins()
    {
        Bounds bounds = collider.bounds;
        bounds.Expand(SkinWidth * -2);

        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }

    // calculate the raycast spacing
    public void CalculateRaycastSpacing()
    {
        Bounds bounds = collider.bounds;
        bounds.Expand(SkinWidth * -2);
        // constrain the ray counts to a minimum of 2
        horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
        verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);
        // calculate the distance between rays
        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }

    // check for horizontal collisions
    public void HorizontalCollisions(ref Vector3 velocity)
    {
        float directionX = 0;
        // check if moving the way character is facing
        if (Mathf.Sign(velocity.x) != Mathf.Sign(collisions.faceDirection))
        {
            directionX = 1 * Mathf.Sign(velocity.x);
        }
        else
        {
            directionX = collisions.faceDirection;
        }
        
        float rayLength = Mathf.Abs(velocity.x) + SkinWidth;
        for (int i = 0; i < horizontalRayCount; i++)
        {
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * (horizontalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, (phasing) ? collisionMaskPhasing : collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);

            if (hit)
            {            
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                if (i == 0 && slopeAngle <= maxClimbAngle)
                {
                    if (collisions.descendingSlope)
                    {
                        collisions.descendingSlope = false;
                        velocity = collisions.velocityOld;
                    }

                    float distanceToSlopeStart = 0;
                    if (slopeAngle != collisions.slopeAngleOld)
                    {
                        distanceToSlopeStart = hit.distance - SkinWidth;
                        velocity.x -= distanceToSlopeStart * directionX;
                    }
                    ClimbSlope(ref velocity, slopeAngle);
                    velocity.x += distanceToSlopeStart * directionX;
                }

                if (!collisions.climbingSlope || slopeAngle > maxClimbAngle)
                {
                    velocity.x = (hit.distance - SkinWidth) * directionX;
                    rayLength = hit.distance;

                    if (collisions.climbingSlope)
                    {
                        velocity.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x);
                    }

                    collisions.left = (directionX == -1);
                    collisions.right = (directionX == 1);
                    collisions.hitAreaBound = (hit.collider.tag == "AreaBound" || hit.collider.tag == "AreaEnd") ? true : false;
                }
            }
        }
    }
    // climb slope
    public void ClimbSlope(ref Vector3 velocity, float slopeAngle)
    {
        float moveDistance = Mathf.Abs(velocity.x);
        float climbVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance * GameControl.UpSlopeSpeedOffset;
        if (velocity.y <= climbVelocityY)
        {
            velocity.y = climbVelocityY;
            velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x) * GameControl.UpSlopeSpeedOffset;
            collisions.below = true;
            collisions.climbingSlope = true;
            collisions.slopeAngle = slopeAngle;
        }
    }
    // walk down slope
    public void DescendSlope(ref Vector3 velocity)
    {
        float directionX = Mathf.Sign(velocity.x);
        Vector2 rayOrigin = ((directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft);
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, 1, collisionMask);
        if (hit)
        {
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
            if (slopeAngle != 0 && slopeAngle <= maxDescendAngle)
            {
                if (Mathf.Sign(hit.normal.x) == directionX)
                {
                    if (hit.distance - SkinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x))
                    {
                        float moveDistance = Mathf.Abs(velocity.x);
                        float descendVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance * GameControl.DownSlopeSpeedOffset;
                        velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x) * GameControl.DownSlopeSpeedOffset;
                        velocity.y -= descendVelocityY;

                        collisions.slopeAngle = slopeAngle;
                        collisions.descendingSlope = true;
                        collisions.below = true;
                    }
                }
            }
        }

    }
    // normal move
    public virtual void Move()
    {

    }
    

    // check for vertical collision
    public void VerticalCollisions(ref Vector3 velocity)
    {
        float directionY = Mathf.Sign(velocity.y);
        float rayLength = Mathf.Abs(velocity.y) + SkinWidth;
        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, (phasing) ? collisionMaskPhasing : collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.red);

            if (hit)
            {
                velocity.y = (hit.distance - SkinWidth) * directionY;
                rayLength = hit.distance;

                if (collisions.climbingSlope)
                {
                    velocity.x = velocity.y / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x);
                }
                collisions.below = (directionY == -1);
                collisions.above = (directionY == 1);
            }
        }
        if (collisions.climbingSlope)
        {
            float directionX = Mathf.Sign(velocity.x);
            rayLength = Mathf.Abs(velocity.x) + SkinWidth;
            Vector2 rayOrigin = ((directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) + Vector2.up * velocity.y;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeAngle != collisions.slopeAngle)
                {
                    velocity.x = (hit.distance - SkinWidth) * directionX;
                    collisions.slopeAngle = slopeAngle;
                }
            }
        }
    }

    public void GroundCheck()
    {
        Vector2 rayOrigin = raycastOrigins.bottomLeft;
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, SkinWidth*2,  collisionMask);
        if (hit)
        {
            collisions.below = true;
        }

    }
    public void AddGravity(ref Vector3 velocity)
    {
        if (collisions.below || collisions.above)
        {
            velocity.y = 0;
        }
        if (!collisions.below)
        {
            if (velocity.y > GameControl.TerminalVelocity)
            {
                velocity.y += GameControl.Gravity * Time.deltaTime;
            }
            if (velocity.y <= GameControl.TerminalVelocity)
            {
                velocity.y = GameControl.TerminalVelocity * Time.deltaTime;
            }
            
        }

    }

    public void JumpCheck(Vector3 velocity)
    {
        VerticalCollisions(ref velocity);
    }
    public struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;

    }
  
    public struct CollisionInfo
    {
        public bool above, below, left, right;
        public int faceDirection;
        public bool hitAreaBound;
        public bool climbingSlope, descendingSlope;
        public float slopeAngle, slopeAngleOld;
        public Vector3 velocityOld;
        public void Reset()
        {
            above = below = left = right = false;
            hitAreaBound = false;
            climbingSlope = false;
            descendingSlope = false;
            slopeAngleOld = slopeAngle;
            slopeAngle = 0;
        }
    }

   

}
