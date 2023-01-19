using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MeleeAttack", menuName = "ScriptableObjects/MeleeAttack", order = 0)]
public class meleeAttack : enemyAttack
{
    public float maxRangeToHit; //max range the player can be from the enemy for the attack to hit the player
    public bool killOnHit; //Most melee attacks will kill on hit, but some will not as they are meant to give debuffs
    //public Debuff debuff; //Does not do anything right now, will implement later
}
