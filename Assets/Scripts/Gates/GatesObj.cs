using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** Creates gate objects (interactable, non-item objects) such as doors, chests, or levers  
 */

public class GatesObj : MonoBehaviour
{
    public bool keyNeeded = false;
    public bool passwordGiven = true;
    public string password;

    // If player tries to open the gate
    // returns 1 if the password entered is incorrect
    // returns 2 if the password is correct
    // returns 3 if the key is in the player's inventory
    // returns 4 if the key is not in the player's inventory
    public int readObj(string userPass)
    {
        bool isKeyInInventory = FindObjectOfType<InventorySystem>().getKey();
        if (!keyNeeded)
        {
            if (!password.Equals(userPass)) return 1;
            return 2;
        }
        else if (isKeyInInventory) return 3;
        return 4;
    }

    // Returns true if the password is correct
    public bool openObj(Item key)
    {
        string userPass = getTyped();
        int num = readObj(userPass);
        if (num == 2)
        {
            return true;
        }
        return false;
    }

    // Gets the password string that the player typed
    public string getTyped()
    {
        return FindObjectOfType<PasswordLogic>().getInput();
    }
}
