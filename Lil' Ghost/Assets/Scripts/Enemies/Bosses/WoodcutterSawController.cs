using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodcutterSawController : RaycastController
{
    public float speed;
    Vector3 velocity;
    public WoodcutterInfo owner;
    public int direction;
    public int damage;
    public float initalVelocityY;
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        velocity.y = initalVelocityY;
    }

    // Update is called once per frame
    void Update()
    {
        
        AddGravity(ref velocity);
        velocity.x = speed * direction * Time.deltaTime;
        
        Move();

        if (collisions.above || collisions.below || collisions.left || collisions.right)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            if (!GameControl.control.playerStats.IsPhaseing )
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
        float dirY = 1;
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

    
}
