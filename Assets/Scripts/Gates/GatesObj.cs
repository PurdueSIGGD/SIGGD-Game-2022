using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Creates gate objects (interactable, non-item objects) such as
     * doors, chests, or levers
     */

public class GatesObj : MonoBehaviour
{
    // Fields
    private bool locked = false;
    private bool keyNeeded = false;
    public Item key;
    private bool passwordNeeded = false;
    private string password;
    private int identity = -1;

    // Constructors
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

    // Methods
    public bool isLocked() { return locked; }
    public void setLocked(bool var) { locked = var; }
    public bool isKeyNeeded() { return keyNeeded; }
    public void setKeyNeeded(bool var) { keyNeeded = var; }
    public bool isPasswordNeeded() { return passwordNeeded; }
    public void setPasswordNeeded(bool var) { passwordNeeded = var; }

    // If player tries to open the gate with a key
    public bool openObj(Item key)
    {
        if (!key.isType(ItemType.KEY)) return false;
        //if (key == identity) return true;
        return false;
    }

    // If player tries to open the gate with the password
    public bool openObj(string password)
    {
        if (this.password.Equals(password)) return true;
        return false;
    }
}
