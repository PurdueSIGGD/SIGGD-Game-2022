public abstract class Debuff
{
    private float duration;
    private float time;

    public Debuff(float duration)
    {
        this.duration = duration;
    }

    public void UpdateDebuff(float dt)
    {
        time += dt;
    }

    public bool HasEnded()
    {
        return time >= duration;
    }

    public abstract void ApplyDebuff();
}
