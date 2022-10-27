public class Slow : Debuff
{
    public static float DebuffPercent { get; private set; }

    private float percentageSlow;

    public Slow(float duration, float percentageSlow) : base(duration)
    {
        this.percentageSlow = percentageSlow;
    }

    public override void ApplyDebuff()
    {
        DebuffPercent -= percentageSlow;
        if (DebuffPercent < 0)
        {
            DebuffPercent = 0;
        }
    }
}
