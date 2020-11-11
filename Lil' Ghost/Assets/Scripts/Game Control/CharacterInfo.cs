using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RaycastController))]
[RequireComponent(typeof(BoxCollider2D))] 
public class CharacterInfo : MonoBehaviour
{
    [HideInInspector]
    public BoxCollider2D attackCollider;
    public Animator animator;
    public SpriteRenderer spriteRenderer;


    public int maxHits;
    public int maxEnergy;
    public float walkSpeed = .1f;
    public float jumpVelocity = .1f;
    public float bounceAmount = 1;
    public bool canRespawn = true;
    public int atkDamage;
    public float knockbackAmount;
    public float knockbackDuration = .1f;

    public int hitsLeft;
    public int energyLeft;
    public bool alive = true;
    
    public bool defeated = false;

    
    public Vector2 knockbackDirection;
    public bool isKnockbacked = false;
    
    public float knockbackTimer;

    
    public Vector3 startingPos;
    public bool OnScreen = false;
    

    public bool canJump = true;
    public float targetVelocityX;
    public float velocityXSmoothing;
    public float accelerationTimeInAir = .5f;
    public float accelerationTimeGround = .2f;
    public float accelerationTimeRoll = .06f;

    public Vector3 velocity;
    public bool canTurn = true;

    public GameObject target = null;


    public virtual void Start()
    {
        hitsLeft = maxHits;
        energyLeft = maxEnergy;
        attackCollider = GetComponent<BoxCollider2D>();
    }
    public virtual void Update()
    {
        if(Time.timeScale == 0)
        {
            return;
        }
    }
    public virtual void FaceRight()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;


    }
    public virtual void FaceLeft()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;

    }
    public virtual void TakeDamage(int amount)
    {
        Debug.Log("taken " + amount + " damage");
        hitsLeft -= amount;
        if(hitsLeft <= 0)
        {
            // die
            Death();
        }
    }
    public virtual void Death()
    {
        // run death animation 
        Debug.Log(this.name + " is dead");
        GetComponent<BoxCollider2D>().enabled = false;
    }
    public virtual void Respawn()
    {
        GetComponent<BoxCollider2D>().enabled = true;
    }

    public virtual void SetKnockback(Vector2 direction)
    {
        if(!isKnockbacked)
        {
            knockbackDirection = direction;
            knockbackTimer = knockbackDuration;
        }
        isKnockbacked = true;
    }
    public virtual Vector2 KnockbackDirection(Collider2D other)
    {
        float dirX = 0;
        float dirY = 0;
        if (transform.position.x >= other.transform.position.x)
        {
            dirX = -1;
        }
        else
        {
            dirX = 1;
        }
        if (transform.position.y >= other.transform.position.y)
        {
            dirY = -1;
        }
        else
        {
            dirY = 1;
        }
        return new Vector2(dirX, dirY);
    }
    
    public virtual void HitSpike()
    {
        hitsLeft = 0;
    }
}
