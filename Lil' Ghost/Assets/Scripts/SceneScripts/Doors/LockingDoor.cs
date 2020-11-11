using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LockType {  Boss, Switch, Key, Trap, MultiSwitch,Rest }
public class LockingDoor : MonoBehaviour
{
    [SerializeField]
    private LockType lockType;
    [SerializeField]
    private GameObject lockSwitchOne;
    [SerializeField]
    private GameObject lockSwitchTwo;
    [SerializeField]
    private GameObject lockSwitchThree;
    [SerializeField]
    private HitInteractiveObj doorOverride;

    private bool hasBeenOverriden = false;
    public bool unlocked = false;
    public BoxCollider2D boxCollider;

    public Material lockedMaterial;
    public Material unlockedMaterial;
    // Start is called before the first frame update
    void Awake()
    {
        if (doorOverride != null)
        {
            doorOverride.IsOverride = true;
        }
       
    }
    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        // if no switch is declared then door is unlocked
        
        
        if(lockType == LockType.Switch || lockType == LockType.MultiSwitch)
        {
            DoorSwitchType();
        }
        else if( lockType == LockType.Trap)
        {

            Unlock();
        }
        else if( lockType == LockType.Rest) 
        {
            Lock();
            GameControl.ResetOnRest += RestUnlock;
        }

        if (unlocked)
        {
            GetComponent<MeshRenderer>().material = unlockedMaterial;
        }
        else
        {
            GetComponent<MeshRenderer>().material = lockedMaterial;
        }
    }

    // Update is called once per frame
    void Update()
    {    
    }

    void OverrideLock()
    {
        Unlock();
        hasBeenOverriden = true;
    }
    void CheckLockState()
    {
        switch (lockType)
        {
            case LockType.Switch:
                {
                    if (lockSwitchOne.GetComponent<HitInteractiveObj>().active)
                    {
                        Unlock();
                    }
                    else
                    {
                        Lock();
                    }
                    break;
                }
            case LockType.MultiSwitch:
                {
                    if(lockSwitchThree == null)
                    {
                        if(lockSwitchOne.GetComponent<HitInteractiveObj>().active && lockSwitchTwo.GetComponent<HitInteractiveObj>().active)
                        {
                            Unlock();
                        }
                        else
                        {
                            Lock();
                        }
                    }
                    else
                    {
                        if (lockSwitchOne.GetComponent<HitInteractiveObj>().active && lockSwitchTwo.GetComponent<HitInteractiveObj>().active && lockSwitchThree.GetComponent<HitInteractiveObj>().active)
                        {
                            Unlock();
                        }
                        else
                        {
                            Lock();
                        }
                    }
                    break;
                }
            case LockType.Trap:
                {
                    if (lockSwitchOne.GetComponent<TrapTrigger>().triped)
                    {
                        Lock();
                    }
                    else if (lockSwitchOne.GetComponent<TrapTrigger>().triped == false)
                    {
                        Unlock();
                    }
                    break;
                }  
        }
    }

    void Unlock()
    {

        // run ulock animation
        // then unlock
        unlocked = true; // make in to a function to run after animation play
        GetComponent<MeshRenderer>().material = unlockedMaterial;
        boxCollider.enabled = false;
    }


    void Lock()
    {
        if (!hasBeenOverriden)
        {
            unlocked = false;
            GetComponent<MeshRenderer>().material = lockedMaterial;
            boxCollider.enabled = true;
        }
        
    }

    /// Determine what type of switch lock to use single or multi
    /// based off of what of the three switches is declared
    void DoorSwitchType()
    {

        // if no switches declared
        if (lockSwitchOne == null && lockSwitchTwo == null && lockSwitchThree == null)
        {
            Unlock();
        }
        //-----------------------------------
        // only one switch declared
        //-----------------------------------
        // only the first
        if (lockSwitchOne != null && lockSwitchTwo == null && lockSwitchThree == null)
        {
            lockType = LockType.Switch;
        }
        // only the second
        else if (lockSwitchOne == null && lockSwitchTwo != null && lockSwitchThree == null)
        {
            lockSwitchOne = lockSwitchTwo;
            lockSwitchTwo = null;
            lockType = LockType.Switch;
        }
        // only the third
        else if (lockSwitchOne == null && lockSwitchTwo == null && lockSwitchThree != null)
        {
            lockSwitchOne = lockSwitchThree;
            lockSwitchThree = null;
            lockType = LockType.Switch;
        }
        //-----------------------------------
        // Two or Three Swithes declared
        //-----------------------------------
        // first and second declared
        else if (lockSwitchOne != null && lockSwitchTwo != null && lockSwitchThree == null)
        {
            lockType = LockType.MultiSwitch;
        }
        // first and third
        else if (lockSwitchOne != null && lockSwitchTwo == null && lockSwitchThree != null)
        {
            lockSwitchTwo = lockSwitchThree;
            lockSwitchThree = null;
            lockType = LockType.MultiSwitch;
        }
        // second and third
        else if (lockSwitchOne == null && lockSwitchTwo != null && lockSwitchThree != null)
        {
            lockSwitchOne = lockSwitchThree;
            lockSwitchThree = null;
            lockType = LockType.MultiSwitch;
        }
        // all three declared
        else if (lockSwitchOne != null && lockSwitchTwo != null && lockSwitchThree != null)
        {
            lockType = LockType.MultiSwitch;
        }
    }


    void RestUnlock()
    {
        unlocked = true; // make in to a function to run after animation play
        GetComponent<MeshRenderer>().material = unlockedMaterial;
        boxCollider.enabled = false;
    }

    
}
