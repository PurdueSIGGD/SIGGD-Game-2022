using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventorySystem : /*ScriptableObject*/MonoBehaviour {

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

    private static int MAX_SLOTS = 3;

    [SerializeField] private GameObject slotHolder;

    private InventorySlot[] inventory = new InventorySlot[MAX_SLOTS];

    // the index of the current selected slot
    private int selectedSlotNum = 0;

    void Awake() {
        instance = this;

        // Grabs all children InventorySlots to base the inventory array on the InventoryCanvas prefab
        inventory = slotHolder.GetComponentsInChildren<InventorySlot>();
        MAX_SLOTS = inventory.Length;
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
            Drop();


        // process use
        if (Keyboard.current.eKey.wasPressedThisFrame)
            Use();        

        // process scroll selected slot
        int prevSelectedSlotNum = selectedSlotNum;
        float scrollValue = Mouse.current.scroll.ReadValue().y;
        if (scrollValue != 0)
        {
            // if scrolling down, slotChange = 1      if scrolling up, slotChange = -1
            int slotChange = (scrollValue < 0) ? 1 : -1;

            // changes the selected slot accordingly
            selectedSlotNum = (selectedSlotNum + MAX_SLOTS + slotChange) % MAX_SLOTS;
            inventory[prevSelectedSlotNum].setSelected(false);
            inventory[selectedSlotNum].setSelected(true);
        }
    }

    private int CalculateEmptySlots() {
        int slots = 0;
        for (int i = 0; i < MAX_SLOTS; i++) {
            if (inventory[i].getHeldItem() == null) {
                slots++;
            }
        }
        return slots;
    }

    void Display() {
        
    }

    /// <summary>
    /// Adds the passed item to the Inventory in the leftmost open inventory slot, taking the item off the gound.
    /// Ignores the item if all slots are filled.
    /// </summary>    
    /// <param name="item"> The Item script of the item GameObject that should be picked up. </param>
    /// <returns>True if there was room to pick up the item</returns>
    public void Add(Item item) {
        int slot = CalculateEmptySlots();
        if (slot == 0) {
            Choose(item);
        }
        else if (slot > 0 && slot < (MAX_SLOTS + 1))
        {
            for (int i = 0; i < MAX_SLOTS; i++)
            {
                if (inventory[i].getHeldItem() == null)
                {
                    // places the item into the inventory slot
                    inventory[i].setHeldItem(item);

                    // finishes the item's state change to being in the UI
                    item.onInventoryAddSuccess();

                    // ends search for open slot
                    break;
                }
            }
        } else {
            Debug.Log("INVENTORY SIZE ERROR::InventorySystem::Add(InventoryItem)");
        }
    }

    /// <summary>
    /// Uses the item in the currently selected inventory slot.  Then, the item is destroyed and removed from the inventory.
    /// </summary>
    public void Use() {
        Item curItem = inventory[selectedSlotNum].getHeldItem();

        if (curItem != null)
        {
            curItem.Use();

            // drops item
            Drop();

            // destroys dropped item
            curItem.DestroyItem();
        }
    }

    /// <summary>
    /// Drops the item in the currently selected inventory slot in from of the player.  
    /// The distance from the player is determined by the vertical angle of the camera.
    /// </summary>
    public void Drop() {
        Item curItem = inventory[selectedSlotNum].getHeldItem();

        if (curItem != null)
        {
            curItem.Release();
            inventory[selectedSlotNum].setHeldItem(null);
        }
    }

    void Choose(Item item) {
        // select which item you want to drop to trade out this item instead
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
