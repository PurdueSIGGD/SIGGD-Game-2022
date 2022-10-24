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

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
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
        Transform pt1 = gameObject.transform.Find("Random Spawn 1");
        Transform pt2 = gameObject.transform.Find("Random Spawn 2");
        Transform pt3 = gameObject.transform.Find("Random Spawn 3");
        Transform pt4 = gameObject.transform.Find("Random Spawn 4");
        return new Transform[GENERATION_POINT_NUM] {pt1, pt2, pt3, pt4};
    }
}