using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slow : Debuff
{
    private float percentage;

    public Slow(float duration, float percentage) : base(duration) {
        this.percentage = percentage;
    }

    public override void StartDebuff() {
        
    }

    public override void EndDebuff() {

    }
}