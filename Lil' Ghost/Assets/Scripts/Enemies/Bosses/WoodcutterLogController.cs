using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodcutterLogController : RaycastController
{
    public float speed;
    Vector3 velocity;

    public WoodcutterInfo owner;

    public int direction;
    public int damage;

    
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        if (owner != null && owner.state == WoodcutterState.Inactive)
        {
            Destroy(this.gameObject);
        }

        AddGravity(ref velocity);
        velocity.x = direction * speed * Time.deltaTime;
        WallDetection(ref velocity);
        Move();
        if(collisions.left || collisions.right)
        {
            Destroy(this.gameObject);
        }
    }

    public override void Move()
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
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            if (!GameControl.control.playerStats.IsPhaseing)
            {
                GameControl.control.HitStop();
                Debug.Log("Player took " + damage + " Damage");
                other.gameObject.SendMessage("TakeDamage", damage);
                other.gameObject.SendMessage("SetKnockback", KnockbackDirection(other));
            }
        }
        
    }

    public Vector2 KnockbackDirection(Collider2D other)
    {
        float dirX = 0;
        float dirY = 3;
        if (transform.position.x >= other.transform.position.x)
        {
            dirX = -1f;
        }
        else
        {
            dirX = 1f;
        }

        return new Vector2(dirX, dirY);
    }


    void WallDetection(ref Vector3 velocity)
    {
        float directionX = collisions.faceDirection;
        float rayLength = 2 * SkinWidth;
        Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.topLeft : raycastOrigins.topRight;
        RaycastHit2D wallCheck = Physics2D.Raycast(rayOrigin, Vector2.right*directionX, rayLength, collisionMask);

        Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.green);

        if (wallCheck)
        {
            Destroy(this.gameObject);
        }
    }
}
