using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This is the main script that will be on item prefabs in the scene

// This base class does basic inventory management when grabbed or released

// NOTE - assumes the following:
                // player has a non-trigger Collider on a gameobject with the "Player" tag
                // player has a Rigidbody because item doesn't
public class Item : MonoBehaviour, IInteractable
{
    // constants
    private const string PLAYER_TAG = "Player";

    // public fields
    public event Action<Item> OnGrab;

    // serializable fields
    [SerializeField] private string itemName;

    // cached fields
    private Rigidbody rb;
    private Collider col;

    // private fields
    private bool inInventory;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    /// <summary>
    /// Handles all logic for when the player picks up an item.
    /// </summary>
    public void Grab()
    {
        if (inInventory)
        {
            Debug.LogError($"Tried to grab item \"{itemName}\" when it's already in player's inventory.");
            return;
        }

        // add to inventory if the InventorySystem has registered for the OnGrab event
        OnGrab?.Invoke(this);

        // remove from ground
        processInteract(true);
        Debug.Log($"{itemName} grabbed");
    }

    /// <summary>
    /// Handles most logic for when the player drops an item.
    /// The InventorySystem should remove this item from its inventory
    /// </summary>
    public void Release()
    {
        if (!inInventory)
        {
            Debug.LogError($"Tried to release item \"{itemName}\" when it's not in player's inventory.");
            return;
        }

        // move to cursor pos     
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, Mathf.Infinity, LayerMask.GetMask("Default")))
            transform.position = hit.point;
        else
            transform.position = Camera.main.transform.position;

        // add to ground
        processInteract(false);
        Debug.Log($"{itemName} released");
    }

    // handles things that are adjusted for both grabbing and releasing
    void processInteract(bool putInInventory)
    {
        //gameObject.SetActive(!putInInventory);
        bringToUI(putInInventory);
        inInventory = putInInventory;
    }

    // deactivates physics components to bring picked-up item object to UI inventory slot
    // or reactivates them if the item is being dropped onto the ground
    void bringToUI(bool toUI)
    {
        rb.useGravity = !toUI;
        col.enabled = !toUI;
    }

    /// <summary>
    /// Handles all logic for when the player uses an item.
    /// </summary>

    // can be overridden in subclasses with the "override" keyword
    // base.Use() can be used then to call this from subclass
    public virtual void Use() {
        // the base Item class cannot be used
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("colliding");
        // ensures this is a valid item to be picked up, and the player is touching this
        if (collision != null && gameObject != null && !inInventory && collision.gameObject.tag.Equals(PLAYER_TAG))
            Grab();
    }
}