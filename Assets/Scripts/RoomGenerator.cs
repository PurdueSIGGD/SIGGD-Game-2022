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


    void Awake() {
        room = transform;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
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