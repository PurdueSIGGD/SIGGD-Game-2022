using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StimBoost : Item
{
    [SerializeField] private float speedBoostDuration = 10f;
    [SerializeField] private float speedBoostPercentage = 35;

    public override void Use()
    {
        BuffsManager buffsManager = GameObject.Find("Player").GetComponent<BuffsManager>();
        if ( buffsManager != null)
        {
            buffsManager.AddBuff(new SpeedBoost(speedBoostDuration, (speedBoostPercentage * 0.01f)));
            Debug.Log($"Stim Boost buff was called");
        }
        base.Use();
    }
}
