using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    private Transform room;
    private Transform floor;

    // The size of a plane of scale 1
    private const int DEFAULT_PLANE_SIZE = 10;

    private float floorWidth;
    private float floorLength;

    // The unique objects to spawn in the rooms
    public GameObject pillarObject;
    // Start is called before the first frame update
    void Start()
    {
        room = transform;
        floor = room.GetChild(0);
        floorWidth = floor.transform.localScale.x * DEFAULT_PLANE_SIZE;
        floorLength = floor.transform.localScale.x * DEFAULT_PLANE_SIZE;

        // objects = new GameObject[obstacleNum];
        generateObstacles();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void generateObstacles() {
        int pillarNum = Random.Range(1, 5);
        for (int i = 0; i < pillarNum; i++) {
            float x = Random.Range(-floorLength / 2, floorLength / 2);
            float z = Random.Range(-floorWidth / 2, floorWidth / 2);
            Vector3 position = transform.position + new Vector3(x, 0, z);
            GameObject pillar = Instantiate(pillarObject, position, Quaternion.identity);
            pillar.transform.SetParent(gameObject.transform);
        }
    }
}