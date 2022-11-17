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
    private bool passwordNeeded = false;
    public Item key;
    private int identity = -1;

    // Constructor
    public GatesObj()
    {
        locked = false;
        keyNeeded = false;
        passwordNeeded = false;
        identity = -1;
    }


    // Methods
    public bool isLocked() { return locked; }
    public void setLocked(bool var) { locked = var; }
    public bool isKeyNeeded() { return keyNeeded; }
    public void setKeyNeeded(bool var) { keyNeeded = var; }
    public bool isPasswordNeeded() { return passwordNeeded; }
    public void setPasswordNeeded(bool var) { passwordNeeded = var; }
    public int getIdentity() { return identity; }
    public void setIdeneity(int var) { identity = var;  }


    public bool openObj(Item key)
    {
        if (!key.isType(ItemType.KEY)) return false;
        //if (key == identity) return true;
        return false;
    }
}
