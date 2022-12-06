using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

// This is the main script that will be on item prefabs in the scene

// This base class does basic inventory management when grabbed or released

// [[[NOTE]]] - assumes the following:
    // player has a non-trigger Collider on a gameobject with the "Player" tag

public class Item : MonoBehaviour, IInteractable
{
    // constants
    const string PLAYER_TAG = "Player";

    // public fields
    public event Action<Item> OnGrab;

    // serializable fields
    [SerializeField] string itemName;
    [SerializeField] ItemType type = ItemType.GENERAL;
    [SerializeField] Sprite inventorySprite;
    [SerializeField] int maxStackSize = 1;
    [SerializeField] int currentNumCharges = 1;
    [SerializeField] bool isShiny;
    [SerializeField] string description;

    // cached fields
    Transform playerTrans;
    //Rigidbody rb;
    Collider col;
    Camera mainCam;

    // starting scale
    Vector3 startLocalScale;

    // private fields
    bool inInventory;

    void Start()
    {
        playerTrans = FindObjectOfType<Player>().gameObject.transform;
        //rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        startLocalScale = transform.localScale;
        mainCam = Camera.main;
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
    }

    /// <summary>
    /// Handles most logic for when the player drops an item that's currently in their inventory.
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
        transform.position = playerTrans.position + playerTrans.forward * 2.5f - playerTrans.up * .3f; //getMouseWorldPosition();

        gameObject.SetActive(true);
        transform.parent = null;
        transform.localScale = startLocalScale;

        // add to ground
        processInteract(false);
    }

    /// <summary>
    /// Call this if the inventory just suceeded in adding this item.  
    /// It finishes item's pickup state change.
    /// </summary>
    public void onInventoryAddSuccess()
    {
        // remove from ground
        processInteract(true);
    }

    // handles things that are adjusted for both grabbing and releasing
    public void processInteract(bool putInInventory)
    {
        //gameObject.SetActive(!putInInventory);
        bringToUI(putInInventory);
        inInventory = putInInventory;        

        if (putInInventory)
        {
            //rb.velocity = rb.angularVelocity = Vector3.zero;            
        }
    }

    // deactivates physics components to bring picked-up item object to UI inventory slot
    // or reactivates them if the item is being dropped onto the ground
    void bringToUI(bool toUI)
    {
        //rb.useGravity = !toUI;
        col.enabled = !toUI;
    }

    /// <summary>
    /// Handles all logic for when the player uses an item.  The InventorySystem should remove this Item, and destroy it if applicable.
    /// </summary>

    // can be overridden in subclasses with the "override" keyword
    // base.Use() can be used then to call this from subclass
    public virtual void Use() {
        // the base Item class cannot be used
        Debug.Log($"{itemName} was activated");
    }

    public bool isStackable()
    {
        return maxStackSize > 1;
    }

    public void DestroyItem()
    {
        //Debug.Log($"destroying {itemName}");
        Destroy(gameObject);
    }

    public string getItemName()
    {
        return itemName;
    }

    public int getStackSize()
    {
        return maxStackSize;
    }

    public int getCurrentNumCharges()
    {
        return currentNumCharges;
    }

    public bool removeCharge()
    {
        return --currentNumCharges <= 0;
    }

    public bool isType(ItemType itemType)
    {
        return type.Equals(itemType);
    }

    public Sprite getSprite()
    {
        return inventorySprite;
    }

    Vector3 getMouseWorldPosition()
    {
        /*Vector2Control mouseScreenPosControl = Mouse.current.position;
        Vector2 mouseScreenPos = new Vector2(mouseScreenPosControl.x.ReadValue(), mouseScreenPosControl.y.ReadValue());
        Vector3 directionToMousePos = mainCam.transform.position - mainCam.ScreenToWorldPoint(mouseScreenPos);*/

        RaycastHit hit;

        if (Physics.Raycast(mainCam.transform.position + mainCam.transform.up, mainCam.transform.forward, out hit, Mathf.Infinity, LayerMask.GetMask("Default")))
            return hit.point + Vector3.up * InventorySystem.ITEM_DROP_HEIGHT;
        else
            return mainCam.transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("collision" + other.gameObject.name);
        // ensures this is a valid item to be picked up, and the player is touching this
        if (other != null && gameObject != null && !inInventory && other.gameObject.tag.Equals(PLAYER_TAG))
            Grab();
    }
}

public enum ItemType { TOOL, GENERAL, KEY }