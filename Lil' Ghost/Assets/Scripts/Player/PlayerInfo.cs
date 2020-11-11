using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(PlayerConroller))]



public class PlayerInfo : CharacterInfo
{
    public Vector2 directionalInput;
    public PlayerConroller controller;
    public new SpriteRenderer renderer;

    public PlayerState currentPlayerState;
    
    public bool canRecharge = true;
    public bool energyFullDrain = false;

    public bool atRestPoint = false;
    public bool inWall = false;
    public bool isAttacking = false;

    Vector3 moveToPoint;
    Vector3 velocityToPoint;
    float angleToPoint;
    Vector3 hitSpikePoint;

    bool hasReset = false;
    public int lastFaceDirection;
    float attackPressedCheck;
    float attackPressedCheckTimer = 0.1f;
    public float groundedCheck;
    float groundedCheckTimer = .1f;
    float energyRechargeDelay;
    float energyRechargeDelayTimer = 1f;

    bool setJumpDelay = false;
    float menuJumpDelay = .01f;



   
    // attack bounce information
    float bounceVelocity;
    bool attackBounce = false;
    bool attackBounceJump = false;
    float attackBounceTime;
    float attackBounceTimer = 0.3f;
    // Jump information
    float jumpPressedCheck;
    float jumpPressedCheckTimer = 0.1f;
    public bool canDoubleJump = false;
    public bool isJumping = false;
    public bool isDoubleJumping = false;
    public float minimumJumpTime = .05f;
    public float maximumJumpTime = .27f;
    float doubleJumpCounter;
    float jumpTimeCounter;
    // dash information
    public bool canDash = false;
    public float dashTime;
    public float dashTimer;
    public float dashSpeed;




    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        controller = GetComponent<PlayerConroller>();
        renderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        FaceRight();
        isJumping = false;
            
    }

    // Update is called once per frame
    public override void Update()
    {
        currentPlayerState = GameControl.control.playerStats.PlayerState;

        if (GameControl.control.inMenu || GameControl.control.inInventroy)
        {
            setJumpDelay = true;
            GameControl.control.playerStats.PlayerState = PlayerState.Menu;
        }
        else
        {
            if (setJumpDelay)
            {
                StartCoroutine(MenuJumpDelay());
            }
        }
        currentPlayerState = GameControl.control.playerStats.PlayerState;
        if (Time.timeScale == 0 || GameControl.control.currentScene == 0)
        {
            return;
        }
        //movement input
        directionalInput = GameControl.control.gameInput;

        if (atRestPoint && groundedCheck > 0 && GameControl.control.firstDownLS)
        {
            Debug.Log("Rest");
            GameControl.control.playerStats.PlayerState = PlayerState.Rest;

        }


        if (isKnockbacked)
        {
            GameControl.control.playerStats.PlayerState = PlayerState.Knockback;
        }
        //check if at 0 health
        if (GameControl.control.playerStats.HitsLeft <= 0)
        {
            isKnockbacked = false;
            GameControl.control.playerStats.PlayerState = PlayerState.Dead;
        }

        switch (GameControl.control.playerStats.PlayerState)
        {
            case PlayerState.Walking:
                {
                    int wallDirX = (controller.collisions.left) ? -1 : 1;

                    // add gravity
                    if (!isJumping && !isDoubleJumping && !attackBounceJump)
                    {
                        controller.AddGravity(ref velocity);
                    }
                    if (canTurn)
                    {
                        if (directionalInput.x > 0 && controller.collisions.faceDirection != 1)
                        {
                            FaceRight();

                        }
                        else if (directionalInput.x < 0 && controller.collisions.faceDirection != -1)
                        {
                            FaceLeft();

                        }
                    }
                    else
                    {
                        if (directionalInput.x > 0 )
                        {
                            controller.collisions.faceDirection = 1;
                        }
                        else if (directionalInput.x < 0)
                        {
                            controller.collisions.faceDirection = -1;
                        }
                    }

                    // timer for checking if still grounded
                    if (controller.collisions.below)
                    {
                        groundedCheck = groundedCheckTimer;
                        canDoubleJump = true;
                    }
                    if (groundedCheck > 0)
                    {
                        groundedCheck -= Time.deltaTime;
                    }

                    /// Jump code
                    if (Input.GetButtonDown("Jump"))
                    {
                        jumpPressedCheck = jumpPressedCheckTimer;
                        
                    }
                    if(jumpPressedCheck > 0)
                    {
                        jumpPressedCheck -= Time.deltaTime;
                        if (groundedCheck > 0)
                        {
                            if (!isJumping && canJump)
                            {
                                jumpPressedCheck = 0;
                                groundedCheck = 0;
                                jumpTimeCounter = 0;
                                isJumping = true;
                                isDoubleJumping = false;
                            }
                        }
                        else
                        {
                            if (!isDoubleJumping && GameControl.control.playerStats.HasDoubleJump && canDoubleJump)
                            {
                                jumpPressedCheck = 0;
                                doubleJumpCounter = 0;
                                isJumping = false;
                                isDoubleJumping = true;
                            }
                        }
                    }
                    if (isJumping)
                    {
                        if (Input.GetButton("Jump"))
                        {
                            if (jumpTimeCounter < maximumJumpTime)
                            {
                                jumpTimeCounter += Time.deltaTime;
                                velocity.y = jumpVelocity * Time.deltaTime;
                            }
                            else
                            {
                                isJumping = false;
                            }
                        }
                        else if (!Input.GetButton("Jump"))
                        {
                            if (jumpTimeCounter < minimumJumpTime)
                            {
                                jumpTimeCounter += Time.deltaTime;
                                velocity.y = jumpVelocity * .1f * Time.deltaTime;
                            }
                            else
                            {
                                isJumping = false;
                            }
                        }
                    }
                    if (isDoubleJumping)
                    {
                        canDoubleJump = false;
                        if (Input.GetButton("Jump"))
                        {
                            if (doubleJumpCounter < maximumJumpTime)
                            {
                                doubleJumpCounter += Time.deltaTime;
                                velocity.y = jumpVelocity * Time.deltaTime;
                            }
                            else
                            {
                                isDoubleJumping = false;
                            }
                        }
                        else if (!Input.GetButton("Jump"))
                        {
                            if (doubleJumpCounter < minimumJumpTime)
                            {
                                doubleJumpCounter += Time.deltaTime;
                                velocity.y = jumpVelocity * .1f * Time.deltaTime;
                            }
                            else
                            {
                                isDoubleJumping = false;
                            }
                        }
                    }

                    // dash input
                    if (Input.GetButtonDown("Dash") && canDash)
                    {
                        // dash state
                    }
                    else
                    {
                        targetVelocityX = directionalInput.x * walkSpeed;
                    }
                    // phase input
                    if (Input.GetAxisRaw("Phase") != 0 && (GameControl.control.playerStats.EnergyLeft > 0) && !energyFullDrain)
                    {
                        GameControl.control.playerStats.IsPhaseing = true;
                        renderer.color = new Color(1f, 1f, 1f, .3f);
                    }
                    if (Input.GetAxisRaw("Phase") == 0 || (GameControl.control.playerStats.EnergyLeft <= 0) || energyFullDrain)
                    {

                        if (!inWall)
                        {
                            if (GameControl.control.playerStats.IsPhaseing)
                            {
                                canRecharge = false;
                                energyRechargeDelay = energyRechargeDelayTimer;
                            }
                            GameControl.control.playerStats.IsPhaseing = false;
                            renderer.color = new Color(1f, 1f, 1f, 1f);
                        }
                        else if (inWall)
                        {

                        }
                    }
                    
                    // shoot ectoplasm input
                    // Aether Mode Input



                    // while phaeing decreese energy
                    if (GameControl.control.playerStats.IsPhaseing)
                    {
                        GameControl.control.playerStats.EnergyLeft -= GameControl.control.playerStats.PhaseCost * Time.deltaTime;
                        if (GameControl.control.playerStats.EnergyLeft <= 0)
                        {
                            energyFullDrain = true;
                        }

                    }
                    else if (!GameControl.control.playerStats.IsPhaseing && canRecharge && GameControl.control.playerStats.EnergyLeft < GameControl.control.playerStats.MaxEnergy)
                    {
                        GameControl.control.playerStats.EnergyLeft += GameControl.control.playerStats.EnergyRechargeRate * Time.deltaTime;
                        if (GameControl.control.playerStats.EnergyLeft >= GameControl.control.playerStats.MaxEnergy)
                        {
                            GameControl.control.playerStats.EnergyLeft = GameControl.control.playerStats.MaxEnergy;
                        }

                    }
                    else if (!canRecharge)
                    {
                        energyRechargeDelay -= Time.deltaTime;
                        if (energyRechargeDelay <= 0)
                        {
                            canRecharge = true;
                        }

                    }
                    if (energyFullDrain && (GameControl.control.playerStats.EnergyLeft == GameControl.control.playerStats.MaxEnergy))
                    {
                        energyFullDrain = false;
                    }

                    if (isJumping && controller.collisions.above)
                    {
                        isJumping = false;
                    }
                    //move left and right

                    velocity.x = directionalInput.x * walkSpeed *Time.deltaTime;



                    SetAnimParameters();

                    controller.Move(velocity, directionalInput, GameControl.control.playerStats.IsPhaseing);

                    break;
                }
            case PlayerState.Dash:
                {

                    break;
                }
            case PlayerState.Cutsceen:
                {
                    break;
                }
            case PlayerState.MoveTo:
                {
                    
                    return;
                }
            case PlayerState.Knockback:
                {
                    if (knockbackTimer <= 0 && controller.collisions.below)
                    {
                        // stop knockback
                        isKnockbacked = false;
                        velocity.y = 0;
                        GameControl.control.playerStats.PlayerState = PlayerState.Walking;
                    }
                    else if (knockbackTimer <= 0 && !controller.collisions.below)
                    {

                        controller.AddGravity(ref velocity);
                        velocity.x = knockbackAmount * knockbackDirection.x * Time.deltaTime;
                        controller.MoveKnockback(velocity);
                    }
                    else
                    {
                        knockbackTimer -= Time.deltaTime;

                        velocity.y = (knockbackAmount * knockbackDirection.y) / 2 * Time.deltaTime;
                        velocity.x = knockbackAmount * knockbackDirection.x * Time.deltaTime;
                        controller.MoveKnockback(velocity);
                    }
                    break;
                }
            case PlayerState.Dead:
                {
                    //play death animation
                    //resawn at the previous rest point
                    Respawn();
                    break;
                }
            case PlayerState.Rest:
                {
                    velocity.x = 0;
                    velocity.y = 0;
                    if (!hasReset)
                    {
                        hasReset = true;
                        controller.collisions.Reset();
                        GameControl.control.Rest();
                        
                    }
                    if (Input.GetButtonDown("Cancel") && hasReset == true)
                    {
                        GameControl.control.playerStats.PlayerState = PlayerState.Walking;
                        controller.AddGravity(ref velocity);
                        hasReset = false;
                    }
                    break;
                }
            case PlayerState.Menu:
                {
                    velocity.x = 0;
                    controller.AddGravity(ref velocity);
                    controller.Move(velocity, directionalInput, GameControl.control.playerStats.IsPhaseing);
                    break;
                }
            case PlayerState.Map:
                {
                    //movement input
                    directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

                    if (canTurn)
                    {
                        if (directionalInput.x > 0 && controller.collisions.faceDirection != 1)
                        {
                            FaceRight();

                        }
                        else if (directionalInput.x < 0 && controller.collisions.faceDirection != -1)
                        {
                            FaceLeft();

                        }
                    }
                    velocity.x = directionalInput.x * walkSpeed * Time.deltaTime;




                    SetAnimParameters();

                    controller.Move(velocity, directionalInput, GameControl.control.playerStats.IsPhaseing);

                    break;
                }
        }

        
    }

    
    private void SetAnimParameters()
    {
        animator.SetFloat("HorizontalSpeed", Math.Abs(velocity.x));
    }

    public override void FaceRight()
    {
        base.FaceRight();
        controller.collisions.faceDirection = 1;
    }

    public override void FaceLeft()
    {
        base.FaceLeft();
        controller.collisions.faceDirection = -1;
    }

    public override void TakeDamage(int amount)
    {
        Debug.Log("taken " + amount + " damage");
        GameControl.control.playerStats.HitsLeft -= amount;
    }
    public override void HitSpike()
    {
        GameControl.control.playerStats.HitsLeft -= 1;
    }
    public override void SetKnockback(Vector2 direction)
    {
        base.SetKnockback(direction);
    }
    //Move to a point
    public void MoveToPoint(Vector3 point)
    {
        if(GameControl.control.playerStats.HitsLeft > 0)
        {
            transform.position = point;
            GameControl.control.HitStop();
        }
        
    }

    IEnumerator MenuJumpDelay()
    {
        isJumping = false;
        isDoubleJumping = false;
        setJumpDelay = false;
        canJump = false;
        yield return new WaitForSecondsRealtime(menuJumpDelay);
        canJump = true;
        StopCoroutine(MenuJumpDelay());
    }

    public override void Respawn()
    {
        
        controller.collisions.Reset();
        GameControl.control.Respawn();
    }
}
