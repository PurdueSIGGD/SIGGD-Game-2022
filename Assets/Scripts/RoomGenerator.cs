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

    private GameObject[] generationPoints;

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
            GameObject point = generationPoints[i];
            GameObject pillar = Instantiate(pillarObject, point.transform.position, Quaternion.identity);
            pillar.transform.SetParent(point.transform);
        }
    }

    private void readGenerationPoints() {
        GenerationPoint[] genPoints = FindObjectsOfType<GenerationPoint>();
        generationPoints = new GameObject[genPoints.Length];
        for(int i = 0; i < generationPoints.Length; i++) {
            generationPoints[i] = genPoints[i].gameObject;
        }
    }
}