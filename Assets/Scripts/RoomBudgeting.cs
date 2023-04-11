using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* --- DEV NOTES ---
    - NEXT: Fix an issue where the forced spawns aren't actually getting spawned.
*/

/// <summary>
/// Creates objects in the room this script is attached to at predefined points in that specific room.
/// </summary>
public class RoomBudgeting : MonoBehaviour {

    [SerializeField]
    private List<Transform> spawns;
    
    // These lists should all be the same size! The index matters, because each index corresponds to one object. (Use a Map instead?)
    [SerializeField]
    private List<Transform> spawnables = new List<Transform>();
    [SerializeField]
    private List<int> maximums = new List<int>(); // -1 indicates no maximum
    [SerializeField]
    private List<int> minimums = new List<int>();
    [SerializeField]
    private List<Transform> forcedSpawns = new List<Transform>();

    [SerializeField]
    private int budget = 0;

    List<Transform> toSpawn = new List<Transform>();
    private List<int> numSpawned = new List<int>();
    private int budgetLeft;
    private int spawnsLeft;


    void Awake() {
        // Placeholder objects are stored in the Prefabs/RoomObjPlaceholders folder for now, with an Attributes script attached to each one

        budgetLeft = budget; // Used for spawning the objects because this instance will be decremented during that, while the original instance persists

        Transform[] roomObjTransforms = transform.GetComponentsInChildren<Transform>(false);
        foreach (Transform t in roomObjTransforms) {
            if (t.gameObject.tag.Equals("RoomSpawn") && !spawns.Contains(t)) {
                print("A spawnpoint is being added to spawns.");
                for (int i = 0; i <= spawns.Count; i++) {
                    if (i == spawns.Count) {
                        spawns.Add(t);
                        break;
                    }
                    if (spawns[i] == null) {
                        spawns[i] = t;
                        break;
                    }
                }
            }
        }
        spawnsLeft = spawns.Count; // Used to end the loop when this number reaches zero, being decremented for each object spawned

        // Make all the serialized lists the same size as the spawnables list if their sizes don't match
        correctListSizeIntSpawnables(numSpawned, 0); // Needed here to make sure the list can store the number of each object spawned during the choosing process
        correctListSizeIntSpawnables(maximums, -1); // Again, -1 indicates no maximum
        correctListSizeIntSpawnables(minimums, 0);
        correctMinAndMax(minimums, maximums);
        correctListSizeTransformSpawns(toSpawn, null); // Must do this so forced spawns can be inserted at desired indexes

        // First, spawn all of the forced spawns -- objects that the user has attached to specific spawnpoints, saying that that
        // specific object MUST spawn at THAT specific spawnpoint. Spawn these in first and then remove those spawnpoints from the
        // "spawns" list, as they are no longer open spawnpoints. Keep in mind that the objects spawned here still count toward
        // the budget.

        for (int i = 0; i < forcedSpawns.Count; i++) {
            if (forcedSpawns[i] != null) {
                queueForcedSpawnIfAble(forcedSpawns[i], i);
            }
        }

        for (int i = 0; i < spawnables.Count; i++) {
            if (maximums[i] == 0) {
                removeFromPool(i);
                i--;
            }
        }

        for (int i = 0; i < spawnables.Count; i++) {
            while (minimums[i] > numSpawned[i] && spawnsLeft > 0) {
                if (!queueSpawnIfAble(i)) {
                    Debug.Log("Error: Can't meet the minimum spawns (" + minimums[i] + ") of " + spawnables[i].name + 
                        " because it exceeds the room's remaining budget!");
                    removeFromPool(i);
                    i--;
                    break;
                }
            }
        }

        while (spawnables.Count > 0 && spawnsLeft > 0) {
            int chosenIndex = (int) Random.Range(0, spawnables.Count);
            if (queueSpawnIfAble(chosenIndex)) {
                if (numSpawned[chosenIndex] >= maximums[chosenIndex] && maximums[chosenIndex] != -1) {
                    Debug.Log("Maximum spawns (" + maximums[chosenIndex] + ") reached for " + spawnables[chosenIndex].name + ".");
                    removeFromPool(chosenIndex);
                }
            } else {
                removeFromPool(chosenIndex);
            }
        }

        // Debug stuff, remove later
        // ***
        string test = "With a budget of " + budget + ", spawned these objects: ";
        foreach (Transform t in toSpawn) {
            if (t != null) {
                test += t.name + " (" + t.GetComponent<Attributes>().weight + "), ";
            }
        }
        test += "with " + budgetLeft + " budget left over.";
        Debug.Log(test);
        // ***

        /* Original object weights:
            Easy Enemy - 6
            Medium Enemy - 13
            Hard Enemy - 28
            Weak Weapon - 15
            Strong Weapon - 30
            Key - 45
            Stamina Refill - 10
        */

        spawnObjects(toSpawn, spawns);
    }

    /// <summary>
    /// Makes the given list (one of the modifier lists for room spawns) the same size as the 'spawnables' list to prevent errors.
    /// This shouldn't happen normally, but it's more for if a developer accidentally provides lists with mismatched sizes through the
    /// room object's serialized fields. All the lists should be the same size, and this method makes them the same size if they're not,
    /// and gives a warning that it was triggered.
    /// </summary>
    /// <param name="input"></param>
    private void correctListSizeIntSpawnables(List<int> input, int valueToAdd) {
        while (input.Count < spawnables.Count) {
            input.Add(valueToAdd);
            // Debug.Log("Added an item to a modifier list to equalize its size with that of 'spawnables'.");
        }
        while (input.Count > spawnables.Count) {
            input.RemoveAt(input.Count - 1);
            // Debug.Log("Removed an item from a modifier list to equalize its size with that of 'spawnables'.");
        }
    }

