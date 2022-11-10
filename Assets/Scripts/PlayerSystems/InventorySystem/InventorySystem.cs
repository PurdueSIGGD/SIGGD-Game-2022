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
            Drop(false);


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

            if (stackAddSuccessful)
            {
                // ends search for open slot
                addSucceeded = stackAddSuccessful;
                break;
            }
        }

        if (!addSucceeded)
            Choose(item);
    }

    void Add(Item item, string anotherVar) { //Second var is so it doesn't complain
        /*   // commented out because of compiler errors

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
        */
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





        // ---------- [NOTE]    -    PLEASE READ    -    UNITY ESSENTIALS ----------
        //
        //
        //
        //
        // In Unity, you do *NOT* want to use while loops like this where you wait for input.        
        //
        //     We do have access to writing main() in Unity, so we don't have complete control over every function call made during the running of our game.
        //     Instead, when we need it, we have a class extend MonoBehaviour, and then we can use certain reserved functions like Awake(), Start(), and Update().
        //     Unity will call these functions from its internal running of the game, which is how the code we write is accessed.  The ONLY way to have any of your
        //     code is run is by having execution start in one of the functions that Unity calls, including those mentioned above.  You can then call code that is
        //     outside of those functions, and even use classes that aren't MonoBehaviours and resemble classes you would write normally outside of Unity.
        //
        //     Extending MonoBehaviour makes the class you write a Component, which is something that can be added to GameObjects in Unity.
        //     Everything you see in the Unity Editor heirarchy is a GameObject with various Components or nested GameObjects attached.
        //
        //     In our code, we use Awake() and Start() for doing any additional setup for the GameObject our MonoBehaviour is attached to,
        //     including any Component on that GameObjec, including the MonoBeviour script we wrote.  This setup is done once during the beginning of the
        //     GameObject's lifetime, and we typically use Update() if changes may need to be made over a GameObject's lifetime.
        //
        //     Every frame, Unity's internals will call every Update() using a singular thread.  This means that all the code in all of the Update() functions
        //     that are used in the game Scene at the current moment, are all trying to be executed by Unity within one single frame (often 1/60 of a second).
        //     So, if our code takes too long to execute, it will lag the game since the code that's supposed to fit within one frame is not actually being
        //     completed within one frame.  In fact, the game will freeze until all of the Update() functions have been completed for that frame.
        //
        //     This means that if, on a given frame, Choose() is called, the entire thread that is calling every MonoBehaviour's Update()
        //     will be stuck in this while loop, freezing the entire game, and almost definitely crashing it.  A new "frame" will not be reached internally
        //     by Unity because Choose() is never left.
        //     
        //     You want to implement while loop functionality like this by setting a boolean field, and moving the contents of this loop to an
        //         if (boolean) {
        //           loop internals ...
        //           return;
        //         }
        //
        //     It looks like this functionality is unfinished since the type of item is checked for, yet the same operations are done in each case.
        //     We can discuss what sort of functionality we want here.
        //     I think there can be an easier solution that doesn't take the player out of the game world and into a menu.


        /*        // commented out because this function will crash the game, see below NOTE for explanation
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
        */
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
