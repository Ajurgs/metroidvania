using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(WoodcutterController))]

public class WoodcutterInfo : CharacterInfo
{
    public GameObject enterDoor;
    public GameObject exitDoor;
    public bool playInro = true;
    public WoodcutterController controller;
    public WoodcutterState state = WoodcutterState.Inactive;
    public float turnTime;

    public float jumpXSpeed;

    public GameObject hitboxAxeAttack;
    public GameObject woodLogPrefab;
    public GameObject sawBladePrefab;
    public Vector3 logSpawnPoint;
    public Vector3 sawSpawnPoint;
    float turnTimer;

    public float attackResetTime;
    float attackResetTimer;
    public float logKickResetTime;
    float logKickResetTimer;
    public float sawThrowResetTime;
    float sawThrowResetTimer;
    public float jumpDuration;
    float jumpTimer;
    public float jumpResetTime;
    float jumpResetTimer;
    public float actionCooldownTime;
    float actionCooldownTimer;

    public int phaseTwoHits;
    public bool inPhaseTwo = false;

    int jumpDirection;
    
    public bool canAct = false;
    bool canAttack = true;
    bool canLogKick= true;
    bool canSawThrow = true;

    bool jumpStartAnimation = false;

    int sameAttackCounter;
    public int sameAttackLimit;

    float sawVelocityX = 30f;
    float sawVelocityY = .45f;
    int sawNumber = 1;
    public bool secondProjectile = false;
    public bool thirdProjectile = false;

    public Color normalColor = new Color(255, 255, 255, 255);
    public Color hitColor = new Color(135, 0, 0, 255);

