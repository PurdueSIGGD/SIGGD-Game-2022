using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ProceduralGenerator : MonoBehaviour
{
    [SerializeField]
    public int FinalRoomCount, TotalRoomCount;

    [SerializeField]
    private int MinRoomWidth, MinRoomDepth, MaxRoomWidth, MaxRoomDepth, TileSize, MinRoomSeparation;

    [SerializeField]
    private int FloorBudget;

    [SerializeField]
    private bool RoundToTileSize;

    [SerializeField]
    private GameObject floor;

    [SerializeField]
    Material DefaultFloorMaterial, MainRoomMaterial;

    private float inRadius = 10;

    private List<Rect> RoomRects;
    private GameObject RoomParent;

    private List<Rect> GabrielEdges;
    private bool DrawGizmos;

    enum GridPoint
    {
        Empty,
        Room,
        Hallway
    }

    GridPoint[][] grid;

    //TODO: Hallway generation

    // Start is called before the first frame update
    void Awake()
    {
        GenerateDungeon();
        ClearScene();
        DrawFloor();
    }

    //Calls DrawRooms() to visualize the output of the procedural algorithm.
    void Start()
    {
    }

    //Uniformly generates a psuedo-random point in a circle of radius maxRadius.
    public Vector2 getRandomPointInCircle(float maxRadius)
    {
        float radius = Mathf.Sqrt(Random.Range(0.0f, maxRadius));
        float theta = Random.Range(0.0f, 2 * Mathf.PI);

        //Convert from polar coordinates to cartesian
        float pointX = radius * Mathf.Cos(theta);
        float pointY = radius * Mathf.Sin(theta);
        if (RoundToTileSize)
        {
            pointX = Mathf.Round(pointX / TileSize) * TileSize;
            pointY = Mathf.Round(pointY / TileSize) * TileSize;
        }

        return new Vector2(pointX - (MinRoomSeparation / 2), pointY - (MinRoomSeparation / 2));
    }

    public void GenerateDungeon()
    {
        RoomParent = this.gameObject.transform.GetChild(0).gameObject;
        RoomRects = new List<Rect>();
        for (int i = 0; i < TotalRoomCount; i++)
        {
            Vector2 randomPoint = getRandomPointInCircle(inRadius);

            //Debug display of the room points as 3D Spheres.
            //Instantiate(randomObject, new Vector3(randomPoint.x, 0, randomPoint.y), Quaternion.identity);

            //Randomly select room width and depth in range [MinRoom, MaxRoom] inclusively.
            //Width and Depth are both integers.
            int roomWidth = Random.Range(MinRoomWidth, MaxRoomWidth + 1) * TileSize + MinRoomSeparation;
            int roomDepth = Random.Range(MinRoomDepth, MaxRoomDepth + 1) * TileSize + MinRoomSeparation;

            //Initialize the room with a Rect data structure for easy manipulation.
            //The final display will convert the Rects into 3D rooms.
            RoomRects.Add(new Rect(randomPoint.x, randomPoint.y, roomWidth, roomDepth));
        }
        Seperate();
        SortByArea();
        GabrielEdges = GabrielGraph(RoomRects.GetRange(0, FinalRoomCount));
    }

    public void ClearScene()
    {
        if (RoomParent == null)
        {
            RoomParent = this.gameObject.transform.GetChild(0).gameObject;
        }
        for (int i = RoomParent.transform.childCount; i > 0; i--)
        {
            DestroyImmediate(RoomParent.transform.GetChild(0).gameObject);
        }

        DrawGizmos = false;
    }

    public void DrawFloor()
    {
        if (RoomRects == null) GenerateDungeon();

        DrawRooms(RoomRects.GetRange(FinalRoomCount, RoomRects.Count - FinalRoomCount), DefaultFloorMaterial);
        DrawRooms(RoomRects.GetRange(0, FinalRoomCount), MainRoomMaterial);

        DrawGizmos = true;
    }

    //Iterates through all of the rooms pushing any overlapping rooms apart
    //until none of the rooms overlap each other.
    private void Seperate()
    {
        bool RoomsOverlap;
        do
        {
            RoomsOverlap = false;
            for (int current = 0; current < RoomRects.Count; current++)
            {
                for (int other = 0; other < RoomRects.Count; other++)
                {
                    //Don't run if the other room is the current room
                    //or if the two rooms do not overlap with Rect.Overlaps()
                    if (current == other || !RoomRects[current].Overlaps(RoomRects[other])) continue;

                    //Check edge case where two rooms are identical in size and location
                    if (RoomRects[current] == RoomRects[other])
                    {
                        RoomRects.RemoveAt(other);
                        continue;
                    }

                    //Could not directly edit the Rect.position value from the list
                    //so abstracted to base Rects.
                    Rect curRoomRect = RoomRects[current];
                    Rect otherRoomRect = RoomRects[other];

                    //direction has a magnitude of 1 pointing in the direction to move the current room
                    Vector2 direction = (RoomRects[current].center - RoomRects[other].center).normalized;

                    //Move the 'current' and 'other' rooms in opposite directions
                    curRoomRect.position = curRoomRect.position + (direction * TileSize);
                    otherRoomRect.position = otherRoomRect.position + ((-direction) * TileSize);

                    //Reset reference to the rectangle in the RoomRects List<Rect>
                    RoomRects[current] = curRoomRect;
                    RoomRects[other] = otherRoomRect;

                    RoomsOverlap = true;
                }
            }
        } while (RoomsOverlap == true);

        for (int i = 0; i < RoomRects.Count; i++)
        {
            Rect roundedRoom = RoomRects[i];
            roundedRoom.position = new Vector2(Mathf.Ceil(roundedRoom.position.x), 
                                               Mathf.Ceil(roundedRoom.position.y));
            RoomRects[i] = roundedRoom;
        }
    }

    //Visualize the RoomRects List<Rect> as Unity GameObjects
    //Floor is some form of plane representing a room.
    private void DrawRooms(List<Rect> rooms, Material roomColor)
    {
        foreach (Rect rect in rooms)
        {
            GameObject testFloor = Instantiate(floor, new Vector3(rect.center.x, 0, rect.center.y), Quaternion.identity, RoomParent.transform);
            #if UNITY_EDITOR
                testFloor.GetComponent<RoomGenerator>().EditorAwake();
            #endif
            // Divide by 10 because the scale of planes is 10. Can be abstracted as a variable if floor is changed
            float standardRoomSize = 8.0f;
            testFloor.transform.localScale = (new Vector3(rect.width - MinRoomSeparation, standardRoomSize, rect.height - MinRoomSeparation)) / standardRoomSize;
            testFloor.transform.Translate(new Vector3(0.5f, 0, 0.5f));

            // Tell the roomGenerators to generate each room
            RoomGenerator roomGenerator = testFloor.GetComponent<RoomGenerator>();
            roomGenerator.generateObstacles(FloorBudget / rooms.Count);

            if (roomColor != null)
            {
                foreach (Renderer planeRend in testFloor.GetComponentsInChildren<Renderer>())
                {
                    planeRend.material = roomColor;
                }
            }

        }
    }

    private void SortByArea()
    {
        RoomRects.Sort(delegate (Rect x, Rect y)
        {
            float xArea = x.width * x.height - MinRoomSeparation + (MinRoomSeparation * MinRoomSeparation);
            float yArea = y.width * y.height - MinRoomSeparation + (MinRoomSeparation * MinRoomSeparation);
            if (xArea < yArea) return 1;
            else if (xArea > yArea) return -1;
            else return 0;
        });
    }

    private List<Rect> GabrielGraph(List<Rect> rooms)
    {
        List<Rect> FinalEdges = new List<Rect>();

        for (int i = 0; i < rooms.Count - 1; i++) 
        {
            for (int j = i + 1; j < rooms.Count; j++)
            {
                Vector2 mid = (rooms[i].center + rooms[j].center) / 2;
                float radius = Vector2.Distance(mid, rooms[i].center);

                bool isValidEdge = true;
                for (int k = 0; k < rooms.Count; k++)
                {
                    if ((k == i) || (k == j)) continue;

                    if (Vector2.Distance(mid, rooms[k].center) < radius)
                    {
                        isValidEdge = false;
                        break;
                    }
                }

                if (isValidEdge)
                {
                    FinalEdges.Add(rooms[i]);
                    FinalEdges.Add(rooms[j]);
                }
            }
        }
        return FinalEdges;
    }

    /**
     * Returns [(minX, maxX), (minY, maxY)]
     */
    Vector2[] getRoomBounds() {
        // Make default bounds
        float[] minDims = new float[] {RoomRects[0].xMin, RoomRects[0].yMin};
        float[] maxDims = new float[] {RoomRects[0].xMax, RoomRects[0].yMax};
        // Loop over each room
        foreach (Rect room in RoomRects) {
            // Get dimensions for the room
            float[] roomMinDims = new float[] {room.xMin, room.yMin};
            float[] roomMaxDims = new float[] {room.xMin, room.yMin};
            // Test max for x and y
            for (int i = 0; i < roomMaxDims.Length; i++) {
                if (roomMaxDims[i] > maxDims[i]) {
                    maxDims[i] = roomMaxDims[i];
                }
            }
            // Test min for x and y
            for (int i = 0; i < roomMaxDims.Length; i++) {
                if (roomMaxDims[i] > maxDims[i]) {
                    maxDims[i] = roomMaxDims[i];
                }
            }
        }
        return new Vector2[] {new Vector2(minDims[0], maxDims[0]), new Vector2(minDims[1], maxDims[1])};
    }

    void OnDrawGizmos()
    {
        if (GabrielEdges != null && DrawGizmos)
        {
            // Direct connections from room to room
            for (int i = 0; i < GabrielEdges.Count / 2; i++)
            {
                Gizmos.color = Color.blue;
                Vector3 vectA = new Vector3(GabrielEdges[i * 2].center.x, 0, GabrielEdges[i * 2].center.y);
                Vector3 vectB = new Vector3(GabrielEdges[i * 2 + 1].center.x, 0, GabrielEdges[i * 2 + 1].center.y);
                Gizmos.DrawLine(vectA, vectB);

                Gizmos.color = Color.green;
                float deltaX = vectA.x - vectB.x;
                float deltaZ = vectA.z - vectB.z;
                Gizmos.DrawLine(vectA, vectA + Vector3.left * deltaX);
                Gizmos.DrawLine(vectB, vectB + Vector3.forward * deltaZ);
            }
        }
    }
}


#if UNITY_EDITOR
[CustomEditor (typeof(ProceduralGenerator))]
public class TrainingEditor : Editor {
	public override void OnInspectorGUI () {
		ProceduralGenerator progen = (ProceduralGenerator)target;
		if(GUILayout.Button("Generate")){
            progen.GenerateDungeon();
		}
		if(GUILayout.Button("Draw Floor")){
            progen.ClearScene();
            progen.DrawFloor();
		}
		if(GUILayout.Button("Clear Floor")){
            progen.ClearScene();
		}
		DrawDefaultInspector ();
	}
}
#endif
