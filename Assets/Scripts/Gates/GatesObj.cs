using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** Creates gate objects (interactable, non-item objects) such as doors, chests, or levers  
 */

public class GatesObj : MonoBehaviour
{
    //private void Start()
    //{
    //openObj(null, "string");
    //}

    // Fields
    [SerializeField] private bool locked = false;
    [SerializeField] private bool keyNeeded = false;
    public Item key;
    [SerializeField] private bool passwordNeeded = false;
    [SerializeField] private string password = "none";
    [SerializeField] private int identity = -1;

    // Constructors
    /*
    public GatesObj()
    {
        locked = false;
        keyNeeded = false;
        passwordNeeded = false;
        identity = -1;
    }
    public GatesObj(bool locked, bool keyNeeded, Item key, int identity)
    {
        this.locked = locked;
        this.keyNeeded = keyNeeded;
        this.key = key;
        this.identity = identity;
    }
    public GatesObj(bool locked, bool passwordNeeded, string password, int identity)
    {
        this.locked = locked;
        this.passwordNeeded = passwordNeeded;
        this.password.Equals(password);
        this.identity = identity;
    }
    */
    // Methods
    public bool isLocked() { return locked; }
    public void setLocked(bool var) { locked = var; }
    public bool isKeyNeeded() { return keyNeeded; }
    public void setKeyNeeded(bool var) { keyNeeded = var; }
    public bool isPasswordNeeded() { return passwordNeeded; }
    public void setPasswordNeeded(bool var) { passwordNeeded = var; }

    // If player tries to open the gate with an item
    // returns 0 if the gate is unlocked or if the gate was not locked to begin with
    // returns 1 if the password entered is incorrect
    // returns 2 if the password is correct
    // returns 3 if the player does not try to open the gate with a key
    // returns 4 if the key incorrect key was inserted
    // returns 5 if the key correct key was inserted
    public int readObj(Item key, string userPass)
    {
        if (!locked) return 0;
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

    // If player tries to open the gate with no items in hand
    // returns 0 if the gate was not locked to begin with
    // returns 1 if the password entered is incorrect
    // returns 2 if the password is correct

    public int readObj(string userPass)
    {
        if (!locked) return 0;
        if (!password.Equals(userPass)) return 1;
        return 2;
    }

    public void openObj(Item key, string userPass)
    {
        int num = readObj(key, userPass);
        if (num == 0 || num == 2)
        {
            gameObject.SetActive(false);
        }
    }


}
