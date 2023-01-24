using UnityEngine;

public class Slow : StatusEffect
{
    public static float VelocityScalar { get; private set; }

    private float percentageSlow;

    public Slow(float duration, float percentageSlow) : base(duration)
    {
        this.percentageSlow = percentageSlow;
    }

    public static void Reset()
    {
        VelocityScalar = 1f;
    }

    public override void ApplyEffect()
    {
        VelocityScalar -= percentageSlow;
        if (VelocityScalar < 0.1f)
        {
            VelocityScalar = 0.1f;
        }
    }

    public static Vector2 CalculateVelocity(Vector2 velocity)
    {
        return VelocityScalar * velocity;
    }
}
