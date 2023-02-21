using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StimBoost : Item
{

    [SerializeField] public float speedBoostDuration = 10f;
    [SerializeField] public float speedBoostPercentage = 40f;


    public override void Use()
    {
        BuffsManager buffsManager = GameObject.Find("Player").GetComponent<BuffsManager>();
        if (buffsManager != null)
        {
            buffsManager.AddBuff(new SpeedBoost(speedBoostDuration, speedBoostPercentage * 0.01f));
            Debug.Log($"StimBoost was called");
        }
    }

}
