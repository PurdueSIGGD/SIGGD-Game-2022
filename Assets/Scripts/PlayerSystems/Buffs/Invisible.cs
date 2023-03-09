using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Invisible : StatusEffect
{
    public static bool Invisibility { get; private set; }

    public Invisible(float duration) : base(duration)
    {

    }

    public static void Reset()
    {
        Invisibility = false;
    }

    public override void ApplyEffect()
    {
        Invisibility = true;
    }

    public static bool isInvisible()
    {
        return Invisibility;
    }
}
