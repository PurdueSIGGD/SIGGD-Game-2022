using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slow : Debuff
{
    private int percentageSlow;

    public Slow(float duration, int percentageSlow) : base (duration) {
       this.percentageSlow = percentageSlow;
    }
     
    public override void StartDebuff() {
        player.Movement.SetMaxSpeed(player.Movement.GetMaxSpeed() * percentageSlow / 100f);
    } 

    public override void EndDebuff() {
        player.Movement.SetMaxSpeed(player.Movement.GetMaxSpeed() * 100 / percentageSlow);
    }
}