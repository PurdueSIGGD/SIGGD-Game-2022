using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Invincible : StatusEffect
{
    public static bool Invincibility { get; private set; }

    public Invincible(float duration) : base(duration)
    {

    }

    public static void Reset()
    {
        Invincibility = false;
    }

    public override void ApplyEffect()
    {
        Invincibility = true;
    }

    public static bool isInvincible()
    {
        return Invincibility;
    }
}
