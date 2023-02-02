using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    private Transform room;

    // The size of a plane of scale 1
    private const float DEFAULT_ROOM_SIZE = 8.0f;

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
        readGenerationPoints();
    }

    public void generateObstacles(int budget) {
        if (generationPoints == null) {
            readGenerationPoints();
        }
        // TODO: Incorporate budget when making pillars
        for (int i = 0; i < generationPoints.Length; i++) {
            if (Random.Range(0, 2) == 0) { continue; }
            if (generationPoints[i] == null) { Debug.Log("genP is null for " + i); }
            Transform point = generationPoints[i];

            // TODO: Uncomment below line and continue next meeting
            if (pillarObject == null) { Debug.Log("pillar is null"); }
            if (point == null) { Debug.Log("point is null"); }
            if (point.transform == null) { Debug.Log("point.transform is null"); }
            if (point.transform.position == null) { Debug.Log("point.transform.position is null"); }
            GameObject pillar = Instantiate(pillarObject, point.transform.position, Quaternion.identity);
            pillar.transform.SetParent(point);
        }
    }

    private void readGenerationPoints() {
        // Get point count
        int roomPointCount = 0;
        string objectName = "Random Spawn " + (roomPointCount + 1);
        while (transform.Find(objectName) != null) {
            roomPointCount++;
            objectName = "Random Spawn " + (roomPointCount + 1);
            Debug.Log("Random gen for " + (roomPointCount + 1));
        }

        // Now get the points
        Transform[] points = new Transform[roomPointCount];
        for (int i = 1; i <= roomPointCount; i++) {
            Transform pt = gameObject.transform.Find("Random Spawn " + i);
        }
        Debug.Log("Total of " + roomPointCount + " points");
        generationPoints = points;
    }
}