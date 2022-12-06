using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPack : Item
{
    [SerializeField] private float healAmount = 100f;
    [SerializeField] private float impactSlowDuration = 0.8f;
    [SerializeField] private float impactSlowPercentage = 90f;
    [SerializeField] private float lingeringSlowDuration = 4f;
    [SerializeField] private float lingeringSlowPercentage = 50f;

    public override void Use()
    {
        // heals player
        //Player.instance.Heal(healAmount);
        //GameObject.Find("Player").get
        DebuffsManager debuffsManager = GameObject.Find("Player").GetComponent<DebuffsManager>();
        if (debuffsManager != null)
        {
            debuffsManager.AddDebuff(new Slow(impactSlowDuration, ((impactSlowPercentage - lingeringSlowPercentage) * 0.01f)));
            debuffsManager.AddDebuff(new Slow(lingeringSlowDuration, (lingeringSlowPercentage * 0.01f)));
            Debug.Log($"Healthpack slow was called");
        }
        base.Use();
    }
}