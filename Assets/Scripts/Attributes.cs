using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attributes : MonoBehaviour
{

    public int weight = 0; // How much of the budget this object takes up

    [SerializeField]
    private string type = "";
    // Represents the type of object this is. Choices include "enemy", "item", "structure", etc.

}
