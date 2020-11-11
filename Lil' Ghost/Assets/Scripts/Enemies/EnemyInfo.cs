using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(EnemyController))]
public class EnemyInfo : CharacterInfo
{

    public Vector2 directionalInput;
    public EnemyController controller;
    public new SpriteRenderer renderer;
    public EnemyState currentState;
    public EnemyType enemyType;
    public StartDirection startDirection;
    public int currentMoveDirectionX;
    public int currentMoveDirectionY;
    public bool StopAtEdge = true;
    // ability info
    bool abilityActive = false;
    public float sightRange;
    public LayerMask sightMask;
    public float aggrovationTime;
    public float aggrovationTimer;
    public float abilityRange;
    public float abilityResetTime;
    public float abilityChargeTime;
    public float abilityDurationTime;
    float abilityResetTimer;
    float abilityChargeTimer;
    float abilityDurationTimer;
    bool abilityCharging;
    public BounderyManager bounderyManager;
    private EnemyState previousState;
    void OnEnable()
    {
        if (alive)
        {
            GameControl.ResetOnRest += Reset;
        }

    }
    void OnDisable()
    {
        if (alive)
        {
            GameControl.ResetOnRest -= Reset;
        }

    }


    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        controller = GetComponent<EnemyController>();
        renderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        SetStartDirection();
        currentState = EnemyState.Active;
        startingPos = transform.position;
        controller.EdgeDetectionActive = StopAtEdge;
    }

    // Update is called once per frame
    public override void Update()
    {
        if(Time.timeScale == 0 || currentState == EnemyState.Dead)
        {
            return;
        }

        if (bounderyManager.active)
        {
            OnScreen = true;
        }
        else
        {
            OnScreen = false;
        }
        if (aggrovationTimer <= 0 && !isKnockbacked)
        {
            currentState = EnemyState.Active;
        }
        if (!OnScreen)
        {
            transform.position = startingPos;
            currentState = EnemyState.Active;
            return;
        }
        switch (currentState)
        {
            case EnemyState.Active:
                {
                    controller.AddGravity(ref velocity);
                    if(controller.collisions.left || controller.collisions.right)
                    {
                        controller.collisions.faceDirection *= -1;
                    }
                    LineOfSight();
                    velocity.x = controller.collisions.faceDirection * walkSpeed *Time.deltaTime;

                    controller.Move(velocity);

                    break;
                }
            case EnemyState.Knockback:
                {
                    if(knockbackTimer <= 0 && controller.collisions.below)
                    {
                        // stop knockback
                        isKnockbacked = false;
                        velocity.y = 0;
                        currentState = previousState;
                    }
                    else if(knockbackTimer <= 0 && !controller.collisions.below)
                    {

                        controller.AddGravity(ref velocity);
                        velocity.x = knockbackAmount * knockbackDirection.x * Time.deltaTime;
                        controller.MoveKnockback(velocity);
                    }
                    else 
                    {
                        knockbackTimer -= Time.deltaTime;

                        velocity.y = ((knockbackAmount * knockbackDirection.y)/2) * Time.deltaTime;
                        velocity.x = knockbackAmount * knockbackDirection.x * Time.deltaTime;
                        controller.MoveKnockback(velocity);
                    }
                    break;
                }
            case EnemyState.Target:
                {
                    aggrovationTimer -= Time.deltaTime;
                    
                    switch (enemyType) 
                    {
                        case EnemyType.Jumping:
                            {

                                if (!abilityActive)
                                {

                                    if(Vector2.Distance(target.transform.position, transform.position) > abilityRange)
                                    {
                                        controller.AddGravity(ref velocity);
                                        if (abilityResetTimer > 0)
                                        {
                                            abilityResetTimer -= Time.deltaTime;
                                        }
                                        controller.collisions.faceDirection = (target.transform.position.x >= transform.position.x) ? 1 : -1;
                                        velocity.x = controller.collisions.faceDirection * walkSpeed * Time.deltaTime;
                                        controller.Move(velocity);
                                    }
                                    else if(Vector2.Distance(target.transform.position, transform.position) <= abilityRange)
                                    {
                                        if (abilityResetTimer > 0)
                                        {
                                            abilityResetTimer -= Time.deltaTime;
                                        }
                                        else if (abilityResetTimer <= 0)
                                        {
                                            controller.collisions.faceDirection = (target.transform.position.x >= transform.position.x) ? 1 : -1;
                                            abilityActive = true;
                                            abilityDurationTimer = abilityDurationTime;
                                        }
                                    }
                                }
                                else if (abilityActive)
                                {
                                    if(abilityDurationTimer > 0)
                                    {
                                        abilityDurationTimer -= Time.deltaTime;
                                        velocity.x = controller.collisions.faceDirection * walkSpeed *2 * Time.deltaTime;
                                        velocity.y = jumpVelocity * Time.deltaTime;
                                        controller.MoveWithOutEdgeDetection(velocity);
                                    }
                                    else if(abilityDurationTimer <= 0)
                                    {
                                        controller.AddGravity(ref velocity);
                                        if (!controller.collisions.below)
                                        {
                                            velocity.x = controller.collisions.faceDirection * walkSpeed *2 * Time.deltaTime;
                                            controller.MoveWithOutEdgeDetection(velocity);
                                        }
                                        else if (controller.collisions.below)
                                        {
                                            abilityResetTimer = abilityResetTime;
                                            abilityActive = false;
                                        }
                                    }
                                }
                                break;
                            }
                        case EnemyType.Sprint:
                            {
                                if(!abilityActive)
                                {

                                    if (Vector2.Distance(target.transform.position, transform.position) > abilityRange)
                                    {
                                        controller.AddGravity(ref velocity);
                                        if (abilityResetTimer > 0)
                                        {
                                            abilityResetTimer -= Time.deltaTime;
                                        }
                                        controller.collisions.faceDirection = (target.transform.position.x >= transform.position.x) ? 1 : -1;
                                        velocity.x = controller.collisions.faceDirection * walkSpeed * Time.deltaTime;
                                        controller.Move(velocity);
                                    }
                                    else if (Vector2.Distance(target.transform.position, transform.position) <= abilityRange)
                                    {
                                        if (abilityResetTimer > 0)
                                        {
                                            abilityResetTimer -= Time.deltaTime;
                                        }
                                        else if (abilityResetTimer <= 0)
                                        {
                                            controller.collisions.faceDirection = (target.transform.position.x >= transform.position.x) ? 1 : -1;
                                            abilityActive = true;
                                            abilityDurationTimer = abilityDurationTime;
                                        }
                                    }
                                }
                                else if(abilityActive)
                                {
                                    if (abilityDurationTimer > 0)
                                    {
                                        abilityDurationTimer -= Time.deltaTime;
                                        if (Vector2.Distance(target.transform.position, transform.position) > .1f)
                                        {
                                            if (abilityDurationTimer <= .2f)
                                            {
                                                abilityResetTimer = abilityResetTime;
                                                abilityActive = false;
                                            }
                                            controller.AddGravity(ref velocity);
                                            velocity.x = controller.collisions.faceDirection * walkSpeed * 2f * Time.deltaTime;
                                            controller.Move(velocity);
                                        }
                                        else if(Vector2.Distance(target.transform.position, transform.position) <= .1f)
                                        {
                                            if (abilityDurationTimer > .2f)
                                            {
                                                abilityDurationTimer = .2f;
                                            }
                                            controller.AddGravity(ref velocity);
                                            velocity.x = controller.collisions.faceDirection * walkSpeed * 2f * Time.deltaTime;
                                            controller.Move(velocity);
                                        }
                                        
                                    }
                                    else if (abilityDurationTimer <= 0)
                                    {
                                        abilityResetTimer = abilityResetTime;
                                        abilityActive = false;
                                    }
                                }
                                break;
                            }
                    }
                    break;
                }
            case EnemyState.Dead:
                {
                    break;
                }
            case EnemyState.Cutsceen:
                {
                    break;
                }
        }


        
    }
    // player touches the enemy
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Player")
        {
            if (!GameControl.control.playerStats.IsPhaseing)
            {
                GameControl.control.HitStop();
                Debug.Log("Player took " + atkDamage + " Damage" + " hits left: " + GameControl.control.playerStats.HitsLeft);
                other.gameObject.SendMessage("TakeDamage", atkDamage);
                other.gameObject.SendMessage("SetKnockback", KnockbackDirection(other));
            }
        }
    }

    public override void Death()
    {
        base.Death();
        GameControl.control.Shake(.15f, .1f);
        currentState = EnemyState.Dead;
        // DISABLE: this is is for hideing when dead
        GetComponent<MeshRenderer>().enabled =false;

    }
    void Reset()
    {
        transform.position = startingPos;
        hitsLeft = maxHits;
        energyLeft = maxEnergy;
        OnScreen = false;
        aggrovationTimer = 0;
        currentState = EnemyState.Active;
        target = null;
        GetComponent<MeshRenderer>().enabled = true;
        SetStartDirection();
        Respawn();
    }
    public override void SetKnockback(Vector2 direction)
    {
        base.SetKnockback(direction);
        previousState = currentState;
        currentState = EnemyState.Knockback;
        controller.collisions.below = false;

    }
    public void SetStartDirection()
    {
        switch (startDirection)
        {
            case StartDirection.Left:
                {
                    currentMoveDirectionX = -1;
                    currentMoveDirectionY = 0;
                    break;
                }
            case StartDirection.Up:
                {
                    currentMoveDirectionX = 0;
                    currentMoveDirectionY = 1;
                    break;
                }
            case StartDirection.Right:
                {
                    currentMoveDirectionX = 1;
                    currentMoveDirectionY = 0;
                    break;
                }
            case StartDirection.Down:
                {
                    currentMoveDirectionX = 0;
                    currentMoveDirectionY = -1;
                    break;
                }
        }
    }

    public void LineOfSight()
    {
        if (sightRange != 0)
        {
            float directionX = controller.collisions.faceDirection;
            float rayLength = sightRange;
            Vector2 rayOrigin = (directionX == -1) ? controller.raycastOrigins.bottomLeft+Vector2.up*transform.localScale/2 : controller.raycastOrigins.bottomRight+Vector2.up*transform.localScale/2;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength,sightMask);
            Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);

            if (hit)
            {
                if(hit.collider.tag == "Player" && aggrovationTimer <=0)
                {
                    currentState = EnemyState.Target;
                    aggrovationTimer = aggrovationTime;
                    target = hit.collider.gameObject;
                }
            }
        }
        
        
    }

    public override void HitSpike()
    {
        hitsLeft = 0;
        Death();
    }
}
