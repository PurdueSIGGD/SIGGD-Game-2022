using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    private Transform room;

    // The size of a plane of scale 1
    private const float DEFAULT_ROOM_SIZE = 8.0f;

    private const int GENERATION_POINT_NUM = 4;

    private float roomWidth;
    private float roomLength;

    private Transform[] generationPoints;

    // The unique objects to spawn in the rooms
    // Just a GameObject right now but could be an array later
    [SerializeField]
    private GameObject pillarObject;

    [SerializeField]
    private int pillarCost;


    void Awake() {
        room = transform;
        generationPoints = getGenerationPoints();
    }

    public void generateObstacles(int budget) {
        // TODO: Incorporate budget when making pillars
        for (int i = 0; i < GENERATION_POINT_NUM; i++) {
            if (Random.Range(0, 2) == 0) { continue; }
            Transform point = generationPoints[i];

            // TODO: Uncomment below line and continue next meeting
            GameObject pillar = Instantiate(pillarObject, point.transform.position, Quaternion.identity);
            pillar.transform.SetParent(point);
        }
    }

    private Transform[] getGenerationPoints() {
        // Get point count
        int roomPointCount = 0;
        string objectName = "Random Spawn " + (roomPointCount + 1);
        while (gameObject.transform.Find(objectName) != null) {
            roomPointCount++;
            objectName = "Random Spawn " + (roomPointCount + 1);
            Debug.Log("In da woop");
        }

        // Now get the points
        Transform[] points = new Transform[roomPointCount];
        for (int i = 1; i <= roomPointCount; i++) {
            Transform pt = gameObject.transform.Find("Random Spawn " + i);
        }
        return points;
    }

    public void EditorAwake() {
        room = transform;
        generationPoints = getGenerationPoints();
    }
}