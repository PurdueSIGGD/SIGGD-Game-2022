using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debuffs : MonoBehaviour
{
 

    /*
        This stuns the player, preventing them from moving or acting. The reason that movement and actions are commented is because there is no velocity setter
        and no way to stop action as of 10/24/22. The percentage slow is divided by 100 to set the speed, and the actions boolean determines if the user can act
        or not. This is meant to combine the functions of a stun debuff and slow debuff into one changable debuff. The action - based comments could just call
        the 'silence' debuff to later be created. This method is meant to be used for the stun debuff (can't move / can't act), the slow debuff (just move 
        slower), and the freeze debuff(move slower / can't act). Also, design team had not decided if debuffs would stack multiplicitavely or addively or
        fully reset the time as of 10/19/2022, so I haven't factored that in yet.
    */
    public void AffectSpeedAndActions(int duration, int percentageSlow, bool canAct) {
        double timeStunned = 0.0;
        bool stunned = true;
        //Movement.setMaxVelocity(Movement.getMaxVelocity * percentageSlow / 100));
        if (!canAct) {
            //stop actions
        }
        while (stunned) {
            timeStunned += Time.deltaTime;
            if (timeStunned > duration) {
                //Movement.setMaxVelocity(10);
                if (!canAct) {
                    //resume actions
                }
                stunned = false;

            }
        }
    }

    /*
      The code below blinds the player for some duration. I don't know how to make the screen go full white so I will ask about that in a later session.
    */
    public void Blind(int duration) {
        double timeBlinded = 0.0;
        bool blinded = true;
        //set screen to full white
        while (blinded) {
            timeBlinded += Time.deltaTime;
            if (timeBlinded > duration) {
                blinded = false;
                //return screen to normal
            }
        }
    }


    public void Confusion(int duration) {
        double timeConfused = 0.0;
        bool confused = true;
        //reverse direction controls
        while (confused) {
            timeConfused += Time.deltaTime;
            if (timeConfused > duration) {
                confused = false;
                //reverse direction controls
            }
        }

    }
}
