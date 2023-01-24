using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContactResistor : Item
{

    [SerializeField] public float immuneDuration = 1.5f;
    [SerializeField] public float speedBoostDuration = 1.5f;
    [SerializeField] public float speedBoostPercentage = 150.0f;

    public override void Use()
    {
        BuffsManager buffsManager = GameObject.Find("Player").GetComponent<BuffsManager>();
        if (buffsManager != null)
        {
            buffsManager.AddBuff(new Immune(immuneDuration));
            buffsManager.AddBuff(new SpeedBoost(speedBoostDuration, speedBoostPercentage * 0.01f));
            Debug.Log($"ContactResistor was called");
        }
    }

}
