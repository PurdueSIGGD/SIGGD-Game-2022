using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaminaCapacityBoost : Item
{
    [SerializeField] private float temporaryCapacityAmount;

    public override void Use() 
    {
        // Increase Stamina Capacity to the temporary cap
         
        base.Use();
    }
}