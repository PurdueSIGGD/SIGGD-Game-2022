using UnityEngine;

public class Ensnared : StatusEffect
{
    public static float VelocityScalar { get; private set; }

    public Ensnared(float duration) : base(duration)
    {
    }

    public static void Reset()
    {
        VelocityScalar = 1f;
    }

    public override void ApplyEffect()
    {
        VelocityScalar = 0f;
    }

    public static Vector2 CalculateVelocity(Vector2 velocity)
    {
        return VelocityScalar * velocity;
    }
}