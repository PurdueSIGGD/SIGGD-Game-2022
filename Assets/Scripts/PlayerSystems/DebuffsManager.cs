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

    // Update is called once per frame
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