    void OnEnable()
    {
        if (alive)
        {
            GameControl.ResetOnRest += Reset;
        }
        
    }
    void OnDisable()
    {
        if(alive)
        {
            GameControl.ResetOnRest -= Reset;
        }
        
    }
    // Start is called before the first frame update
    public override void Start()
    {
        controller = GetComponent<WoodcutterController>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        startingPos = transform.position;
        hitsLeft = maxHits;
        controller.collisions.faceDirection = -1;
        playInro = !GameControl.control.woodcutterEncountered;
        knockbackDuration = .05f;
        DeactivaeAttack();
        StartingFaceDirection();
    }
    // Update is called once per frame
    public override void Update()
    {
        if(hitsLeft <= phaseTwoHits && !inPhaseTwo)
        {
            SetPhaseTwo();
        }
        if (isKnockbacked && (state != WoodcutterState.Knockback || state != WoodcutterState.Jump))
        {
            state = WoodcutterState.Knockback;
        }
        AttackCooldown();
        switch (state)
        {
            case WoodcutterState.Inactive:
                {
                    controller.AddGravity(ref velocity);

                    break;
                }
            case WoodcutterState.Into:
                {
                    controller.AddGravity(ref velocity);
                    // play into animation and cutsene 
                    GameControl.control.woodcutterEncountered = true;
                    break;
                }
            case WoodcutterState.Walk:
                {
                    if (target != null)
                    {
                        //controller.collisions.faceDirection = (target.transform.position.x >= transform.position.x) ? 1 : -1;

                        if(target.transform.position.x >= transform.position.x)
                        {
                            if(controller.collisions.faceDirection == -1)
                            {
                                state = WoodcutterState.Turn;
                                turnTimer = turnTime;
                            }
                        }
                        else
                        {
                            if (controller.collisions.faceDirection == 1)
                            {
                                state = WoodcutterState.Turn;
                                turnTimer = turnTime;
                            }
                        }
                        controller.AddGravity(ref velocity);
                        velocity.x = controller.collisions.faceDirection * walkSpeed * Time.deltaTime;
                        // choose what action is preformed


                        if (Mathf.Abs(transform.position.x - target.transform.position.x) <= 1.8f)
                        {
                            velocity.x = 0;
                            AttackCooldown();
                        }

                        if (canAct)
                        {
                            Debug.Log("Takes action");
                            decideAction(Mathf.Abs(transform.position.x - target.transform.position.x));
                        }

                        SetAnimParameters();
                        controller.Move(velocity);
                        

                    }
                    else
                    {
                        Reset();
                    }

                    break;
                }
            case WoodcutterState.Turn:
                {
                    if(turnTimer > 0.1)
                    {
                        turnTimer -= Time.deltaTime;
                        controller.AddGravity(ref velocity);
                        velocity.x = controller.collisions.faceDirection * walkSpeed * Time.deltaTime;
                        controller.Move(velocity);
                    }
                    else if(turnTimer < 0.1 && turnTimer > 0)
                    {
                        turnTimer -= Time.deltaTime;
                    }
                    else if (turnTimer <= 0)
                    {
                        if(controller.collisions.faceDirection == -1)
                        {
                            FaceRight();
                        }
                        else if (controller.collisions.faceDirection == 1)
                        {
                            FaceLeft();
                        }
                    }
                    break;
                }
            case WoodcutterState.Knockback:
                {
                    if (knockbackTimer <= 0 && controller.collisions.below)
                    {
                        // stop knockback
                        isKnockbacked = false;
                        velocity.y = 0;
                        state = WoodcutterState.Walk;
                    }
                    else if (knockbackTimer <= 0 && !controller.collisions.below)
                    {

                        controller.AddGravity(ref velocity);
                        velocity.x = knockbackAmount * knockbackDirection.x * Time.deltaTime;
                        controller.Move(velocity);
                    }
                    else
                    {
                        knockbackTimer -= Time.deltaTime;

                        velocity.y = ((knockbackAmount * knockbackDirection.y) / 2) * Time.deltaTime;
                        velocity.x = knockbackAmount * knockbackDirection.x * Time.deltaTime;
                        controller.Move(velocity);
                    }
                    break;
                }
            case WoodcutterState.AxeAttack:
                {
                    attackResetTimer = attackResetTime;
                    velocity.x = 0;
                    SetAnimParameters();
                    break;
                }
            case WoodcutterState.LogKick:
                {
                    
                    logKickResetTimer = logKickResetTime;
                    velocity.x = 0;
                    SetAnimParameters();
                    break;
                }
            case WoodcutterState.SawToss:
                {
                    
                    sawThrowResetTimer = logKickResetTime;
                    velocity.x = 0;
                    SetAnimParameters();
                    break;
                }
            case WoodcutterState.Jump:
                {
                    if (jumpStartAnimation)
                    {

                    }
                    else
                    {
                        if (jumpTimer > 0)
                        {
                            jumpTimer -= Time.deltaTime;
                            velocity.x = jumpDirection * jumpXSpeed * Time.deltaTime;
                            velocity.y = jumpVelocity * Time.deltaTime;
                            controller.Move(velocity);
                        }
                        if (jumpTimer <= 0)
                        {
                            controller.AddGravity(ref velocity);
                            if (!controller.collisions.below)
                            {
                                velocity.x = jumpDirection * jumpXSpeed * Time.deltaTime;     
                            }
                            else if (controller.collisions.below)
                            { 
                                velocity.x = 0;
                                JumpLand();
                            }
                            controller.Move(velocity);
                        }
                    }
                    break;
                }
            case WoodcutterState.Dead:
                {
                    velocity.x = 0;
                    break;
                }
        }
    }

