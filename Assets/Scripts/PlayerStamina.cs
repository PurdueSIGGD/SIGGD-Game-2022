using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
   Author : Bryce Liebenauer
*/
public class PlayerStamina : MonoBehaviour
{

    private int stamina;

    // Start is called before the first frame update
    void Start()
    {
        stamina = 100;
    }

    // Update is called once per frame. As of 10/5/22, stamina won't regen when standing still. Could change in future.
    void Update()
    {
        
    }

    /*
      Gets stamina for other classes
    */
    int getStamina() {
        return stamina;
    }

    /*
       Dashes, plays sound / animation, and subtracts stamina for dash, if the player has stamina for it. If  AS OF 10/5/22, 20 IS AN ARBITRARY NUMBER
    */
    void Dash() {
        
        if (stamina >= 20) {
        //play dash animation here
        //play dash sound here
        stamina -= 20;
        }
    }

    /*
       Heavy attacks, plays sound / animation, and subtracts stamina for attack, if the player has stamina for it. If  AS OF 10/5/22, 20 IS AN ARBITRARY NUMBER
    */
    void HeavyAttack() {
        //play heavy attack animation here
        //play heaavy attack sound here
        if (stamina >= 25) {
            stamina -= 25;
            }
    }

    /*
      Changes stamina using a parameter. Mostly just for testing as of 10/5/2022
    */
    void changeStamina(int stamina) {
         if(this.stamina > stamina) {
            this.stamina -= stamina;
         }
         
    }
}