    /// <summary>
    /// Same function as correctListSizeInt, but adds objects instead of ints to correct the size, and uses the 'spawns' list as
    /// the one with the correct size.
    /// </summary>
    /// <param name="input"></param>
    /// <param name="objToAdd"></param>
    private void correctListSizeTransformSpawns(List<Transform> input, Transform transToAdd) {
        while (input.Count < spawns.Count) {
            input.Add(transToAdd);
            // Debug.Log("Added an item to a modifier list to equalize its size with that of 'spawnables'.");
        }
        while (input.Count > spawns.Count) {
            input.RemoveAt(input.Count - 1);
            // Debug.Log("Removed an item from a modifier list to equalize its size with that of 'spawnables'.");
        }
    }

    /// <summary>
    /// Makes sure that the minimum spawns of each item is less than or equal to the maximum. If it's not, the minimum is set to the maximum.
    /// </summary>
    /// <param name="mins"></param>
    /// <param name="maxes"></param>
    private void correctMinAndMax(List<int> mins, List<int> maxes) {
        for (int i = 0; i < spawnables.Count; i++) {
            if ((mins[i] > maxes[i]) && (maxes[i] != -1)) { // This shouldn't happen. That's why we're correcting it if it does.
                mins[i] = maxes[i]; // Sets the minimum value to the maximum value
            }
        }
    }

    /// <summary>
    /// Removes a spawnable object from the pool of objects that can be spawned. Removes its corresponding modifiers in the
    /// modifier lists, as well. (Make sure each modifier list is listed inside this method's code)
    /// </summary>
    /// <param name="index">
    /// </param>
    private void removeFromPool(int index) {
        spawnables.RemoveAt(index);
        maximums.RemoveAt(index);
        numSpawned.RemoveAt(index);
        minimums.RemoveAt(index);
    }

    /// <summary>
    /// Spawns the chosen objects at the available spawnpoints, making sure not to spawn them in the floor but instead sitting
    /// on the floor, depending on their collider. (Spawnpoints must be level with the floor)
    /// </summary>
    /// <param name="toSpawn"></param>
    /// <param name="spawns"></param>
    private void spawnObjects(List<Transform> toSpawn, List<Transform> spawns) {
        for (int i = 0; i < toSpawn.Count; i++) {
            if (toSpawn[i] != null) {
                GameObject spawnedObj = GameObject.Instantiate(toSpawn[i].gameObject);
                if (spawnedObj.transform.GetComponent<Collider>() is SphereCollider) {
                    spawnedObj.transform.position = new Vector3(
                        spawns[i].position.x,
                        spawns[i].position.y + (toSpawn[i].transform.localScale.y *
                            (toSpawn[i].transform.GetComponent<SphereCollider>().radius)),
                        spawns[i].position.z);
                    print("Object spawned");
                } else if (spawnedObj.transform.GetComponent<Collider>() is BoxCollider) {
                    spawnedObj.transform.position = new Vector3(
                        spawns[i].position.x,
                        spawns[i].position.y + (toSpawn[i].transform.localScale.y *
                            (toSpawn[i].transform.GetComponent<BoxCollider>().size.y / 2)),
                        spawns[i].position.z);
                    print("Object spawned");
                } else if (spawnedObj.transform.GetComponent<Collider>() is CapsuleCollider) {
                    spawnedObj.transform.position = new Vector3(
                        spawns[i].position.x,
                        spawns[i].position.y + (toSpawn[i].transform.localScale.y *
                            (toSpawn[i].transform.GetComponent<CapsuleCollider>().height / 2)),
                        spawns[i].position.z);
                    print("Object spawned");
                } else {
                    spawnedObj.transform.position = new Vector3(
                        spawns[i].position.x,
                        spawns[i].position.y + (toSpawn[i].transform.localScale.y + 1),
                        spawns[i].position.z);
                    print("Object spawned...probably at the wrong height. Its collider can't yet be handled well, so go yell at"
                        + " Jared to fix it.");
                }
            }
        }
    }

    private bool queueSpawn(int objIndex, int spawnIndex) {

        print("Attemtping to queue spawn of " + spawnables[objIndex].name + " at spawnpoint " + spawnIndex + ". There is "
            + budgetLeft + " budget left before this attempt, and the object has a weight of "
            + spawnables[objIndex].gameObject.GetComponent<Attributes>().weight + ".");

        if (spawnables[objIndex].gameObject.GetComponent<Attributes>().weight <= budgetLeft) {
            if (spawnIndex == -1) { // Index will be -1 if the spawn is random and not forced to be at a specific spawnpoint
                for (int i = 0; i < toSpawn.Count; i++) {
                    if (toSpawn[i] == null) {
                        toSpawn[i] = spawnables[objIndex];
                        budgetLeft -= spawnables[objIndex].gameObject.GetComponent<Attributes>().weight;
                        numSpawned[objIndex]++;
                        spawnsLeft--;
                        return true;
                    }
                }
            } else {
                toSpawn[spawnIndex] = spawnables[objIndex];
                budgetLeft -= spawnables[objIndex].gameObject.GetComponent<Attributes>().weight;
                numSpawned[objIndex]++;
                spawnsLeft--;
                return true;
            }
        }
        return false;
    }

    private bool queueSpawnIfAble(int objIndex) {
        return queueSpawn(objIndex, -1);
    }

    private bool queueForcedSpawnIfAble(int objIndex, int spawnIndex) {
        return queueSpawn(objIndex, spawnIndex);
    }

    private bool queueForcedSpawnIfAble(Transform t, int spawnIndex) {
        for (int i = 0; i < spawnables.Count; i++) {
            if (spawnables[i] == t) {
                Debug.Log("found");
                return queueSpawn(i, spawnIndex);
            }
        }
        return false;
    }

}
