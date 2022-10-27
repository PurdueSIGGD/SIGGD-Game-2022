using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Debuff : MonoBehaviour
{
    protected Player player;
    private float duration;
    private float time;

    public Debuff(float duration) {
        this.duration = duration;
    }

    public void SetPlayer(Player player) {
        this.player = player;
    }

    public void UpdateDebuff() {
        time += Time.deltaTime;
    }

    public bool HasEnded() {
        return time >= duration;
    }

    public abstract void StartDebuff();

    public abstract void EndDebuff();

    // This code makes the player knocked down, at which point they will need to manually get back up.
    // public void knockedDown() {
    //     bool knockedDown = true;
    //     // input one = /*(keyboard input)*/;
    //     // input two = /*(different keyboard input)*/;
    //     // input three = /*(final keyboard input)*/;
    //     bool firstkey = false;
    //     bool secondkey = false;
    //     while(knockedDown) {
    //         // stop all actions. My (Neel) idea is to make the player input 3 random inputs in quick sucession on their keyboard in order to get back up.
    //         if(one) {
    //             firstkey = true;
    //             if(firstkey) {
    //                 if(two) {
    //                     secondkey = true;
    //                 } else {
    //                     firstkey = false;
    //                 }
    //                 if(firstkey && secondkey) {
    //                     if(three) {
    //                         knockedDown = false;
    //                     } else {
    //                         firstkey = false;
    //                         secondkey = false;
    //                     }
    //                 }
    //             }
    //         }
    //     }
    // }

}
