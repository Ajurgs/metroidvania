using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingEnemyContoller : EnemyController
{


    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FloatMove(Vector3 velocity)
    {
        transform.Translate(velocity);
    }
}
