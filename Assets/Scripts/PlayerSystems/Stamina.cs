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

    public bool DecreaseStamina(int amount) {
        if (stamina >= amount) {
            stamina -= amount;
            return true;
            
        }
        return false;
    }

    public int GetStamina() {
        return stamina;
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