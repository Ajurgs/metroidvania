using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTemplate : ScriptableObject
{
    public int maxHits;
    public int maxEnergy;
    public int attackDamage;
    public float movementSpeed;
    public float jumpVelocity;
    public float bounceAmount;
    public float knockbackAmount;
    public float knockbackDuration;
    public float aggrovationTime;
    public float abilityRange;
    public float abilityResetTime;
    public float abilityDurationTime;
    public float abilityChargeTime;
    public float sightRange;
    public bool canRespawn;
    public bool stopAtEdge;

    public virtual void Ability()
    {

    }

}
