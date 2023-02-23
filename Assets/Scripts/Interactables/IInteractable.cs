using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This interface should be used for items you can pick up, or interactive puzzle objects
public interface IInteractable
{
    void Grab();
    void Release();

    public static bool isLayerInLayerMask(int layerToCheck, LayerMask layerMask)
    {
        // bitshifting to interpret the layerMask.  Don't worry if this looks confusing.
        return layerMask.value == (layerMask.value | (1 << layerToCheck));
    }
}