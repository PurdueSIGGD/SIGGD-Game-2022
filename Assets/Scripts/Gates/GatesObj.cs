using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** Creates gate objects (interactable, non-item objects) such as doors, chests, or levers  
 */

public class GatesObj : MonoBehaviour
{
    // Fields
    public bool keyNeeded = false;
    public Item key;
    public bool passwordNeeded = false;
    public bool passwordGiven = true;
    public string password;
    public int identity = -1;

    // If player tries to open the gate with an item
    // returns 1 if the password entered is incorrect
    // returns 2 if the password is correct
    // returns 3 if the player does not try to open the gate with a key
    // returns 4 if the key incorrect key was inserted
    // returns 5 if the key correct key was inserted
    public int readObj(Item key, string userPass)
    {
        if (!keyNeeded)
        {
            int i = readObj(userPass);
            if (i == 1) return 1;
            if (i == 2) return 2;
        }
        if (!key.isType(ItemType.KEY)) return 3;
        //if (key != identity) return 4;
        return 5;
    }

    // If player tries to open the gate with a password
    // returns 1 if the password entered is incorrect
    // returns 2 if the password is correct

    public int readObj(string userPass)
    {
        if (!password.Equals(userPass)) return 1;
        return 2;
    }

    // Returns true if the password is correct
    public bool openObj(Item key)
    {
        string userPass = getTyped();
        int num = readObj(key, userPass);
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