    public override void TakeDamage(int amount)
    {
        StartCoroutine(flashOnHit());
        base.TakeDamage(amount);
    }
    IEnumerator flashOnHit()
    {
        spriteRenderer.color = hitColor;
        yield return new WaitForSecondsRealtime(.05f);
        spriteRenderer.color = normalColor;
        StopCoroutine(flashOnHit());
    }
    private void SetAnimParameters()
    {
        animator.SetFloat("HorizontalSpeed", Math.Abs(velocity.x));
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (!GameControl.control.playerStats.IsPhaseing)
            {
                GameControl.control.HitStop();
                Debug.Log("Player took " + atkDamage + " Damage");
                other.gameObject.SendMessage("TakeDamage", atkDamage);
                other.gameObject.SendMessage("SetKnockback", KnockbackDirection(other));
            }
        }
    }
    public override Vector2 KnockbackDirection(Collider2D other)
    {
        float dirX = 0;
        float dirY = 2;
        if (transform.position.x >= other.transform.position.x)
        {
            dirX = -2f;
        }
        else
        {
            dirX = 2f;
        }

        return new Vector2(dirX, dirY);
    }
    public override void SetKnockback(Vector2 direction)
    {
        if (state != WoodcutterState.Jump)
        {
            base.SetKnockback(direction);
            controller.collisions.below = false;
        }
    }
    void AttackCooldown()
    {
        if(actionCooldownTimer > 0 && state == WoodcutterState.Walk)
        {
            actionCooldownTimer -= Time.deltaTime;
        }
        if (actionCooldownTimer <= 0)
        {
            canAct = true;
        }
        if (state != WoodcutterState.AxeAttack && attackResetTimer > 0)
        {
            attackResetTimer -= Time.deltaTime;
        }
        else if (state != WoodcutterState.AxeAttack && attackResetTimer <= 0)
        {
            canAttack = true;
        }
        if (state != WoodcutterState.LogKick && logKickResetTimer > 0)
        {
            logKickResetTimer -= Time.deltaTime;
        }
        else if (state != WoodcutterState.LogKick && logKickResetTimer <= 0)
        {
            canLogKick = true;
        }
        if (state != WoodcutterState.SawToss && sawThrowResetTimer > 0)
        {
            sawThrowResetTimer -= Time.deltaTime;
        }
        else if (state != WoodcutterState.SawToss && sawThrowResetTimer <= 0)
        {
            canSawThrow = true;
        }
        if (state != WoodcutterState.Jump && jumpResetTimer > 0)
        {
            jumpResetTimer -= Time.deltaTime;
        }
        else if (state != WoodcutterState.Jump && jumpResetTimer <= 0)
        {
            canJump = true;
        }
    }
    void decideAction(float distanceToTarget)
    {
        canAct = false;
        if (inPhaseTwo)
        {
            actionCooldownTimer = actionCooldownTime/2;
        }
        else
        {
            actionCooldownTimer = actionCooldownTime;
        }
        
        if (inPhaseTwo)
        {
            secondProjectile = true;
        }
        if (Mathf.Abs(transform.position.x - target.transform.position.x) > 15)
        {
            if (canJump)
            {
                Jump((transform.position.x > target.transform.position.x) ? -1 : 1);
                return;
            }
            else if (canLogKick)
            {
                LogKick();
                return;
            }
        }
        else if (Mathf.Abs(transform.position.x - target.transform.position.x) > 9 && Mathf.Abs(transform.position.x - target.transform.position.x) <= 15)
        {
            if (canSawThrow)
            {
                SawThrow();
                return;
            }
        }
        else if (Mathf.Abs(transform.position.x - target.transform.position.x) > 5.5 && Mathf.Abs(transform.position.x - target.transform.position.x) <= 9)
        {
            if (canJump)
            {
                int rndNum = UnityEngine.Random.Range(-1, 1);

                if (Mathf.Sign(rndNum) > 0)
                {
                    Jump(-controller.collisions.faceDirection);
                    return;
                }
                else
                {
                    Jump(controller.collisions.faceDirection);
                    return;
                }

            }
            else if (Mathf.Abs(transform.position.x - target.transform.position.x) > 8)
            {
                if (canLogKick)
                {
                    LogKick();
                    return;
                }
            }
        }
        else if (Mathf.Abs(transform.position.x - target.transform.position.x) <= 5.5)
        {

            if (sameAttackCounter == sameAttackLimit)
            {
                int rndNum = UnityEngine.Random.Range(-1, 1);

                if (Mathf.Sign(rndNum) > 0)
                {
                    Jump(-controller.collisions.faceDirection);

                }
                else
                {
                    Jump(controller.collisions.faceDirection);
                }
                sameAttackCounter = 0;
                return;
            }
            else if (canAttack)
            {
                AxeAttack();
                sameAttackCounter += 1;
                return;
            }
        }
    }
    public void AxeHitbox()
    {
        hitboxAxeAttack.SetActive(true);
    }
    public void DeactivaeAttack()
    {
        if (inPhaseTwo)
        {
            if (secondProjectile)
            {
                Debug.Log("set second projectile to false");
                hitboxAxeAttack.SetActive(false);
                if (state == WoodcutterState.SawToss)
                {
                    state = WoodcutterState.Walk;
                    thirdProjectile = true;
                    SawThrow(2);
                }
                else if (state == WoodcutterState.LogKick)
                {
                    state = WoodcutterState.Walk;
                    LogKick();
                }
                else
                {
                    state = WoodcutterState.Walk;
                }
                secondProjectile = false;
            }
            else if (thirdProjectile)
            {
                
                state = WoodcutterState.Walk;
                SawThrow(3);
                thirdProjectile = false;
            }
            else
            {
                hitboxAxeAttack.SetActive(false);
                state = WoodcutterState.Walk;
            }
            
        }
        else
        {
            hitboxAxeAttack.SetActive(false);
            state = WoodcutterState.Walk;
        }
            
    }
    public void SpawnLog()
    {
        Vector3 adjustment = new Vector3(logSpawnPoint.x * controller.collisions.faceDirection, logSpawnPoint.y, 0);
        
        GameObject woodLog = Instantiate(woodLogPrefab, transform.position + adjustment , transform.rotation);
        woodLog.GetComponent<WoodcutterLogController>().direction = controller.collisions.faceDirection;
        woodLog.GetComponent<WoodcutterLogController>().owner = this;

    }
    public void SpawnSaw()
    {
        Vector3 adjustment = new Vector3(sawSpawnPoint.x * controller.collisions.faceDirection * -1, sawSpawnPoint.y, 0);
        GameObject sawBlade = Instantiate(sawBladePrefab, transform.position + adjustment, transform.rotation);
        sawBlade.GetComponent<WoodcutterSawController>().direction = controller.collisions.faceDirection;
        sawBlade.GetComponent<WoodcutterSawController>().owner = this;
        if(sawNumber == 1)
        {
            sawBlade.GetComponent<WoodcutterSawController>().speed = sawVelocityX;
            sawBlade.GetComponent<WoodcutterSawController>().initalVelocityY = sawVelocityY;
        }
        else if(sawNumber == 2)
        {
            sawBlade.GetComponent<WoodcutterSawController>().speed = sawVelocityX * 1.5f;
            sawBlade.GetComponent<WoodcutterSawController>().initalVelocityY = sawVelocityY * 1.2f;
            
        }
        else if(sawNumber == 3)
        {
            sawBlade.GetComponent<WoodcutterSawController>().speed = sawVelocityX * .8f;
            sawBlade.GetComponent<WoodcutterSawController>().initalVelocityY = sawVelocityY * .8f;
        }
    }
    public void JumpLand()
    {
        animator.SetTrigger("EndJump");
    }
    public void EndJump()
    {
        ResetAnimation();
        jumpResetTimer = jumpResetTime;
        if (!inPhaseTwo)
        {
            if (canLogKick || canSawThrow)
            {
                if (canLogKick && canSawThrow)
                {
                    int rndNum = UnityEngine.Random.Range(-1, 1);
                    if (rndNum > 0)
                    {
                        LogKick();
                    }
                    else
                    {
                        SawThrow();
                    }
                }
                else if (!canSawThrow)
                {
                    LogKick();
                }
                else if (!canLogKick)
                {
                    SawThrow();
                }
            }
            else
            {
                state = WoodcutterState.Walk;
            }
        }
        else
        {
            canAct = false;
            actionCooldownTimer = actionCooldownTime;
            int rndNum = UnityEngine.Random.Range(-1, 1);
            if (rndNum > -.25f)
            {
                LogKick();
            }
            else
            {
                SawThrow();
            }
        }
    }
    public override void Death()
    {
        StopAllCoroutines();
        alive = false;
        base.Death();
        state = WoodcutterState.Dead;
        enterDoor.SendMessage("Unlock");
        exitDoor.SendMessage("Unlock");
        GameControl.ResetOnRest -= Reset;
        GameControl.control.woodcutterDefeated = true;
        Destroy(this.gameObject);
    }
    void Reset()
    {
        Debug.Log("woodcutter reset");
        transform.position = startingPos;
        hitsLeft = maxHits;
        energyLeft = maxEnergy;
        velocity.x = 0;
        OnScreen = false;
        jumpStartAnimation = false;
        inPhaseTwo = false;
        StartingFaceDirection();
        animator.SetTrigger("EndJump");
        ResetAnimation();
        state = WoodcutterState.Inactive;
        animator.Play("Idle");
        sameAttackCounter = 0;
        SetAnimParameters();
        Debug.Log(state);
    }
    void Activate()
    {
        if(state == WoodcutterState.Inactive)
        {
            if (playInro)
            {
                animator.SetTrigger("PlayIntro");
                state = WoodcutterState.Into;
                playInro = false;
            }
            else
            {
                state = WoodcutterState.Walk;
            }
        }
    }
    void AxeAttack()
    {
        canAttack = false;
        animator.SetTrigger("Attack");
        velocity.x = 0;
        state = WoodcutterState.AxeAttack;
    }
    void LogKick()
    {
        canLogKick = false;
        animator.SetTrigger("LogKick");
        velocity.x = 0;
        state = WoodcutterState.LogKick;
    }
    void SawThrow(int sawNum = 1)
    {
        canSawThrow = false;
        animator.SetTrigger("SawThrow");
        velocity.x = 0;
        sawNumber = sawNum;
        state = WoodcutterState.SawToss;
    }
    void Jump(int direction)
    {
        canJump = false;
        jumpStartAnimation = true;
        jumpDirection = direction;
        jumpTimer = jumpDuration;
        state = WoodcutterState.Jump;
        animator.ResetTrigger("EndJump");
        if (controller.collisions.faceDirection == direction)
        {
            animator.SetTrigger("JumpForward");
        }
        else
        {
            animator.SetTrigger("JumpBack");
        }
    }
    public void EndJumpStart()
    {
        jumpStartAnimation = false;
    }
    IEnumerator ActiveDelay()
    {
        yield return new WaitForSecondsRealtime(.2f);
        state = WoodcutterState.Walk;
    }
    public void FinishIntro()
    {
        state = WoodcutterState.Walk;
    }
    public override void FaceLeft()
    {
        base.FaceLeft();
        controller.collisions.faceDirection *= -1;
        state = WoodcutterState.Walk;
    }
    public override void FaceRight()
    {
        base.FaceRight();
        controller.collisions.faceDirection *= -1;
        state = WoodcutterState.Walk;
    }
    public void StartingFaceDirection()
    {
        Vector3 scale = transform.localScale;
        scale.x = -1;
        transform.localScale = scale;
        controller.collisions.faceDirection = 1;
    }
    void ResetAnimation()
    {
        animator.ResetTrigger("SawThrow");
        animator.ResetTrigger("LogKick");
        animator.ResetTrigger("Attack");
        animator.ResetTrigger("JumpBack");
        animator.ResetTrigger("JumpForward");
        animator.ResetTrigger("EndJump");
    }
    public void GroundCheck()
    {
        controller.GroundCheck();
        if (controller.collisions.below)
        {
            animator.SetTrigger("EndJump");
        }
    }
    void SetPhaseTwo()
    {
        inPhaseTwo = true;
        secondProjectile = true;
    }

}
