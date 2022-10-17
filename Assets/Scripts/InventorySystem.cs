using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : /*ScriptableObject*/MonoBehaviour {

    // NOTE - I have turned all InventoryItems to Items with comments so that
    //        dropping an item can reactivate the Item prefab in the scene
    //                                        - William Zollman (discord: wZollman)

    private static int MAX_SLOTS = 20;
    private /*InventoryItem*/InventorySlot[] inventory = new /*InventoryItem*/InventorySlot[MAX_SLOTS];

    void Awake() {
        // InventoryItem[] inventory = new InventoryItem[MAX_SLOTS];
        // inventory[0] = new InventoryItem("Hello"); //this was to test that I can add items to the array (Learning C# as I write)
        // Find a way to make a save file and then load that file into the inventory (if the game can incorporate saving)
    }

    private int CalculateEmptySlots() {
        int slots = 0;
        for (int i = 0; i < MAX_SLOTS; i++) {
            if (inventory[i] == null) {
                slots++;
            }
        }
        return slots;
    }

    // Start is called before the first frame update
    void Start() {
        // ensures items in scene can actually be picked up while testing
        SubscribeToItemsInScene();
    }

    // Update is called once per frame
    void Update() {
        
    }

    void Display() {
        
    }

    void Add(/*InventoryItem*/Item item) {
        int slot = CalculateEmptySlots();
        if (slot == 0) {
            Choose(item);
        }
        else if (slot > 0 && slot < (MAX_SLOTS + 1))
        {
            for (int i = 0; i < MAX_SLOTS; i++)
            {
                if (inventory[i] == null)
                {
                    inventory[i].setHeldItem(item);
                    break;
                }
            }
        } else {
            Debug.Log("INVENTORY SIZE ERROR::InventorySystem::Add(InventoryItem)");
        }
    }

    void Use() {
    
    }

    void Drop() {
    
    }

    void Choose(/*InventoryItem*/Item item) {
        
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
