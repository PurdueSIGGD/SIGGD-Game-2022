using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebuffsManager : MonoBehaviour
{
    private List<StatusEffect> debuffs;

    void Start()
    {
        debuffs = new List<StatusEffect>();
    }

    public void AddDebuff(StatusEffect debuff)
    {
        if (!Immune.isImmune())
        {
            debuffs.Add(debuff);
            Debug.Log($"Debuff applied to Player");
        }
    }

    private void ResetDebuffs() {
        Slow.Reset();
        Ensnared.Reset();
    }

    public void UpdateDebuffs()
    {
        for (int i = 0; i < debuffs.Count; i++)
        {
            StatusEffect debuff = debuffs[i];
            debuff.UpdateEffect(Time.deltaTime);
            if (debuff.HasEnded())
            {
                debuffs.RemoveAt(i);
                i--;
            }
        }

        ResetDebuffs();
        foreach (StatusEffect debuff in debuffs)
        {
            debuff.ApplyEffect();
        }

    }

    public void Cleanse()
    {
        foreach (StatusEffect debuff in debuffs)
        {
            //debuff.setTime(debuff.getDuration());
            debuff.End();
        }
    }

    /*public Vector2 ApplySlow(Vector2 velocity)
    {
        return Slow.DebuffPercent * velocity;
    }*/
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


