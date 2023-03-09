using UnityEngine;

public class Stunned : StatusEffect
{
    public static float VelocityScalar { get; private set; }
    public static bool isStunned = false;

    public Stunned(float duration) : base(duration)
    {
    }

    public static void Reset()
    {
        VelocityScalar = 1f;
        isStunned = false;
    }

    public override void ApplyEffect()
    {
        VelocityScalar = 0f;
        isStunned = true;
    }

    public static Vector2 CalculateVelocity(Vector2 velocity)
    {
        return VelocityScalar * velocity;
    }
}