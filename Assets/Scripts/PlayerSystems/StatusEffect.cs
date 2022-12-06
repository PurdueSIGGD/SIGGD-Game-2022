public abstract class StatusEffect
{
    private float duration;
    private float time;

    public StatusEffect(float duration)
    {
        this.duration = duration;
    }

    public void UpdateEffect(float dt)
    {
        time += dt;
    }

    public bool HasEnded()
    {
        return time >= duration;
    }

    public abstract void ApplyEffect();
}
