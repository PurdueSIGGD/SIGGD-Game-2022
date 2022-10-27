public class Slow : Debuff
{
    public static float _debuffPercent;
    public static float DebuffPercent
    {
        get { return _debuffPercent; }
        set
        {
            _debuffPercent = value;
            if (_debuffPercent <= 0f)
            {
                _debuffPercent = 0f;
            }
        }
    }

    private float percentageSlow;

    public Slow(float duration, float percentageSlow) : base(duration)
    {
        this.percentageSlow = percentageSlow;
    }

    public override void ApplyDebuff()
    {
        DebuffPercent -= percentageSlow;
    }
}
