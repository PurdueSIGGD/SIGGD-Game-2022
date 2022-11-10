using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SEManager : MonoBehaviour
{
    private List<StatusEffect> debuffs;

    void Start()
    {
        debuffs = new List<StatusEffect>();
    }

    public void AddBuff(StatusEffect debuff)
    {
        debuffs.Add(debuff);
        debuff.IsBuff = true;
    }

    public void AddDebuff(StatusEffect debuff)
    {
        debuffs.Add(debuff);
        debuff.IsBuff = false;
    }

    private void ResetStatusEffects() {
        MovementSE.Reset();
    }

    public void UpdateStatusEffects()
    {
        for (int i = 0; i < debuffs.Count; i++)
        {
            StatusEffect debuff = debuffs[i];
            debuff.UpdateStatusEffect(Time.deltaTime);
            if (debuff.HasEnded())
            {
                debuffs.RemoveAt(i);
                i--;
            }
        }

        ResetStatusEffects();
        foreach (StatusEffect debuff in debuffs)
        {
            debuff.ApplyStatusEffect();
        }
    }

    public static Vector2 ApplyMovement(Vector2 velocity)
    {
        return MovementSE.SEPercent * velocity;
    }
}


// Old Code
// This code makes the player knocked down, at which point they will need to manually get back up.
// public void knockedDown() {
//     bool knockedDown = true;
//     // input one = /*(keyboard input)*/;
//     // input two = /*(different keyboard input)*/;
//     // input three = /*(final keyboard input)*/;
//     bool firstkey = false;
//     bool secondkey = false;
//     while(knockedDown) {
//         // stop all actions. My (Neel) idea is to make the player input 3 random inputs in quick sucession on their keyboard in order to get back up.
//         if(one) {
//             firstkey = true;
//             if(firstkey) {
//                 if(two) {
//                     secondkey = true;
//                 } else {
//                     firstkey = false;
//                 }
//                 if(firstkey && secondkey) {
//                     if(three) {
//                         knockedDown = false;
//                     } else {
//                         firstkey = false;
//                         secondkey = false;
//                     }
//                 }
//             }
//         }
//     }
// }


