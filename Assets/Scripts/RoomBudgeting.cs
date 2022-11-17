using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* --- DEV NOTES ---
-   For now, we're assuming that there will be several set locations in a room where any object can spawn, so long as it isn't too
large for that location. Eventually we can add functionality for spawning probability and constraints, but just assume that any
object can spawn at the points for now.
-   STOPPING POINT: Work on version 3. (Big comment near bottom)
*/

/**
* Creates objects in the room this script is attached to at predefined points in that specific room.
*
*/
public class RoomBudgeting : MonoBehaviour {

    private List<GameObject> spawns;
    [SerializeField]
    
    // These lists should all be the same size! The index matters, because each index corresponds to one object. (Use a Map instead?)
    private List<Transform> spawnables = new List<Transform>();
    [SerializeField]
    private List<int> maximums = new List<int>();
    [SerializeField]
    private List<int> minimums = new List<int>();

    [SerializeField]
    private int budget = 0;

    void Awake() {
        // Placeholder objects are stored in the Prefabs/RoomObjPlaceholders folder for now, with an Attributes script attached to each one

        int tempBudget = budget; // Used for spawning the objects because this instance will be deprecated during that, while the original instance persists
        spawns = new List<GameObject>();
        Transform[] roomObjTransforms = transform.GetComponentsInChildren<Transform>(false);
        foreach (Transform t in roomObjTransforms) {
            if (t.gameObject.tag.Equals("ObjectSpawn")) {
                spawns.Add(t.gameObject);
            }
        }

        // The objects' transforms to be allowed to spawn in the room should be added to the script's list through the serialized field in the editor.
        List<Transform> toSpawn = new List<Transform>();

        /* Version 2: Same as version 1, but when an object whose weight exceeds the remaining budget is chosen, the loop doesn't stop.
        Instead, it doesn't modify the remaining budget for that iteration and randomly chooses another object that is NOT the one that
        was just chosen. It keeps doing this, ignoring each object from the list that has too much weight, until an object is chosen that
        fits in the remaining budget, which is spawned. This process repeats until the point that no object in the list will fit in the budget.
        // Implement version 2 here. It'll likely need a second list that holds references to the ignored objects or incedes to exclude them
        // from the random selection somehow.
        */
        while (spawnables.Count != 0 /* && toSpawn.Count < spawns.Count */ ) {
            int chosenIndex = (int) Random.Range(0, spawnables.Count);
            int objWeight = spawnables[chosenIndex].gameObject.GetComponent<Attributes>().weight;
            if (objWeight <= tempBudget) {
                toSpawn.Add(spawnables[chosenIndex]);
                tempBudget -= objWeight;
            } else {
                spawnables.Remove(spawnables[chosenIndex]);
            }
        }
        string test = "With a budget of " + budget + ", spawned these objects: ";
        foreach (Transform t in toSpawn) {
            test += t.name + " (" + t.GetComponent<Attributes>().weight + "), ";
        }
        test += "with " + tempBudget + " budget left over.";

        Debug.Log(test);


        /* Version 3: Add the ability for devs to set constraints on how many objects can spawn in a room. Have a minimum and maximum
        associated with that room through more lists serialized from this script, or making use of a Map-type list to pair the mins and
        maxes with the objects that can be spawned. Also, have a global max and min that are attached to each object's attributes script,
        and these values apply to every room the object spawns in. The most restrictive constraint between the local-to-room and global
        ones is the one that applies (add an override option?). In addition, I might want to add a safeguard that makes sure all of the
        arrays used for the objects are the same size, and if they aren't, average them and set default values. (Thinking I should use
        the array with the actual objects to decide the sizes of the other ones). Then, after this, add actual spawn points in the room
        and uncomment the part of the while loop conditional that stops the loop when an object has spawned in every available spawn point.
        */



        /* Original object weights:
            Easy Enemy - 6
            Medium Enemy - 13
            Hard Enemy - 28
            Weak Weapon - 15
            Strong Weapon - 30
            Key - 45
            Stamina Refill - 10
        */

        foreach (Transform t in toSpawn) {
            GameObject.Instantiate(t.gameObject);
            // Set position and stuff here and put it into the actual room in the scene
        }
        
    }
    
}
