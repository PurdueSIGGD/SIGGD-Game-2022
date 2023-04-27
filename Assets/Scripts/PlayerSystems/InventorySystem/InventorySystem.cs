using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

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

    // tracks whether or not there is a stun gun projectile out so player can't spam
    public bool hasNotReCapturedProjectile;

    [SerializeField] private GameObject slotHolder;
    [SerializeField] GameObject shotSound;
    [SerializeField] GameObject cartridgeSound;

    private InventorySlot[] inventory;

    // the index of the current selected slot
    private int selectedSlotNum = 1;
    private bool chooseLock = false;
    private Item chooseItem = null;
    //private float[] useduration = { 0f, 0f, 0f, 0f, 0f };
    //private float[] curDuration = { -1f, -1f, -1f, -1f, -1f };
    private Animator animator;

    void Awake() {
        //instance = this;

        // Grabs all children InventorySlots to base the inventory array on the InventoryCanvas prefab
        // ASSUMES that all slots are ordered in the hierarchy top->bottom = left->right in the HUD
        //inventory = GetComponentsInChildren<InventorySlot>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // ensures items in scene can actually be picked up while testing
        //SubscribeToItemsInScene();

        // gets the animator on the player
        animator = transform.parent.GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (chooseLock) {
            new Slow(1, 100); //Don't know if Slow duration is frame or seconds (Kyle)
        }
        if (!chooseLock) {
            new Slow(0, 0); //Put this here just in case (Kyle)
        }
        // process drop
        /*if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            Drop(false);
            if (chooseItem != null) {
                Add(chooseItem);
                chooseItem = null;
            }
        }*/

        // process use
        /*if (Keyboard.current.eKey.wasPressedThisFrame && !Stunned.isStunned) // IF issues remove the stunned check because 
                                                                                // I wasn't able to check if it worked but it wasn't
                                                                                // erroring out
        {
            Use();
        }*/

        // get rid of the item that would have overflowed the inventory
        /*if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            if (chooseItem != null) {
                Drop(chooseItem); //2nd drop to manage dropping an item not in the inventory
                chooseItem = null;
            }
        }*/

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
            selectedSlotNum = (selectedSlotNum == 0) ? 3 : ((selectedSlotNum == 4) ? 1 : selectedSlotNum);
            inventory[prevSelectedSlotNum].setSelected(false);
            inventory[selectedSlotNum].setSelected(true);
        }



        //Update active item timers and destroy ended item uses
        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i].getCurDuration() > 0f)
            {
                inventory[i].setCurDuration(inventory[i].getCurDuration() - Time.deltaTime);
            }
            else if (inventory[i].getCurDuration() > -1f)
            {
                inventory[i].setCurDuration(-1f);
                inventory[i].setUseDuration(0f);
                Item droppedItem = Drop(inventory[i], true);
                droppedItem.End();
                droppedItem.DestroyItem();
            }
        }

        //Update active item timers and destroy ended item uses
        /* for (int i = 0; i < curDuration.Length; i++)
        {
            if (curDuration[i] > 0f)
            {
                curDuration[i] -= Time.deltaTime;
            }
            else if (curDuration[i] > -1f)
            {
                curDuration[i] = -1f;
                useduration[i] = 0f;
                Item droppedItem = Drop(inventory[i], true);
                droppedItem.DestroyItem();
            }
        } */

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

            if (stackAddSuccessful)
            {
                // finishes the item's state change to being in the UI
                item.onInventoryAddSuccess();

                // ends search for open slot
                addSucceeded = stackAddSuccessful;
                break;
            }
        }

        if (!addSucceeded)
            Choose(item);
    }


    public void isFiring()
    {
        if (Stunned.isStunned)
        {
            return;
        }
        UseStunGun();
    }

    public void isSecondaryFiring()
    {
        if (Stunned.isStunned)
        {
            return;
        }
        Use();
    }

    public void isPressingDrop()
    {
        Drop(false);
        if (chooseItem != null)
        {
            Add(chooseItem);
            chooseItem = null;
        }
    }


    /// <summary>
    /// Uses the item in the currently selected inventory slot.  If the current slot is not the Stun Gun, the item is destroyed and removed from the inventory.
    /// </summary>
    public void Use() {
        // determines if the player is trying to use their stun gun ammo or not
        bool isUsingStunGun = inventory[selectedSlotNum].isStackType(ItemType.TOOL);

        // uses the appropriate item
        if (isUsingStunGun)
            UseStunGun();
        else
            UseItem();
    }

    // For Using Stun Gun Ammo
    void UseStunGun()
    {
        // makes sure player can't spam projectiles
        if (hasNotReCapturedProjectile)
            return;

        // uses the stun gun ammo
        inventory[0].useStackItem();

        if (!inventory[0].hasStack)
            return;

        decrementStunGunAmmo();
        // plays animation
        animator.SetTrigger("Shoot");
        StartCoroutine(PlayShootSound());
    }

    IEnumerator PlayShootSound()
    {
        Instantiate(shotSound, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(1f);
        Instantiate(cartridgeSound, transform.position, Quaternion.identity);
    }

    // For Using items other than the Stun Gun Ammo
    void UseItem()
    {

        if (inventory[selectedSlotNum].getCurDuration() > -1f)
            return;

        InventorySlot slot = inventory[selectedSlotNum];

        if ((slot.stack == null) || (slot.stack.Count == 0))
            return;

        Item activeItem = slot.stack[slot.stack.Count - 1];
        
        if (activeItem == null)
            return;

        activeItem.Use();
        inventory[selectedSlotNum].setUseDuration(activeItem.useDuration);
        inventory[selectedSlotNum].setCurDuration(activeItem.useDuration);

        /* if (curDuration[selectedSlotNum] > -1f)
            return;

        InventorySlot slot = inventory[selectedSlotNum];
        Item activeItem = slot.stack[slot.stack.Count - 1];
        
        if (activeItem == null)
            return;

        activeItem.Use();
        useduration[selectedSlotNum] = activeItem.useDuration;
        curDuration[selectedSlotNum] = activeItem.useDuration; */



        // drops an item if there is one in the current inventory slot      
        /*Item droppedItem = Drop(true);

        if (droppedItem == null)
            return;

        // uses the item
        droppedItem.Use();

        // plays animation
        animator.SetTrigger("Use");

        // destroys dropped item        
        droppedItem.DestroyItem(); */
    }

    /// <summary>
    /// Drops the item in the currently selected inventory slot in from of the player.  
    /// The distance from the player is determined by the vertical angle of the camera.
    /// </summary>
    public Item Drop(bool shouldRemoveCharge) {

        if (inventory[selectedSlotNum].getCurDuration() > -1f)
            return null;

        InventorySlot curSlot = inventory[selectedSlotNum];
        return Drop(curSlot, shouldRemoveCharge);

        // can't drop tool ammo or drop from an empty stack
        /*if (!curSlot.hasStack || (!shouldRemoveCharge && !curSlot.isStackType(ItemType.GENERAL)))
            return null;

        Item curItem = curSlot.removeCharge(shouldRemoveCharge);
        if (curItem != null)
            curItem.Release();

        return curItem; */
    }

    public Item Drop(InventorySlot slot, bool shouldRemoveCharge)
    {
        //InventorySlot curSlot = slot;

        // can't drop tool ammo or drop from an empty stack
        if (!slot.hasStack || (!shouldRemoveCharge && !slot.isStackType(ItemType.GENERAL)))
            return null;

        Item curItem = slot.removeCharge(shouldRemoveCharge);
        if (curItem != null)
            curItem.Release();

        return curItem;
    }


    // NOTE - This function doesn't do anything.  Examine Item.Release() to see why this just prints out an error when called.
    //
    //        "Dropping" is a concept strictly within the inventory system.  This means that if an item isn't in the inventory, there is nothing to drop.
    //        In order to do what I think you're trying to do, we need to make separate functionality from the inventory that allows you to hold an object outside
    //        of your inventory.  Then, here, we will call some functionality to put down that extra object (again, separate from the actual inventory).
    /*public*/
    void Drop(Item item) {
        item.Release();
    }

    /// <summary>
    /// Removes one stun gun ammo from the inventory.  Should only be called when a stun gun projectile hits an enemy.
    /// </summary>
    public void decrementStunGunAmmo()
    {
        // stores slotNum for resetting after this removal of ammo
        int prevSelectedSlotNum = selectedSlotNum;
        bool succeeded = false;

        // iterates through slots until one with stun gun ammo is reached {
        for (int i = 0; i < inventory.Length; i++)
        {
            InventorySlot curSlot = inventory[i];

            // removes 1 ammo from the Stun Gun Ammo stack, if found
            if (curSlot.isStackType(ItemType.TOOL) && curSlot.hasStack)
            {
                // set so Drop() works as intended
                selectedSlotNum = i;

                // drops an ammo from the stun gun ammo slot     
                Item droppedItem = Drop(true);

                if (droppedItem == null)
                    return;

                // destroys dropped item        
                droppedItem.DestroyItem();

                succeeded = true;
                break;
            }                
        }

        if (!succeeded)
            Debug.LogError("Found no Stun Gun Ammo to decrement");

        // reset selected slot num
        selectedSlotNum = prevSelectedSlotNum;
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
    public void SubscribeToItemsInScene()
    {
        instance = this;

        // Grabs all children InventorySlots to base the inventory array on the InventoryCanvas prefab
        // ASSUMES that all slots are ordered in the hierarchy top->bottom = left->right in the HUD
        inventory = GetComponentsInChildren<InventorySlot>();

        // gets all items in scene
        Item[] allItems = FindObjectsOfType<Item>();

        // registers Add() to every item in the scene
        foreach (Item item in allItems)
            item.OnGrab += Add;

        // sets the first inventory slot to be selected
        inventory[selectedSlotNum].setSelected(true);
    }
}
