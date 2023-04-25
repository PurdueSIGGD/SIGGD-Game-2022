using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/** Creates gate objects (interactable, non-item objects) such as doors, chests, or levers  
 */

public class GatesObj : MonoBehaviour
{
    const string glyphs = "QWERTYUIOPASDFGHJKLZXCVBNM1234567890-";

    public bool keyNeeded = false;
    public bool passwordGiven = true;
    public string password = null;

    public void Start()
    {
        // Sets up password of each door based on the floor
        int floor = 0;
        int activeScene = SceneManager.GetActiveScene().buildIndex;
        if (activeScene < 5) floor = 1;
        if (activeScene > 4 && activeScene < 9) floor = 2;
        if (activeScene < 8) floor = 3;

        for (int i = 0; i < floor + 2; i++)
        {
            password += glyphs[Random.Range(0, glyphs.Length - 1)];
        }
    }
    //Random.Range(0, 1);

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
    public bool openObj()
    {
        string userPass = getTyped();
        int num = readObj(userPass);
        if (num == 2 || num == 3)
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
