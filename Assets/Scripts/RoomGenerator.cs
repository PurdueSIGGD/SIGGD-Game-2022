using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    private Transform room;

    // The size of a plane of scale 1
    public const float DEFAULT_ROOM_SIZE = 8.0f;

    private float roomWidth;
    private float roomLength;

    // The unique objects to spawn in the rooms
    public GameObject pillarObject;
    // Start is called before the first frame update
    void Start()
    {
        room = transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Standardizes the size of the door on rescaled rooms
    public void setRoomScale(Vector2 scale) {
        roomWidth = scale.x * DEFAULT_ROOM_SIZE;
        roomLength = scale.y * DEFAULT_ROOM_SIZE;

        Transform[] walls = {room.Find("Right Wall"), room.Find("Left Wall"), room.Find("Front Wall"), room.Find("Back Wall")};
        float[] wallLengths = {roomWidth, roomLength, roomLength, roomLength};
        for (int i = 0; i < 4; i++) {
            fixWall(walls[i], wallLengths[i]);
        }
    }

    private void fixWall(Transform wall, float wallLength) {
        // TODO: Implement
    }

    public void generateObstacles() {
        int pillarNum = Random.Range(1, 5);
        for (int i = 0; i < pillarNum; i++) {
            float x = Random.Range(-roomLength / 2, roomLength / 2);
            float z = Random.Range(-roomWidth / 2, roomWidth / 2);
            Vector3 position = transform.position + new Vector3(x, 0, z);
            GameObject pillar = Instantiate(pillarObject, position, Quaternion.identity);
            pillar.transform.SetParent(gameObject.transform);
        }
    }
}