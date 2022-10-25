using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    // temporary constants for 3D object inventory
    const float SCALE_MULT_IN_UI = .075f;
    const float DIST_FRONT_OF_INV = .07f;
    const float VERT_OFFSET = .035f;
    [SerializeField] float ROTATE_SPEED = 20f;

    [SerializeField] Image border;
    [SerializeField] Image background;

    [SerializeField] Color selectedColor = Color.white;

    bool isSelected;
    Item heldItem;
    Color unselectedColor; // set to InventorySystem prefab's slot border colors

    void Awake()
    {
        unselectedColor = border.color;

        setHighlight(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (heldItem != null)
            heldItem.transform.Rotate(Vector3.one * ROTATE_SPEED * Time.deltaTime);
    }

    public void setHeldItem(Item item)
    {
        heldItem = item;

        if (item != null)
        {
            item.transform.position = transform.position + transform.forward * -DIST_FRONT_OF_INV + transform.up * VERT_OFFSET;
            item.transform.parent = transform;
            item.transform.localScale *= SCALE_MULT_IN_UI;
        }        
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
