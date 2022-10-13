using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
   Author : Bryce Liebenauer
*/
public class Stamina : MonoBehaviour
{
    [SerializeField] private int maxStamina = 100; // The maximum amount of stamina the player can have
    [SerializeField] private int staminaRegen = 10; // The amount of stamina the player regains per second

    private int stamina; // An integer to track stamina
    private bool isClimbing; // A boolean to see if the player is climbing
    private float climbTime;

    // Start is called before the first frame update. As of 10/7/22, 100 is an arbitrary number
    void Start()
    {
        stamina = 100;
    }

    public void UpdateStamina()
    {
        climbTime += Time.deltaTime;
        if (isClimbing)
        {
            float lossTime = 0.1f;
            // reset climb time if it's over stamina loss time
            if (climbTime >= lossTime)
            {
                climbTime = climbTime - lossTime;
                stamina -= 1;
            }
        }
    }

    /*
       Dashes, plays sound / animation, and subtracts stamina for dash, if the player has stamina for it. If  As of 10/7/22, 20 is an arbitrary number
    */
    public void Dash()
    {
        if (stamina >= 20)
        {
            //play dash animation here
            //play dash sound here
            stamina -= 20;
        }
    }

    /*
       Heavy attacks, plays sound / animation, and subtracts stamina for attack, if the player has stamina for it. As of 10/7/22, 25 is an arbitrary number
    */
    public void HeavyAttack()
    {
        if (stamina >= 25)
        {
            //play heavy attack animation here
            //play heaavy attack sound here
            stamina -= 25;
        }
    }

    /*
      Changes stamina using a parameter. Used for items that buff / increase stamina, or decrease it for other bonuses if an item like
      that exists. Needs reasonable limits
    */
    public void AddStamina(int stamina)
    {
        this.stamina += stamina;
        if (this.stamina >= 100)
        {
            this.stamina = 100;
        }

    }

    /*
      Opens a door using stamina if the player has enough, play the sound and animation. As of 10/12/22, 15 is an arbitrary number
    */
    public void OpenDoor() {
       if (stamina >= 15) {
        //play animation here
        //play sound here
        stamina -= 15;
       }
    }

    /*
      This is used to change the boolean that keeps track of if the user is climbing. Every time the user starts to climb or stops climbing, the
      number of frames climbed resets to 0. This ensures that stamina is subtracted properly. For example, if the user starts climbing while the
      remainder when framesClimbed is divided by 6 is 5(or framesClimbed % 6 = 5), they will instantly lose stamina on the next frame of climbing, which
      isn't what we want if we plan on having the player lose stamina every 6 frames of climbing. This setting it to 0 every time the isClimbing boolean
      is changed fixes that error. For an more in depth explination, dm Bryce L. on discord

      changed to reset time
    */
    public void SetIsClimbing(bool isClimbing)
    {
        this.isClimbing = isClimbing;
        climbTime = 0;
    }
}
