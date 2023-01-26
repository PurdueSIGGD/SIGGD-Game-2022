using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Unity.VisualScripting;

[System.Serializable]
public class attackTuple
{
    public enemyAttack attack;
    public float weight; // if multiple attacks can be used at once, this skews the randomness towards one or another
}

public class attackManager : MonoBehaviour
{
    [SerializeField]
    private attackTuple[] attackTuples;
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    //finds valid attacks, then chooses one to use
    //returns the cooldown time of the attack
    public float tryAttack(float distance)
    {
        //get valid attacks
        List<attackTuple> validAttacks = new List<attackTuple>();
        float totWeight = 0;
        foreach (var attackTuple in attackTuples)
        {
            if (distance >= attackTuple.attack.attackMinumumRange && distance <= attackTuple.attack.attackMaximumRange)
            {
                validAttacks.Add(attackTuple);
                totWeight += attackTuple.weight;
            }
        }

        //if no valid attacks return zero
        if (validAttacks.Count <= 0 || totWeight <= 0)
        {
            return 0;
        }

        //choose random value
        float result = Random.Range(0, totWeight);

        //choose the correct attack
        float sum = 0;
        foreach (var attackTuple in validAttacks)
        {
            if (sum + attackTuple.weight > result)
            {
                //this is in range
                //do attack
                IEnumerator coroutine = doAttack(attackTuple.attack);
                StartCoroutine(coroutine);
                return attackTuple.attack.attackWindupTime + attackTuple.attack.attackCooldownTime;
            }
            sum += attackTuple.weight;
        }

        //this should never occur
        return 0;
    }

    private IEnumerator doAttack(enemyAttack attack)
    {
        Debug.Log(transform.name + " has started an attack windup");
        
        //windup
        animator.SetTrigger(attack.animTriggerName);
        yield return new WaitForSeconds(attack.attackWindupTime);

        Transform playerTrans = (Transform)Variables.ActiveScene.Get("player");
        Vector3 toPlayerVector = playerTrans.position - transform.position;

        //deal with attack type
        if (attack is meleeAttack)
        {
            Debug.Log(transform.name + " does a melee attack");

            //if close enough, kill or apply debuff
            float maxRange = ((meleeAttack)attack).maxRangeToHit;
            if (Vector3.SqrMagnitude(toPlayerVector) <= maxRange * maxRange)
            {
                Debug.Log(transform.name + "'s melee attack hit");
                if (((meleeAttack)attack).killOnHit)
                {
                    playerTrans.GetComponent<Player>().kill();
                } else if (false) //check for debuff existing here
                {
                    //apply debuff to the player
                }
            } else
            {
                Debug.Log(transform.name + "'s melee attack missed");
            }
        } else if (attack is rangedAttack)
        {
            Debug.Log(transform.name + " does a ranged attack");
            //spawn projectile
            GameObject g = Instantiate(((rangedAttack)attack).projectile, transform.position + Vector3.up * ((rangedAttack)attack).spawnOffset, Quaternion.LookRotation(toPlayerVector - Vector3.up * ((rangedAttack)attack).spawnOffset, Vector3.up));
        }
        
    }
}
