using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Attack", menuName = "ScriptableObjects/EnemyAttack", order = 1)]
public class enemyAttack : ScriptableObject
{
    public float attackWindupTime;
    public float attackCooldownTime;
    public float attackMinumumRange;
    public float attackMaximumRange;
    public Debuff debuff; //This is not appearing for some reason
}
