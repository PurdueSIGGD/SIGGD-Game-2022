using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This interface should be used for items you can pick up, or interactive puzzle objects
public interface IInteractable
{
    void Grab();
    void Release();
}