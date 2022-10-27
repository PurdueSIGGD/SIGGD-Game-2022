using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebuffsManager : MonoBehaviour
{
    private List<Debuff> debuffs;

    void Start()
    {
        debuffs = new List<Debuff>();
    }

    public void AddDebuff(Debuff debuff)
    {
        debuffs.Add(debuff);
    }

    private void ResetDebuffs() {
        Slow.Reset();
    }

    public void UpdateDebuffs()
    {
        for (int i = 0; i < debuffs.Count; i++)
        {
            Debuff debuff = debuffs[i];
            debuff.UpdateDebuff(Time.deltaTime);
            if (debuff.HasEnded())
            {
                debuffs.RemoveAt(i);
                i--;
            }
        }

        ResetDebuffs();
        foreach (Debuff debuff in debuffs)
        {
            debuff.ApplyDebuff();
        }
    }

    public Vector2 ApplySlow(Vector2 velocity)
    {
        return Slow.DebuffPercent * velocity;
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


