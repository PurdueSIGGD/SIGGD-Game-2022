using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlot : MonoBehaviour
{
    // temporary constants for 3D object inventory
    const float SCALE_MULT_IN_UI = .075f;
    const float DIST_FRONT_OF_INV = .07f;
    const float VERT_OFFSET = .035f;
    const float ROTATE_SPEED = 20f;

    [SerializeField] ItemType stackType = ItemType.GENERAL;
    [SerializeField] Image border;
    [SerializeField] Image background;
    [SerializeField] TextMeshProUGUI stackText;

    [SerializeField] Color selectedColor = Color.white;

    // if the slot has an item (or stack of items in it)
    public bool hasStack { get { return stack.Count > 0; } }
    
    // the name of the item (or items in the stack), if there is at least an item in the slot
    public string stackName { get { return hasStack ? stack[0].getItemName() : ""; } }



    bool isSelected;
    List<Item> stack;
    
    Color unselectedColor; // set to InventorySystem prefab's slot border colors

    void Awake()
    {
        stack = new List<Item>();
        updateStackText();

        unselectedColor = border.color;
        setHighlight(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (hasStack && !stackType.Equals(ItemType.TOOL))
        {
            stack[0].transform.position = transform.position + transform.forward * -DIST_FRONT_OF_INV + transform.up * VERT_OFFSET;

            stack[0].transform.Rotate(Vector3.one * ROTATE_SPEED * Time.deltaTime);
        }
    }

    /// <summary>
    /// Attempts to add the item to this InventorySlot, not allowing the following additions:<br/>
    /// - item is null<br/>
    /// - item's type doesn't match this InventorySlot's stackType (using ItemType enum)<br/>
    /// - item doesn't match the items in the stack (item.name mismatch)<br/>
    /// - this InventorySlot's stack is full<br/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns>Returns true if addition succeeded.</returns>
    public bool addToStack(Item item)
    {        
        // doesn't add a null item
        if (item == null)
        {
            Debug.LogError($"Cannot add null item");
            return false;
        }
        else if (!item.isType(stackType)) // doesn't add an item that doesn't match the stack
        {
            //Debug.Log($"Cannot add item \"{item.getItemName()}\" to stack of \"{stackName}\"");
            return false;
        }

        // checks for stack adding issues
        if (hasStack)
        {
            if (!item.getItemName().Equals(stackName)) // doesn't add an item that doesn't match the stack
            {
                //Debug.Log($"Cannot add item \"{item.getItemName()}\" to stack of \"{stackName}\"");
                return false;
            }
            else if (getNumCharges() + item.getCurrentNumCharges() > stack[0].getStackSize()) // doesn't add an item that can't fit
            {
                //Debug.Log($"Cannot add item \"{item.getItemName()}\" to stack of \"{stackName}\" of size {getNumCharges()} >= {stack[0].getStackSize()}");
                return false;
            }
        }

        // move 3D object to inventory
        item.gameObject.SetActive(!hasStack && !stackType.Equals(ItemType.TOOL)); // doesn't display item in slot if there's already one being displayed
        item.transform.position = transform.position + transform.forward * -DIST_FRONT_OF_INV + transform.up * VERT_OFFSET;
        item.transform.parent = transform;
        item.transform.localScale *= SCALE_MULT_IN_UI;

        // adds item to stack & updates stack counter
        stack.Add(item);
        updateStackText();

        return true;
    }

    public Item removeCharge(bool shouldRemoveCharge)
    {
        if (!hasStack)
        {
            Debug.LogError($"Cannot remove from empty stack");
            return null;
        }

        int removeIndex = stack.Count - 1;
        Item item = stack[removeIndex];

        item.Use();

        // if forcing an item remove by not removing a charge     or     if charges == 0
        if (!shouldRemoveCharge || item.removeCharge())
            stack.RemoveAt(removeIndex);
        else // returns null if no removing occured
            item = null;

        updateStackText();

        return item;
    }

    void updateStackText()
    {
        int numCharges = getNumCharges();

        // doesn't display number in stack if stack doesn't exist or if the item isn't stackable
        // always displays your tool ammo
        if (stackType.Equals(ItemType.TOOL))
        {
            stackText.text = numCharges.ToString();

            if (hasStack)
                stackText.text += "/" + stack[0].getStackSize();
        }
        else if (!hasStack || !stack[0].isStackable())
            stackText.text = "";                    
        else
            stackText.text = getNumCharges().ToString();
    }

    // selects or unselects this inventory slot (will be controlled by InventorySystem)
    public void setSelected(bool selected)
    {
        isSelected = selected;
        setHighlight(isSelected);
    }

    public bool getIsSelected()
    {
        return isSelected;
    }

    public bool isStackType(ItemType type)
    {
        return stackType.Equals(type);
    }

    /// <summary>
    /// The number of charges held in the stack.  This is the sum of all of the charges that each item in the stack currently holds.<br/>
    /// For normal items without multiple charges, this is equivalent to the number of items in the stack.
    /// </summary>
    public int getNumCharges()
    {
        int result = 0;
        foreach (Item i in stack)
            result += i.getCurrentNumCharges();

        return result;
    }

    // highlights the item slot's background if it is selected
    void setHighlight(bool highlight)
    {
        border.color = highlight ? selectedColor : getUnselectedColor();
    }

    Color getUnselectedColor()
    {
        return unselectedColor;
    }
}