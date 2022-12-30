using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* --- DEV NOTES ---
-   For now, we're assuming that there will be several set locations in a room where any object can spawn, so long as it isn't too
large for that location. Eventually we can add functionality for spawning probability and constraints, but just assume that any
object can spawn at the points for now.
- STOPPING POINT: 
*/

/**
* Creates objects in the room this script is attached to at predefined points in that specific room.
*
*/
public class RoomBudgeting : MonoBehaviour {

    private List<GameObject> spawns;
    [SerializeField]
    private List<Transform> spawnables = new List<Transform>();
    private int budget;

    /// <summary>
    /// Finds the spawnpoints in the room with the tag "ObjectSpawn" (assuming this script is attached to the room and
    /// the objects in the room are children of the room) and adds them to a list.
    /// </summary>
    public RoomBudgeting(int budget) {
        this.budget = budget;
    }

    void Awake() {
        // First, assume that every object can be generated with the same likelihood until the budget value of the room is exceded.
        // Currently, the immplementation for the generation puts a RoomGenerator script with every room upon floor generation. Thinking
        // that the same could be done with this script, or the RoomGenerator script could instantiate and call this one instead for greater
        // customizability with parameters for each room prefab.

        // We do still need a way for this script to choose which objects to spawn. It'll need to reference the prefabs for those objects
        // and possibly have a list passed in to tell the script which objects are viable for that specific room.

        // Placeholder objects are stored in the Prefabs/RoomObjPlaceholders folder for now, with an Attributes script attached to each one

        spawns = new List<GameObject>();
        Transform[] roomObjTransforms = transform.GetComponentsInChildren<Transform>(false);
        foreach (Transform t in roomObjTransforms) {
            if (t.gameObject.tag.Equals("ObjectSpawn")) {
                spawns.Add(t.gameObject);
            }
        }

        // The objects' transforms to be allowed to spawn in the room should be added to the script's list through the serialized field in the editor.
        List<Transform> toSpawn = new List<Transform>();
        bool stopped = false;
        while (!stopped) {
            int chosenIndex = (int) Random.Range(0, spawnables.Count);
            int objWeight = spawnables[chosenIndex].gameObject.GetComponent<Attributes>().weight;
            if (objWeight <= budget) {
                toSpawn.Add(spawnables[chosenIndex]);
                budget -= objWeight;
            } else {
                stopped = true;
                Debug.Log("Budget limit reached.");
            }
        }
        // Right now, this randomly chooses objects to spawn in the room until an object is chosen that is over the remaining budget, at which point
        // the whole thing stops. It should instead search for objects with a smaller weight to spawn at this point to use as much of the budget as possible,
        // but we can get to that later.

        foreach (Transform t in toSpawn) {
            GameObject.Instantiate(t.gameObject);
            // Set position and stuff here and put it into the actual room in the scene
        }
        
    }
    
}
