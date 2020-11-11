using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ConnectedTo {  Door, Elevator, Event};

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class HitInteractiveObj : MonoBehaviour
{
    [SerializeField]
    private ConnectedTo connected;
    [SerializeField]
    private bool hasTimer = false;
    [SerializeField]
    private float holdAmount = 2;
    private float holdTimer;

    private bool isOverride = false;

    public bool active = false;
    public GameObject effects;

    Animator animator;

    public bool IsOverride { get => isOverride; set => isOverride = value; }

    
    void Start()
    {
        animator = GetComponent<Animator>();
        if (IsOverride)
        {
            hasTimer = false;
        }
    }

    void Update()
    {
        if( hasTimer && holdTimer > 0 && active)
        {
            holdTimer -= Time.deltaTime;
        }
        else if(hasTimer && holdTimer <= 0 && active)
        {
            active = false;
            effects.SendMessage("CheckLockState",null,SendMessageOptions.DontRequireReceiver);
            animator.SetBool("Active", false);
        }
    }
    public void Hit()
    {
        if(active == false)
        {
            active = true;
            GameControl.control.HitStop();
            switch (connected)
            {
                case ConnectedTo.Door:
                    {
                        if (isOverride)
                        {
                            effects.SendMessage("OverrideLock");
                        }
                        else
                        {
                            effects.SendMessage("CheckLockState");
                        }
                        
                        break;
                    }
                case ConnectedTo.Elevator:
                    {
                        effects.SendMessage("CallElevator");
                        break;
                    }
            }
            animator.SetBool("Active", true);
            if (hasTimer)
            {
                Debug.Log("swtich timer set");
                holdTimer = holdAmount;
            }
        }
 
    }

 


}
