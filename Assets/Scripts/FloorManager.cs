using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class will generate the the whole floor
public class FloorManager : MonoBehaviour
{
    public GameObject squareRoom;

    private const int NUM_ROOMS = 5;

    // Start is called before the first frame update
    void Start()
    {
        GenerateFloor(/* There would normally be stuff in here */);
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
    void GenerateFloor(/* list of things to put in the rooms and their frequencies */) {
        // Temporary test
        int positionRange = 50;
        for(int i = 0; i < NUM_ROOMS; i++) {
            Vector3 position = new Vector3(Random.Range(-positionRange, positionRange), 0, Random.Range(-positionRange, positionRange));
            GameObject room = Instantiate(squareRoom, position, Quaternion.identity);

            // Set up the room
            RoomGenerator roomGenerator = room.GetComponent<RoomGenerator>();
            roomGenerator.generateObstacles(/* budget parameter */);
        }
    }

    void initFloorGenerators() {
        for (int i = 0; i < NUM_ROOMS; i++) {
            // Do random stuff
            
        }
    }
}
