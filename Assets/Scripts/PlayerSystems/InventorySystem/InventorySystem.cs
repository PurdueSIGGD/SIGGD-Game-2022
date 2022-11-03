using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventorySystem : MonoBehaviour {

    // [[[NOTE]]] - assumes the following:
        // player has a non-trigger Collider on a gameobject with the "Player" tag
        // the Canvas component on the InventorySystem has the main camera assigned to it (for 3D item display)
        // This class will be accessed with InventorySystem.instance

    // CONTROLS -
        // drop selected item - 'Q'
        // use selected item - 'E'
        // pickup item - touch it with player *see above note*

    // The static instance used to use this class
    public static InventorySystem instance;

    // The height at which items are dropped at before falling toward the ground
    public const float ITEM_DROP_HEIGHT = 1f;


    [SerializeField] private GameObject slotHolder;

    private InventorySlot[] inventory;

    // the index of the current selected slot
    private int selectedSlotNum = 0;
    private boolean chooseLock = false;
    private Item chooseItem = null;

    void Awake() {
        instance = this;

        // Grabs all children InventorySlots to base the inventory array on the InventoryCanvas prefab
        // ASSUMES that all slots are ordered in the hierarchy top->bottom = left->right in the HUD
        inventory = GetComponentsInChildren<InventorySlot>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // ensures items in scene can actually be picked up while testing
        SubscribeToItemsInScene();

        // sets the first inventory slot to be selected
        inventory[selectedSlotNum].setSelected(true);
    }

    // Update is called once per frame
    void Update()
    {
        // process drop
        if (Keyboard.current.qKey.wasPressedThisFrame) 
        {
            Drop(false);
            if (chooseItem != null) {
                Add(chooseItem);
                chooseItem = null;
            }
        }

        // process use
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            Use();
        }

        // get rid of the item that would have overflowed the inventory
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            if (chooseItem != null) {
                Drop(chooseItem); //2nd drop to manage dropping an item not in the inventory
                chooseItem = null;
            }
        }

        // End player stun and change the chooseLock variable
        if (chooseLock && chooseItem == null)
        {
            EndChoose();
        }


        // process scroll selected slot
        int prevSelectedSlotNum = selectedSlotNum;
        float scrollValue = Mouse.current.scroll.ReadValue().y;
        if (scrollValue != 0)
        {
            // if scrolling down, slotChange = 1      if scrolling up, slotChange = -1
            int slotChange = (scrollValue < 0) ? 1 : -1;

            // changes the selected slot accordingly
            // scrolling up selects right slot, scrolling down selects right slot
            selectedSlotNum = (selectedSlotNum + inventory.Length + slotChange) % inventory.Length;
            inventory[prevSelectedSlotNum].setSelected(false);
            inventory[selectedSlotNum].setSelected(true);
        }
    }

    private int CalculateEmptySlots(ItemType type) {
        int slots = 0;
        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i].isStackType(type))
                slots++;
        }
        return slots;
    }

    // don't think we ever will need  this
    private int FindIndexOf(Item item) {
        for (int i = 0; i < inventory.Length; i++) {
            if (inventory[i].stackName == item.getItemName()) {
                return i;
            }
        }
        return -1;
    }

    /// <summary>
    /// Adds the passed item to the Inventory in the leftmost open inventory slot, taking the item off the gound.
    /// Ignores the item if all slots are filled.
    /// </summary>    
    /// <param name="item"> The Item script of the item GameObject that should be picked up. </param>
    /// <returns>True if there was room to pick up the item</returns>
    public void Add(Item item) {
        bool addSucceeded = false;

        for (int i = 0; i < inventory.Length; i++)
        {            
            // places the item into the inventory slot
            bool stackAddSuccessful = inventory[i].addToStack(item);

            // finishes the item's state change to being in the UI
            if (stackAddSuccessful)
                item.onInventoryAddSuccess();

            if (stackAddSuccessful) // Do these need to be different if statements?
            {
                // ends search for open slot
                addSucceeded = stackAddSuccessful;
                break;
            }
        }

        if (!addSucceeded)
            Choose(item);
    }

    /// <summary>
    /// Uses the item in the currently selected inventory slot.  Then, the item is destroyed and removed from the inventory.
    /// </summary>
    public void Use() {
        // drops an item if there is one in the current inventory slot      
        Item droppedItem = Drop(true);

        if (droppedItem == null)
            return;

        // uses the item
        droppedItem.Use();

        // destroys dropped item        
        droppedItem.DestroyItem();
    }

    /// <summary>
    /// Drops the item in the currently selected inventory slot in from of the player.  
    /// The distance from the player is determined by the vertical angle of the camera.
    /// </summary>
    public Item Drop(bool shouldRemoveCharge) {
        InventorySlot curSlot = inventory[selectedSlotNum];

        // can't drop tool ammo or drop from an empty stack
        if (!curSlot.hasStack || (!shouldRemoveCharge && !curSlot.isStackType(ItemType.GENERAL)))
            return null;

        Item curItem = curSlot.removeCharge(shouldRemoveCharge);
        if (curItem != null)
            curItem.Release();

        return curItem;
    }

    public void Drop(Item item) {
        item.Release();
    }

    void Choose(Item item) {
        chooseLock = true;
        chooseItem = item;
        // ENABLE PLAYER STUN (either with this method or with calls to chooseLock (make that variable public))
    }

    void EndChoose() {
        chooseLock = false;
        // DISABLE PLAYER STUN (either with this method or with calls to chooseLock (make that variable public))
    }


    /// <summary>
    /// [temporary] This function registers Add() to every Item's OnGrab() event.
    /// 
    /// Once Rooms are implemented and spawn the items,
    /// the inventory system will subscribe itself to the spawned items' onPickup events.
    /// </summary>
    void SubscribeToItemsInScene()
    {
        // gets all items in scene
        Item[] allItems = FindObjectsOfType<Item>();

        // registers Add() to every item in the scene
        foreach (Item item in allItems)
            item.OnGrab += Add;
    }
}
