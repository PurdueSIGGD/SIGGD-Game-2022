using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    [SerializeField] Image border;
    [SerializeField] Image background;

    [SerializeField] Color selectedColor = Color.white;

    bool isSelected;
    Item heldItem;

    public void setHeldItem(Item item)
    {
        item.transform.position = transform.position;
        heldItem = item;
    }

    public Item getHeldItem()
    {
        return heldItem;
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

    // Start is called before the first frame update
    void Start()
    {
        setHighlight(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // highlights the item slot's background if it is selected
    void setHighlight(bool highlight)
    {
        background.color = highlight ? selectedColor : getUnselectedColor();
    }

    Color getUnselectedColor()
    {
        return background.color;
    }
}
