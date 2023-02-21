using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectTest : Item
{
    [SerializeField] private float speedBoostDuration = 10f;
    [SerializeField] private float speedBoostPercentage = 35;

    public override void Use()
    {
        //DebuffsManager debuffsManager = GameObject.Find("Player").GetComponent<DebuffsManager>();
        BuffsManager buffsManager = GameObject.Find("Player").GetComponent<BuffsManager>();
        if (buffsManager != null )
        {
            //debuffsManager.Cleanse();
            //Debug.Log($"Cleanse Called");
            buffsManager.AddBuff(new Immune(5f));
            Debug.Log($"Immune Buff was called");
        }
        /*BuffsManager buffsManager = GameObject.Find("Player").GetComponent<BuffsManager>();
        if ( buffsManager != null)
        {
            buffsManager.AddBuff(new SpeedBoost(speedBoostDuration, (speedBoostPercentage * 0.01f)));
            Debug.Log($"Stim Boost buff was called");
        }
        base.Use(); */
    }
}
