using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RangedAttack", menuName = "ScriptableObjects/RangedAttack", order = 1)]
public class rangedAttack : enemyAttack
{
    public float spawnOffset; //used to decide how high on the enemy the projectile will spawn
    public GameObject projectile;
}
