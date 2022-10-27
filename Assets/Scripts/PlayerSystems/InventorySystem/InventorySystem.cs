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

    private static int NUMBER_OF_GENERAL_SLOTS = 3;
    private static int MAX_STACKS = 3;
    private static int NUMBER_OF_KEY_SLOTS = 1;
    private static int NUMBER_OF_TOOL_SLOTS = 1;
    private static int INDEX_OF_KEY = NUMBER_OF_GENERAL_SLOTS;
    private static int INDEX_OF_TOOL = NUMBER_OF_GENERAL_SLOTS + NUMBER_OF_KEY_SLOTS;
    private static int MAX_SLOTS = NUMBER_OF_GENERAL_SLOTS + NUMBER_OF_KEY_SLOTS + NUMBER_OF_TOOL_SLOTS;

    [SerializeField] private GameObject slotHolder;

    private InventorySlot[] inventory = new InventorySlot[MAX_SLOTS];
    private int[] itemsPerSlot = new int[MAX_SLOTS];

    // the index of the current selected slot
    private int selectedSlotNum = 0;

    void Awake() {
        instance = this;

        // Grabs all children InventorySlots to base the inventory array on the InventoryCanvas prefab
        inventory = slotHolder.GetComponentsInChildren<InventorySlot>();
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

    private int CalculateEmptyGeneralSlots() {
        int slots = 0;
        for (int i = 0; i < INDEX_OF_KEY; i++) {
            if (inventory[i] == null) {
                slots++;
            }
        }
        return slots;
    }

    private int CalculateEmptyKeySlots() {
        int slots = 0;
        for (int i = INDEX_OF_KEY; i < INDEX_OF_TOOL; i++) {
            if (inventory[i] == null) {
                slots++;
            }
        }
        return slots;
    }

    private int CalculateEmptyToolSlots() {
        int slots = 0;
        for (int i = INDEX_OF_TOOL; i < MAX_SLOTS; i++) {
            if (inventory[i] == null) {
                slots++;
            }
        }
        return slots;
    }

    private int FindIndexOf(Item item) {
        for (int i = 0; i < MAX_SLOTS; i++) {
            if (inventory[i] == item) {
                return i;
            }
        }
        return -1;
    }

    void Display() {
        //I don't know if we need this method or how to imp it
    }

    /// <summary>
    /// Adds the passed item to the Inventory in the leftmost open inventory slot, taking the item off the gound.
    /// Ignores the item if all slots are filled.
    /// </summary>    
    /// <param name="item"> The Item script of the item GameObject that should be picked up. </param>
    /// <returns>True if there was room to pick up the item</returns>
    public void Add(Item item) {
        int slot = CalculateEmptyGeneralSlots();
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

    void Add(Item item, string anotherVar) { //Second var is so it doesn't complain
        int slots = -1;
        string type = ""; //Empty String so the code doesn't error before we have a method to determine what type of item it is
        // string type = item.GetItemType();

        if (type.Equals("General")) {
            slots = CalculateEmptyGeneralSlots();
            for (int i = 0; i < INDEX_OF_KEY; i++) {
                if (inventory[i].Equals(item)) {
                    if (item.isStackable()) { //Need a way to check if it is stackable
                        if (itemsPerSlot[i] < MAX_STACKS) {
                            itemsPerSlot[i]++;
                            return;
                        }
                    }
                }
            }

            if (slots != 0) {
                for (int i = 0; i < INDEX_OF_KEY; i++) {
                    if (inventory[i] ==  null) {
                        inventory[i] = item;
                        itemsPerSlot[i] = 1;
                        break;
                    }
                }
            }

            Choose(item);
        } else if (type.Equals("Key")) { // Assumes that keys aren't stackable
            slots = CalculateEmptyKeySlots();
            if (slots != 0) {
                for (int i = INDEX_OF_KEY; i < INDEX_OF_TOOL; i++) {
                    if (inventory[i] == null) {
                        inventory[i] = item;
                        return;
                    }
                }
            }

            Choose(item);
        } else if (type.Equals("Tool")) { // Assumes that tools are not stackable
            for (int i = INDEX_OF_TOOL; i < MAX_SLOTS; i++) {
                if (inventory[i] == null) {
                    inventory[i] = item;
                    return;
                }
            }

            Choose(item);
        }

    }

    /// <summary>
    /// Uses the item in the currently selected inventory slot.  Then, the item is destroyed and removed from the inventory.
    /// </summary>
    public void Use() {
        Item curItem = inventory[selectedSlotNum].getHeldItem();

        if (curItem != null)
        {
            int index = FindIndexOf(curItem);
            int stacks = itemsPerSlot[index];
            curItem.Use();

            // drops item
            if (stacks <= 1) {
                Drop();
            } else {
                itemsPerSlot[index]--;
            }

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
        //Choose when the inventory is full
        //I dont know how to implement at this moment [Just brainstorming here]
        //Open the inventory 
        //Display what item do you want to drop
        //Key press to not pick up the item
        //If they want to keep the item, they select the item that they want to drop
        //Press a key if they want to drop the currently selected item

        // Q to drop new item
        // R to drop selected item to replace it

        string type = "";
        // string type = item.GetItemType();

        while (true) {
            if (type.Equals("General")) {
                if (Keyboard.current.qKey.wasPressedThisFrame) {
                    return;
                } else if (Keyboard.current.rKey.wasPressedThisFrame) {
                    Drop();
                    Add(item, ""); //Get rid of string when the version of Add() is selected
                    return;
                }
            } else if (type.Equals("Key")) {
                if (Keyboard.current.qKey.wasPressedThisFrame) {
                    return;
                } else if (Keyboard.current.rKey.wasPressedThisFrame) {
                    Drop();
                    Add(item, ""); //Get rid of string when the version of Add() is selected
                    return;   
                }
            } else if (type.Equals("Tool")) {
                if (Keyboard.current.qKey.wasPressedThisFrame) {
                    return;
                } else if (Keyboard.current.rKey.wasPressedThisFrame) {
                    Drop();
                    Add(item, ""); //Get rid of string when the version of Add() is selected
                    return;
                }
            }
        }
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
