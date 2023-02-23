using System;
using UnityEngine;

public class Immune : StatusEffect
{
    //public static float VelocityScalar { get; private set; }
    public static bool Immunity {  get; private set; }

    public Immune(float duration) : base(duration)
    {
        DebuffsManager debuffsManager = GameObject.Find("Player").GetComponent<DebuffsManager>();
        if (debuffsManager != null)
        {
            debuffsManager.Cleanse();
            Debug.Log($"Cleanse was called");
        }
    }

    public static void Reset()
    {
        Immunity = false;
    }

    public override void ApplyEffect()
    {
        Immunity = true;
    }

    public static bool isImmune()
    {
        return Immunity;
    }
}