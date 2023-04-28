using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* --- DEV NOTES ---
-   For now, we're assuming that there will be several set locations in a room where any object can spawn, so long as it isn't too
large for that location. Eventually we can add functionality for spawning probability and constraints, but just assume that any
object can spawn at the points for now.
-   STOPPING POINT: I believe the mins and maxes are working, and there is a safeguard that corrects it if the min is higher than the max. Next, I should focus on
actually finding the spawnpoints in the room and actually placing the objects there before adding any more modifiers.
*/

/// <summary>
/// Creates objects in the room this script is attached to at predefined points in that specific room.
/// </summary>
public class RoomBudgeting : MonoBehaviour {

    private List<GameObject> spawnPoints;
    
    [SerializeField]
    private GameObject[] spawnables;

    [SerializeField]
    private int budget = 0;

    private List<GameObject> toSpawn;

    [SerializeField]
    private GameObject key;

    public void Go(bool keyRoom) {
        if (spawnables == null || spawnables.Length == 0) { return; }

        int tempBudget = budget; // Used for spawning the objects because this instance will be deprecated during that, while the original instance persists

        // Read in the room's spawn points
        Transform pointObject = transform.Find("SpawnPoints");
        DumbSpawner[] genPoints = pointObject.GetComponentsInChildren<DumbSpawner>();
        spawnPoints = new List<GameObject>();
        for (int i = 0; i < genPoints.GetLength(0); i++) {
            spawnPoints.Add(genPoints[i].gameObject);
        }

        // Find lowest budget
        int lowestBudget = 9999;
        for (int i = 0; i < spawnables.Length; i++) {
            GameObject g = spawnables[i].gameObject;
            Attributes attributes = g.GetComponent<Attributes>();
            if (attributes == null)
            {
                continue;
            }
            int w = attributes.weight;
            if (w < lowestBudget) {
                lowestBudget = w;
            }
        }

        // The objects' transforms to be allowed to spawn in the room should be added to the script's list through the serialized field in the editor.
        toSpawn = new List<GameObject>();

        // Case where the room needs to spawn a key
        if (keyRoom) {
            toSpawn.Add(key);
        }

        int tempCount = 0;
        Debug.Log("lowest: " + lowestBudget);
        while (tempBudget >= lowestBudget) {
            // Try to spawn a random item
            int randIndex = (int) Random.Range(0, spawnables.Length);
            GameObject g = spawnables[randIndex].gameObject;
            Attributes attributes = g.GetComponent<Attributes>();
            if (attributes == null)
            {
                continue;
            }
            int w = attributes.weight;
            if (tempBudget >= w) {
                toSpawn.Add(spawnables[randIndex]);
                tempBudget -= w;
            }   
            Debug.Log("woop");
            if (tempCount++ == 100) {
                break;
            }
        }

        string debugText = "With a budget of " + budget + ", spawned these objects: ";
        foreach (GameObject t in toSpawn) {
            Attributes attributes = t.GetComponent<Attributes>();
            int w = 0;
            if (attributes != null)
            {
                w = attributes.weight;
            }
            debugText += t.name + " (" + w + "), ";
        }
        debugText += "with " + tempBudget + " budget left over.";
        Debug.Log(debugText);
        
        spawnObjects(toSpawn);     
    }

    /// <summary>
    /// Spawns the chosen objects at the available spawnpoints, making sure not to spawn them in the floor but instead sitting
    /// on the floor, depending on their collider. (Spawnpoints must be level with the floor)
    /// </summary>
    /// <param name="toSpawn"></param>
    private void spawnObjects(List<GameObject> toSpawn) {
        while (toSpawn.Count > 0) {
            if (spawnPoints.Count == 0) { return; }
            int randIndex = (int) Random.Range(0, spawnPoints.Count);
            GameObject spawnedObj = Instantiate(toSpawn[0].gameObject, spawnPoints[randIndex].transform);
            //Instantiate(toSpawn[0].gameObject, spawnPoints[randIndex].transform.position, Quaternion.identity);

            Transform objTransform = spawnedObj.transform;
            float heightAdjustment = 0.0f;
            Collider collider = objTransform.GetComponent<Collider>();
            if (collider is SphereCollider) {
                heightAdjustment = objTransform.localScale.y * objTransform.GetComponent<SphereCollider>().radius;
            } else if (collider is BoxCollider) {
                heightAdjustment = objTransform.localScale.y * (objTransform.GetComponent<BoxCollider>().size.y / 2);
            } else if (collider is CapsuleCollider) {
                heightAdjustment = objTransform.localScale.y * (objTransform.GetComponent<CapsuleCollider>().height / 2);
            }

            //objTransform.parent = spawnPoints[randIndex].transform;
            objTransform.position = spawnPoints[randIndex].transform.position + Vector3.up * heightAdjustment;
            spawnPoints.RemoveAt(randIndex);
            toSpawn.RemoveAt(0);
        }
    }

    // Assumption: the starting room's key is not set because it should never spawn keys
    public bool IsStartingRoom() {
        return key == null;
    }
}