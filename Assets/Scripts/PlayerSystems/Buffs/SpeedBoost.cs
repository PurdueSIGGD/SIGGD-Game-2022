using UnityEngine;

public class SpeedBoost : StatusEffect
{
    public static float VelocityScalar { get; private set; }

    private float percentageSpeedBoost;

    public SpeedBoost(float duration, float percentageSpeedBoost) : base(duration)
    {
        this.percentageSpeedBoost = percentageSpeedBoost;
    }

    public static void Reset()
    {
        VelocityScalar = 1f;
    }

    public override void ApplyEffect()
    {
        VelocityScalar += percentageSpeedBoost;
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
