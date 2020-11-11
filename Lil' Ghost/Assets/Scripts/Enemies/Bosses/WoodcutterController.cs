using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(WoodcutterInfo))]
public class WoodcutterController : RaycastController
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
    public void Move(Vector3 velocity, bool standingOnPlatform = false)
    {
        UpdateRaycastOrigins();
        collisions.Reset();
        collisions.velocityOld = velocity;


        if (velocity.y < 0)
        {
            DescendSlope(ref velocity);
        }

        HorizontalCollisions(ref velocity);
        VerticalCollisions(ref velocity);

        transform.Translate(velocity);
        if (standingOnPlatform)
        {
            collisions.below = true;
        }
    }



}
