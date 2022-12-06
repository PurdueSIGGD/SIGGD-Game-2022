using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffsManager : MonoBehaviour
{
    private List<StatusEffect> buffs;

    void Start()
    {
        buffs = new List<StatusEffect>();
    }

    public void AddBuff(StatusEffect buff)
    {
        buffs.Add(buff);
        Debug.Log($"Buff applied to Player");
    }

    private void ResetBuffs()
    {
        SpeedBoost.Reset();
    }

    public void UpdateBuffs()
    {
        for (int i = 0; i < buffs.Count; i++)
        {
            StatusEffect buff = buffs[i];
            buff.UpdateEffect(Time.deltaTime);
            if (buff.HasEnded())
            {
                buffs.RemoveAt(i);
                i--;
            }
        }

        ResetBuffs();
        foreach (StatusEffect buff in buffs)
        {
            buff.ApplyEffect();
        }
    }
}
