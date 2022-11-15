using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public float period;
    public float time {get {return _time;} set {_time = value; OnValueChanged.Invoke(value);}}
    public float normalizedTime => this.time / this.period;
    private float _time;
    public bool loop = false;
    public UnityEngine.Events.UnityEvent OnPeriodDone;
    public UnityEngine.Events.UnityEvent<float> OnValueChanged;

    [SerializeField] private bool runOnAwake;

    public void rewind(float duration) {
        this.time = Mathf.Max(this.time - duration, 0.0f);
    }

    public bool IsFinished() {
        return this.time == this.period;
    }

    public float TimeLeft() {
        return this.period - this.time;
    }

    public void StartTimer() {
        this.time = 0f;
    }

    // does not call the event
    public void FinishTimer() {
        this.time = period;
        OnPeriodDone?.Invoke();
    }

    void Awake() {
        this.OnPeriodDone ??= new UnityEngine.Events.UnityEvent();
        this.OnValueChanged ??= new UnityEngine.Events.UnityEvent<float>();

        if (runOnAwake) {
            StartTimer();
        } else {
            this.time = period;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {   
        var initTime = time;

        // Timer does nothing if it's done and it doesn't loop
        if (time >= period && loop == false) {
            return;
        }

        var dt = Time.deltaTime;

        if (time + dt >= period) {
            if (loop) {
                time = (time + dt) % period;
            } else {
                time = period;
            }
            // this event can possibly change the time
            OnPeriodDone?.Invoke();
        } else {
            time = time + dt;
        }
    }
}
