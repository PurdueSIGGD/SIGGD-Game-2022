public abstract class StatusEffect
{
    public bool IsBuff;

    private float duration;
    private float time;

    public StatusEffect(float duration)
    {
        this.duration = duration;
    }

    public void UpdateStatusEffect(float dt)
    {
        time += dt;
    }

    public bool HasEnded()
    {
        return time >= duration;
    }

    public abstract void ApplyStatusEffect();
}
