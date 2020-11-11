using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class PlayerHitbox : MonoBehaviour
{
    private BoxCollider2D attackCollider;
    public PlayerInfo player;
    public bool atkDown;
    // Start is called before the first frame update
    void Start()
    {
        attackCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!GameControl.control.playerStats.IsPhaseing)
        {
            if(other.gameObject.tag == "HitInteract")
            {
                other.gameObject.SendMessage("Hit");
            }
            else if(other.gameObject.tag == "Enemy" || other.gameObject.tag == "Destructible")
            {
                // send damage
                other.gameObject.SendMessage("TakeDamage", GameControl.control.playerStats.Damage);
                
                if (other.gameObject.tag == "Enemy")
                {
                    //send knockback
                    if (atkDown)
                    {
                        float boucneMod = other.GetComponent<EnemyInfo>().bounceAmount;
                        player.velocity.y = player.jumpVelocity * Time.deltaTime * boucneMod;
                    }
                    other.gameObject.SendMessage("SetKnockback", KnockbackDirection(other));
                    GameControl.control.HitStop();
                }
                else if (other.gameObject.tag == "Destructible")
                {
                    if (atkDown)
                    {
                        float boucneMod = other.GetComponent<DestructibleObject>().bounceAmount;
                        player.velocity.y = player.jumpVelocity * Time.deltaTime * boucneMod;
                    }
                    
                }
            }
        }
        
    }

    public Vector2 KnockbackDirection(Collider2D other)
    {
        float dirX = 0;
        float dirY = 0;
        if(player.transform.position.x >= other.transform.position.x)
        {
            dirX = -1;
        }
        else
        {
            dirX = 1;
        }
        if (player.transform.position.y >= other.transform.position.y)
        {
            dirY = 1;
        }
        else
        {
            dirY = -1;
        }
        return new Vector2(dirX, dirY);
    }
}
