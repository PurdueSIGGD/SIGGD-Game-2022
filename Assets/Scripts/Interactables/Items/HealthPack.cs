using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPack : Item
{
    [SerializeField] private float healAmount;

    public override void Use()
    {
        // heals player
        //Player.instance.Heal(healAmount);
        base.Use();
    }
}