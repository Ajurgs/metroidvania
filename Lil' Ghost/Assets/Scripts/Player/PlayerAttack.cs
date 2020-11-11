using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{

    public GameObject hitboxHorizontal;
    public GameObject hitboxUp;
    public GameObject hitboxDown;

    public PlayerInfo player;
    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        player = GetComponent<PlayerInfo>();
    }

    // Update is called once per frame
    void Update()
    {
        // attack input
        if (Input.GetButtonDown("Attack") && !player.isAttacking)
        {
            player.isAttacking = true;
            player.lastFaceDirection = player.controller.collisions.faceDirection;

            player.canTurn = false;
            player.controller.JumpCheck(player.velocity);
            if (player.directionalInput.y < 0 && player.groundedCheck <= 0)
            {
                animator.SetTrigger("AttackD");
            }
            else if (player.directionalInput.y == 1)
            {
                animator.SetTrigger("AttackU");

            }
            else
            {
                animator.SetTrigger("AttackH");

            }

        }
    }

    public void DeactivateAttacks()
    {
        hitboxHorizontal.SetActive(false);
        hitboxUp.SetActive(false);
        hitboxDown.SetActive(false);
    }
    public void AttackHorizontal()
    {
        hitboxHorizontal.SetActive(true);
    }
    public void AttackUp()
    {
        hitboxUp.SetActive(true);
    }
    public void AttackDown()
    {
        hitboxDown.SetActive(true);
    }
    public void ResetAttack()
    {
        player.canTurn = true;
        player.isAttacking = false;
        if (player.lastFaceDirection != player.controller.collisions.faceDirection)
        {
            player.controller.collisions.faceDirection = player.lastFaceDirection;
        }

    }
}
