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

    private float timeClimbed; //A float to keep track of the number of seconds the user has climbed, used to calculate stamina loss

    // Start is called before the first frame update. As of 10/7/22, 100 is an arbitrary number, and it resets the timeClimbed to 0.0
    void Start()
    {
        stamina = 100;
        timeClimbed = 0.0f;
    }

    /* Update is called once per frame. As of 10/5/22, stamina won't regen when standing still. Could change in future.
       
       The climbing portion means every 6 frames of of climbing, or every 0.1 seconds, a person loses one stamina, or loss of 10 stamina per second of climbing.
       As of 10/7/22, this number if arbitrary. The sound / animation comments are only in there as reminders, they could go somewhere else. The subtraction
       of 0.1 from timeClimbed allows the time to always stay between 0.0 and 0.1, while also allowing to get to 0.1, change the stamina, then reset. If I 
       didn't reset it, then time would be equal to the number of seconds a user has been climbing, while can be a big number after some playtime, 
       which isn't what we want. Dm Bryce L. on Discord if more of an explination is needed
       */

    void Update()
    {
        //6*Time.deltaTime
        while (isClimbing) {
            timeClimbed += Time.deltaTime;
            if (timeClimbed % 0.1 == 0) {
                stamina -= 1;
                timeClimbed -= 0.1f;
                //call climbing animation here
                //call climbing sound here
            }
        }
        
    }

    /*
       Dashes, plays sound / animation, and subtracts stamina for dash, if the player has stamina for it. As of 10/7/22, 20 is an arbitrary number
    */
    public void Dash() {
        
        if (stamina >= 20) {
        //play dash animation here
        //play dash sound here
        stamina -= 20;
        }
    }

    /*
       Heavy attacks, plays sound / animation, and subtracts stamina for attack, if the player has stamina for it. As of 10/7/22, 25 is an arbitrary number
    */
    public void HeavyAttack() {
        if (stamina >= 25) {
            //play heavy attack animation here
            //play heaavy attack sound here
            stamina -= 25;
            }
    }

     /*
       Opens a door using stamina, plays the associated sound and animation. As of 10/12/22, 15 is an arbitrary number
     */
    public void OpenDoor() {
        if (stamina >= 15) {
            //play open door animation here
            //play open door sound here
            stamina -= 15;
        }
    }

    /*
      Changes stamina using a parameter. Used for items that buff / increase stamina, or decrease it for other bonuses if an item like
      that exists. Needs reasonable limits
    */
    public void AddStamina(int stamina) {
         this.stamina += stamina;
         if (this.stamina >= 100) {
            this.stamina = 100;
         }
         
    }


    /*
      This is used to change the boolean that keeps track of if the user is climbing. 
    */
    public void ChangeIsClimbing(bool isClimbing) {
        this.isClimbing = isClimbing;
    }
}
