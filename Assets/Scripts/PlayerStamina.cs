using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
   Author : Bryce Liebenauer
*/
public class PlayerStamina : MonoBehaviour
{

    private int stamina; //An integer to track stamina

    private bool isClimbing; //A boolean to see if the player is climbing

    private int framesClimbed; //An integer to keep track of the number of frames the user has climbed, used to calculate stamina loss

    // Start is called before the first frame update. As of 10/7/22, 100 is an arbitrary number
    void Start()
    {
        stamina = 100;
    }

    /* Update is called once per frame. As of 10/5/22, stamina won't regen when standing still. Could change in future.
       
       The climbing portion means every 6 frames of climbing, a person loses one stamina, or loss of 10 stamina per second of climbing.
       As of 10/7/22, this number if arbitrary. The sound / animation comments are only in there as reminders, they could go somewhere else
    */

    void Update()
    {
        while(isClimbing) {
           framesClimbed++;
           if(framesClimbed % 6 == 0) {
            stamina -= 1;
            //play climbing sound here
            //play climbing animation here
           }
        }
    }

    /*
       Dashes, plays sound / animation, and subtracts stamina for dash, if the player has stamina for it. If  As of 10/7/22, 20 is an arbitrary number
    */
    void Dash() {
        
        if (stamina >= 20) {
        //play dash animation here
        //play dash sound here
        stamina -= 20;
        }
    }

    /*
       Heavy attacks, plays sound / animation, and subtracts stamina for attack, if the player has stamina for it. As of 10/7/22, 25 is an arbitrary number
    */
    void HeavyAttack() {
        if (stamina >= 25) {
            //play heavy attack animation here
            //play heaavy attack sound here
            stamina -= 25;
            }
    }

    /*
      Changes stamina using a parameter. Used for items that buff / increase stamina, or decrease it for other bonuses if an item like
      that exists. Needs reasonable limits
    */
    void ChangeStamina(int stamina) {
         if(this.stamina > stamina) {
            this.stamina += stamina;
         }
         
    }

    /*
      This is used to change the boolean that keeps track of if the user is climbing. Every time the user starts to climb or stops climbing, the
      number of frames climbed resets to 0. This ensures that stamina is subtracted properly. For example, if the user starts climbing while the
      remainder when framesClimbed is divided by 6 is 5(or framesClimbed % 6 = 5), they will instantly lose stamina on the next frame of climbing, which
      isn't what we want if we plan on having the player lose stamina every 6 frames of climbing. This setting it to 0 every time the isClimbing boolean
      is changed fixes that error. For an more in depth explination, dm Bryce L. on discord
    */
    void ChangeIsClimbing(bool isClimbing) {
        this.isClimbing = isClimbing;
        framesClimbed = 0;
    }
}
