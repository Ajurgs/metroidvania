using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum TrapType { Boss, Monster, Puzzel}
public class TrapTrigger : MonoBehaviour
{
    [SerializeField]
    private TrapType trapType;

    private BoxCollider2D boxCollider;
    public GameObject effects;
    public bool triped = false;
    public bool disabled = false;
    void OnEnable()
    {
        GameControl.ResetOnRest += Reset;

    }
    void OnDisable()
    {
        GameControl.ResetOnRest -= Reset;
    }
    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!disabled && !triped)
        {
            triped = true;
            effects.SendMessage("CheckLockState");
            
        }
        
    }
    void Reset()
    {
        if (!disabled)
        {
            triped = false;
            effects.SendMessage("CheckLockState");
        }
    }
}
