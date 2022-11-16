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
    private int MinRoomWidth, MinRoomDepth, MaxRoomWidth, MaxRoomDepth, TileSize;

    [SerializeField]
    private float MinRoomSeparation;

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
    private Room[] FinalRoomPlan;
    private GameObject RoomParent;

    private List<Rect> GabrielEdges;
    private bool DrawGizmos;

    enum GridPoint
    {
        Empty,
        Room,
        Hallway
    }

    public struct Room {
        public GameObject physicalRoom;
        public Rect roomRect;
        public int rotation;
        public int rectPointer;
        int gridX;
        int gridY;
        Vector3[] hallways;

        public Room(RoomScriptableObject roomObj, int rotation, int rectPointer) {
            this.physicalRoom = roomObj.room;
            this.rotation = rotation;
            this.rectPointer = rectPointer;
            gridX = -1;
            gridY = -1;
            roomRect = new Rect();

            hallways = new Vector3[roomObj.hallways.Length];
            Vector3 halfRoom = new Vector3(roomObj.dimensions.x / 2, 0, roomObj.dimensions.z / 2);
            Vector3 rotHalfRoom = new Vector3(roomObj.dimensions.z / 2, 0, roomObj.dimensions.x / 2);
            for (int i = 0; i < hallways.Length; i++) {
                switch(rotation) {
                    case 0:
                        this.hallways[i] = roomObj.hallways[i];
                        break;
                    case 1:
                        this.hallways[i] = roomObj.hallways[i] - halfRoom;
                        this.hallways[i] = new Vector3(hallways[i].z, hallways[i].y, -1 * hallways[i].x);
                        this.hallways[i] += rotHalfRoom;
                        break;
                    case 2:
                        this.hallways[i] = roomObj.hallways[i] - halfRoom;
                        this.hallways[i] = new Vector3(-1 * hallways[i].x, hallways[i].y, -1 * hallways[i].z);
                        this.hallways[i] += halfRoom;
                        break;
                    case 3:
                        this.hallways[i] = roomObj.hallways[i] - halfRoom;
                        this.hallways[i] = new Vector3(-1 * hallways[i].z, hallways[i].y, hallways[i].x);
                        this.hallways[i] += rotHalfRoom;
                        break;
                }
            }
        }

        public void SetRect(Rect room) {
            this.roomRect = room;
        }

        public void SetGridPoint(int xPos, int yPos) {
            if (this.gridX != -1) return;

            this.gridX = xPos;
            this.gridY = yPos;
        }
    }

    [SerializeField]
    RoomScriptableObject[] FloorRooms;


    GridPoint[,] grid;

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
            pointX = Mathf.Round(pointX);
            pointY = Mathf.Round(pointY);
        }

        return new Vector2(pointX, pointY);
    }

    public void GenerateDungeon()
    {
        RoomParent = this.gameObject.transform.GetChild(0).gameObject;
        RoomRects = new List<Rect>();
        FinalRoomPlan = new Room[FinalRoomCount];
        int remainingFinalRooms = FinalRoomCount;
        int remainingNormRooms = TotalRoomCount - FinalRoomCount;
        for (int i = 0; i < TotalRoomCount; i++)
        {
            if (Random.Range(0, remainingNormRooms + remainingFinalRooms) < remainingNormRooms) {
                remainingNormRooms--;
                Vector2 randomPoint = getRandomPointInCircle(inRadius);

                //Debug display of the room points as 3D Spheres.
                //Instantiate(randomObject, new Vector3(randomPoint.x, 0, randomPoint.y), Quaternion.identity);

                //Randomly select room width and depth in range [MinRoom, MaxRoom] inclusively.
                //Width and Depth are both integers.
                float roomWidth = Random.Range(MinRoomWidth, MaxRoomWidth + 1) + MinRoomSeparation;
                float roomDepth = Random.Range(MinRoomDepth, MaxRoomDepth + 1) + MinRoomSeparation;

                //Initialize the room with a Rect data structure for easy manipulation.
                //The final display will convert the Rects into 3D rooms.
                RoomRects.Add(new Rect(randomPoint.x - (roomWidth / 2), randomPoint.y - (roomDepth / 2), roomWidth, roomDepth));
            } else {
                remainingFinalRooms--;
                Vector2 randomPoint = getRandomPointInCircle(inRadius);

                RoomScriptableObject roomType = FloorRooms[Random.Range(0, FloorRooms.Length)];
                int rotation = Random.Range(0, 4);
                float roomWidth = (int) roomType.dimensions.x + MinRoomSeparation;
                float roomDepth = (int) roomType.dimensions.z + MinRoomSeparation;
                if (rotation % 2 == 1) {
                    float temp = roomWidth;
                    roomWidth = roomDepth;
                    roomDepth = temp;
                }

                FinalRoomPlan[remainingFinalRooms] = new Room(roomType, rotation, i);
                RoomRects.Add(new Rect(randomPoint.x - (roomWidth / 2), randomPoint.y - (roomDepth / 2), roomWidth, roomDepth));
            }
        }

        Seperate();
        GabrielEdges = GabrielGraph(FinalRoomPlan);
        GenerateGrid();
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

        DrawRooms(RoomRects.GetRange(0, RoomRects.Count - FinalRoomCount), DefaultFloorMaterial);
        DrawRooms(FinalRoomPlan, MainRoomMaterial);

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
                    curRoomRect.position = curRoomRect.position + direction;
                    otherRoomRect.position = otherRoomRect.position - direction;

                    //Reset reference to the rectangle in the RoomRects List<Rect>
                    RoomRects[current] = curRoomRect;
                    RoomRects[other] = otherRoomRect;

                    RoomsOverlap = true;
                }
            }
        } while (RoomsOverlap);

        for (int i = 0; i < RoomRects.Count; i++)
        {
            Rect roundedRoom = RoomRects[i];
            roundedRoom.position = new Vector2(Mathf.Ceil(roundedRoom.position.x), 
                                               Mathf.Ceil(roundedRoom.position.y));
            // Now discard the extra dimensions for room separation                                   
            roundedRoom.height = roundedRoom.height - MinRoomSeparation;
            roundedRoom.width = roundedRoom.width - MinRoomSeparation;
            RoomRects[i] = roundedRoom;
        }

        
        for (int i = 0; i < FinalRoomCount; i++) {
            FinalRoomPlan[i].SetRect(RoomRects[FinalRoomPlan[i].rectPointer]);
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
            testFloor.transform.localScale = (new Vector3(rect.width, standardRoomSize, rect.height)) / standardRoomSize;
            testFloor.transform.Translate(new Vector3(-0.5f, 0, -0.5f));

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

    private void DrawRooms(Room[] rooms, Material roomColor) {
        foreach (Room finalRoom in rooms)
        {
            GameObject testFloor = Instantiate(finalRoom.physicalRoom, new Vector3(finalRoom.roomRect.center.x, 0, finalRoom.roomRect.center.y), Quaternion.identity, RoomParent.transform);
            #if UNITY_EDITOR
                //testFloor.GetComponent<RoomGenerator>().EditorAwake();
            #endif
            testFloor.transform.Translate(new Vector3(-0.5f, 0, -0.5f));
            testFloor.transform.Rotate(new Vector3(0, 90, 0) * finalRoom.rotation);

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

    private List<Rect> GabrielGraph(Room[] rooms)
    {
        List<Rect> FinalEdges = new List<Rect>();

        for (int i = 0; i < rooms.Length - 1; i++) 
        {
            for (int j = i + 1; j < rooms.Length; j++)
            {
                Vector2 mid = (rooms[i].roomRect.center + rooms[j].roomRect.center) / 2;
                float radius = Vector2.Distance(mid, rooms[i].roomRect.center);

                bool isValidEdge = true;
                for (int k = 0; k < rooms.Length; k++)
                {
                    if ((k == i) || (k == j)) continue;

                    if (Vector2.Distance(mid, rooms[k].roomRect.center) < radius)
                    {
                        isValidEdge = false;
                        break;
                    }
                }

                if (isValidEdge)
                {
                    FinalEdges.Add(rooms[i].roomRect);
                    FinalEdges.Add(rooms[j].roomRect);
                }
            }
        }
        return FinalEdges;
    }

    /**
     * Returns [(minX, maxX), (minY, maxY)]
     */
    Vector2[] GetRoomBounds() {
        // Make default bounds
        float[] minDims = new float[] {RoomRects[0].xMin, RoomRects[0].yMin};
        float[] maxDims = new float[] {RoomRects[0].xMax, RoomRects[0].yMax};
        // Loop over each room
        foreach (Rect room in RoomRects) {
            // Get dimensions for the room
            float[] roomMinDims = new float[] {room.xMin, room.yMin};
            float[] roomMaxDims = new float[] {room.xMax, room.yMax};
            // Test max for x and y
            for (int i = 0; i < roomMaxDims.Length; i++) {
                if (roomMaxDims[i] > maxDims[i]) {
                    maxDims[i] = roomMaxDims[i];
                }
            }
            // Test min for x and y
            for (int i = 0; i < roomMinDims.Length; i++) {
                if (roomMinDims[i] < minDims[i]) {
                    minDims[i] = roomMinDims[i];
                }
            }
        }
        return new Vector2[] {new Vector2(minDims[0], maxDims[0]), new Vector2(minDims[1], maxDims[1])};
    }

    // Rooms are diagonal if the x or y center of a room lies within another room
    private bool AreRoomsDiagonal(Rect room1, Rect room2) {
        Vector2 intPoint1 = new Vector2(room2.center.x, room1.center.y);
        Vector2 intPoint2 = new Vector2(room1.center.x, room2.center.y);
        return room1.Contains(intPoint1) || room1.Contains(intPoint2) || 
               room2.Contains(intPoint1) || room2.Contains(intPoint2);
    }

    void GenerateGrid() {
        // Returns [(minX, maxX), (minY, maxY)]
        // Initialize the grid, with the size proportional to tileSize
        Vector2[] bounds = GetRoomBounds();
        int xSize = (int) (Mathf.Abs(bounds[0][0]) + Mathf.Abs(bounds[0][1]));
        int ySize = (int) (Mathf.Abs(bounds[1][0]) + Mathf.Abs(bounds[1][1]));
        grid = new GridPoint[xSize, ySize];

        // Uh oh: n^3 algo
        float xMin = bounds[0][0];
        float yMin = bounds[1][0];
        for (int i = 0; i < grid.GetLength(0); i++) {
            for (int j = 0; j < grid.GetLength(1); j++) {
                grid[i, j] = GridPoint.Empty;
                for (int k = 0; k < FinalRoomCount; k++) {
                    Vector2 position = new Vector2(xMin + i, yMin + j);
                    if (FinalRoomPlan[k].roomRect.Contains(position)) {
                        grid[i, j] = GridPoint.Room;
                        FinalRoomPlan[k].SetGridPoint(i, j);
                    }
                }
            }
        }

        GenerateHallwayGrid();
    }

    void GenerateHallwayGrid() {
        // Now try to make hallways
        for (int i = 0; i < GabrielEdges.Count / 2; i++) {
            Rect room1 = GabrielEdges[i * 2];
            Rect room2 = GabrielEdges[i * 2 + 1];
            if (AreRoomsDiagonal(room1, room2)) {
                // Try both paths starting with room1
                List<Vector2> path1Points = new List<Vector2>();
                float x = room1.center.x;
                float y = room1.center.y;
                // X direction
                if (room1.center.x > room2.center.x) {
                    while (x >= room2.center.x) {
                        path1Points.Add(new Vector2(x, y));
                        x -= 1;
                    }
                } else {
                    while (x <= room2.center.x) {
                        path1Points.Add(new Vector2(x, y));
                        x += 1;
                    }
                }
                // Y direction
                if (room1.center.y > room2.center.y) {
                    while (y >= room2.center.y) {
                        path1Points.Add(new Vector2(x, y));
                        y -= 1;
                    }
                } else {
                    while (y <= room2.center.y) {
                        path1Points.Add(new Vector2(x, y));
                        y += 1;
                    }
                }

                // TODO: Now try starting from room2

                // Fill in grid with hallway points
                Vector2[] bounds = GetRoomBounds();
                int magX = (int) Mathf.Abs(bounds[0].x);
                int magY = (int) Mathf.Abs(bounds[0].y);
                for (int j = 0; j < path1Points.Count; j++) {
                    Vector2 point = path1Points[j];
                    int gridX = (int) (magX + point.x);
                    int gridY = (int) (magY + point.y);
//                    grid[gridX, gridY] = GridPoint.Hallway;
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        if (DrawGizmos)
        {
            if (GabrielEdges != null) {
                // Direct connections from room to room
                for (int i = 0; i < GabrielEdges.Count / 2; i++)
                {
                    Rect room1 = GabrielEdges[i * 2];
                    Rect room2 = GabrielEdges[i * 2 + 1];
                    Gizmos.color = AreRoomsDiagonal(room1, room2) ? Color.blue : Color.magenta;
                    Vector3 vectA = new Vector3(room1.center.x, 0, room1.center.y);
                    Vector3 vectB = new Vector3(room2.center.x, 0, room2.center.y);
                    Gizmos.DrawLine(vectA, vectB);

                    // Jank hallway generation turned off for now
                    if (false) {
                        Gizmos.color = Color.green;
                        float deltaX = vectA.x - vectB.x;
                        float deltaZ = vectA.z - vectB.z;
                        Gizmos.DrawLine(vectA, vectA + Vector3.left * deltaX);
                        Gizmos.DrawLine(vectB, vectB + Vector3.forward * deltaZ);
                    }
                }
            }
            // Can be changed like on/off switch
            if (true) {
                Vector2[] bounds = GetRoomBounds();
                float xMin = bounds[0][0];
                float yMin = bounds[1][0];
                for (int i = 0; i < grid.GetLength(0); i++) {
                    for (int j = 0; j < grid.GetLength(1); j++) {
                        Gizmos.color = getGridColor(grid[i, j]);
                        Gizmos.DrawSphere(new Vector3(xMin + i, 0, yMin + j), 0.3f);
                    }
                }
            }
        }
    }

    Color getGridColor(GridPoint point) {
        switch(point) {
            case GridPoint.Empty:
                return Color.blue;
            case GridPoint.Hallway:
                return Color.green;
            case GridPoint.Room:
                return Color.red;
        }
        return Color.black;
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
