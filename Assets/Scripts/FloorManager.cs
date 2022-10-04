using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class will generate the the whole floor
public class FloorManager : MonoBehaviour
{
    public GameObject room;
    RoomGenerator[] roomGenerators;
    // Start is called before the first frame update
    void Start()
    {
        int numFloors = GenerateFloor(/* There would normally be stuff in here */);
        roomGenerators = new RoomGenerator[numFloors];
        initFloorGenerators();
    }

    // Update is called once per frame
    void Update()
    {
        // If the player is on a new floor {
        //  GenerateFloor();
        // }
    }

    /*
     * Generates the floor and returns the number of floors generated
     */
    int GenerateFloor(/* list of things to put in the rooms and their frequencies */) {
        // Temporary test
        int positionRange = 50;
        for(int i = 0; i < 5; i++) {
            Vector3 position = new Vector3(Random.Range(-positionRange, positionRange), 0, Random.Range(-positionRange, positionRange));
            Instantiate(room, position, Quaternion.identity);
        }
        return 5; // 
    }

    void initFloorGenerators() {
        for (int i = 0; i < roomGenerators.Length; i++) {
            // Do random stuff
            roomGenerators[i] = new RoomGenerator(1);
        }
    }
}
