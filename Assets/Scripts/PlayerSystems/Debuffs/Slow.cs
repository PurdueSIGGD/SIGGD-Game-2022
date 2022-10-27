using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slow : Debuff
{


    private float duration;
    private int percentageSlow;

    public Slow(float duation, int percentageSlow) : base (duration) {
       this.percentageSlow = percentageSlow;

    }
     
    public void StartDebuff() {
        player.Movement.setMaxSpeed(player.Movement.getMaxSpeed() * percentageSlow / 100);
    } 

    public void EndDebuff() {
        player.Movement.setMaxSpeed(player.Movement.getMaxSpeed() * 100 / percentageSlow);
    }
}