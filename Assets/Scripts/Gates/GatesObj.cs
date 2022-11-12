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
    // private Item item 

    public bool isLocked() { return locked; }
    public void setLocked(bool var) { locked = var; }
    public bool isKeyNeeded() { return keyNeeded; }
    public void setKeyNeeded(bool var) { keyNeeded = var; }
    public bool isPasswordNeeded() { return passwordNeeded; }
    public void setPasswordNeeded(bool var) { passwordNeeded = var; }
}
