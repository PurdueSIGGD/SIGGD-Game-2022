using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class BakeLevelNav : MonoBehaviour
{
    public NavMeshSurface surface;

    // Use this for initialization
    public void BuildNavigation()
    {
        //Even though this is simple, leaving as a seperate script in case I need to expand later
        surface.BuildNavMesh();
    }
}
