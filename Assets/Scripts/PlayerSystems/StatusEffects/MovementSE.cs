public class MovementSE : StatusEffect
{
    public static float SEPercent { get; private set; }

    private float percentageChange;

    public MovementSE(float duration, float percentageChange) : base(duration)
    {
        this.percentageChange = percentageChange;
    }

    public static void Reset()
    {
        SEPercent = 1f;
    }

    public override void ApplyStatusEffect()
    {
        if (IsBuff)
        {
            SEPercent += percentageChange;
        }
        else
        {
            SEPercent -= percentageChange;
            if (SEPercent < 0)
            {
                SEPercent = 0;
            }
        }
    }
}
