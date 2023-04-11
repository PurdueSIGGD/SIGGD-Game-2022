using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyAttack : ScriptableObject
{
    public float attackWindupTime; //Time from attack animation starts until the attack can hit the player or shoots a projectile
    public float attackCooldownTime; //Time after the attack can hit the player or shoots a projectile where the enemy should be unable to attack again
    public float attackMinumumRange; //Min range to attempt this attack
    public float attackMaximumRange; //Max range to attempt this attack
    public string animTriggerName; //Name of the trigger in the animation tree which corresponds to this attack's animation
    public GameObject audioPrefab; //Stores the gameobject with an audio source on it to spawn on the attack's beginning
}
