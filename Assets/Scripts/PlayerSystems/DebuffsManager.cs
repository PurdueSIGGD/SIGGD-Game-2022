using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebuffsManager : MonoBehaviour
{
    private Player player;

    private List<Debuff> debuffs;

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<Player>();
    }

    void AddDebuff(Debuff debuff)
    {
        debuff.SetPlayer(player);
        debuff.StartDebuff();
        debuffs.Add(debuff);
    }

    // we can have a temporary variable what we values we want to change
    // at the start of a frame, we save the original values of the variables we are about to change,
    // apply debuffs to temporary variables
    // then set the temporary variable to the actual values
    // then before the start of the next frame, get the original values and reapply the debuffs
    public void UpdateDebuffs()
    {
        for (int i = 0; i < debuffs.Count; i++) {
            Debuff debuff = debuffs[i];
            if (debuff.HasEnded()) {
                debuff.EndDebuff();
                debuffs.RemoveAt(i);
                i--;
            }
        }
        foreach (Debuff debuff in debuffs) {
            debuff.UpdateDebuff();
        }
    }
}
