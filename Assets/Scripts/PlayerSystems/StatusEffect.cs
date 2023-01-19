public abstract class StatusEffect
{
    private float duration;
    private float time;

    public StatusEffect(float duration)
    {
        this.duration = duration;
    }

    public float getDuration()
    {
        return duration;
    }

    public void setDuration(float duration)
    {
        this.duration = duration;
    }

    public float getTime()
    {
        return time;
    }

    public void setTime(float time)
    {
        this.time = time;
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
