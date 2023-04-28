using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

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
    private GameObject HallwayPrefab, floor;

    [SerializeField]
    Material DefaultFloorMaterial, MainRoomMaterial;

    [SerializeField]
    private GameObject blockerPlane;

    private float scaleMultiplier = 0.85f;

    private float inRadius = 10;

    private List<Rect> RoomRects;
    private Room[] FinalRoomPlan;
    private GameObject RoomParent;

    private List<int[]> GabrielEdges;
    [SerializeField]
    private bool DoDebug = false;
    private bool DrawGizmos;
    private int gridPadding = 3;

    private PatrolPoint[] patrolPoints;

    [System.Flags]
    public enum GridPoint
    {
        Empty = 1,
        Room = 2,
        Hallway = 4,
        Doorway = 8
    }

    public struct cell {
        // Row and Column index of its parent
        // Note that 0 <= i <= ROW-1 & 0 <= j <= COL-1
        public int parent_i, parent_j;
        // f = g + h
        public double f, g, h;

        public void setValues(int pi, int pj, double f, double g, double h) {
            parent_i = pi;
            parent_j = pj;
            this.f = f;
            this.g = g;
            this.h = h;
        }
    }

    public struct Room {
        public GameObject physicalRoom;
        public Rect roomRect;
        public int rotation;
        public int rectPointer;
        public int gridX;
        public int gridY;
        Vector3[] hallways;

        public int yOffset;

        public Room(RoomScriptableObject roomObj, int rotation, int rectPointer, float scaleMultiplier) {
            this.physicalRoom = roomObj.room;
            this.rotation = rotation;
            this.rectPointer = rectPointer;
            gridX = -1;
            gridY = -1;
            roomRect = new Rect();

            this.yOffset = Mathf.RoundToInt(roomObj.hallways[0].y);

            hallways = new Vector3[roomObj.hallways.Length];
            // Use halfRoom to center the room around a new local origin. rotHalfRoom is used to assist with the off angles.
            Vector3 halfRoom = new Vector3(roomObj.dimensions.x / 2, 0, roomObj.dimensions.z / 2);
            Vector3 rotHalfRoom = new Vector3(roomObj.dimensions.z / 2, 0, roomObj.dimensions.x / 2);
            // Depending on the set rotation set in the constructor, use basics of rotation matrices to rotate the hallways
            // to match the desired ending location. Rotation of 0 is the identity transformation.
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

        public void SetGridPoint(int xPos, int yPos, GridPoint[,] grid) {
            if (this.gridX != -1) return;

            this.gridX = xPos;
            this.gridY = yPos;

            for (int i = 0; i < hallways.Length; i++) {
                Vector3 hallPoint = hallways[i];
                grid[xPos + Mathf.RoundToInt(hallPoint.x), yPos + Mathf.RoundToInt(hallPoint.z)] = GridPoint.Doorway;
            }
        }

        public int[][] ClosestHallways(Room other) {
            int[] localClosestHallway = new int[2];
            int[] remoteClosestHallway = new int[2];
            float distance = 10000;
            for (int i = 0; i < this.hallways.Length; i++) {
                float tempDist = Vector3.Distance(this.hallways[i] + new Vector3(gridX, 0, gridY), other.roomRect.center);
                if (tempDist < distance) {
                    distance = tempDist;
                    localClosestHallway = new int[]{Mathf.RoundToInt(this.hallways[i].x + gridX), Mathf.RoundToInt(this.hallways[i].z + gridY)};
                }
            }
            distance = 10000;
            for (int i = 0; i < other.hallways.Length; i++) {
                float tempDist = Vector3.Distance(other.hallways[i] + new Vector3(other.gridX, 0, other.gridY), this.roomRect.center);
                if (tempDist < distance) {
                    distance = tempDist;
                    remoteClosestHallway = new int[]{Mathf.RoundToInt(other.hallways[i].x + other.gridX), Mathf.RoundToInt(other.hallways[i].z + other.gridY)};
                }
            }

            if (localClosestHallway[0] < 0) localClosestHallway[0] = 0;
            if (localClosestHallway[1] < 0) localClosestHallway[1] = 0;
            if (remoteClosestHallway[0] < 0) remoteClosestHallway[0] = 0;
            if (remoteClosestHallway[1] < 0) remoteClosestHallway[1] = 0;
            return new int[][]{localClosestHallway, remoteClosestHallway};
        }

        public Vector3[] getHallways() {
            return hallways;
        }
    }

    [SerializeField]
    private RoomScriptableObject[] FloorRooms;

    [SerializeField]
    private RoomScriptableObject StartRoom;
    private bool CreatedStart;
    [SerializeField]
    private RoomScriptableObject EndRoom;
    private bool CreatedEnd;


    public GridPoint[,] grid;


    // Start is called before the first frame update
    void Awake()
    {
        //Generate layout
        GenerateDungeon();

        //remove any prior objects
        ClearScene();

        //spawn the room
        DrawFloor();

        //Block off open doors
        fixOpenDoors();

        //Make the end room have correct number of doors
        foreach (RemoveIfDoorNear rm in FindObjectsOfType<RemoveIfDoorNear>())
        {
            rm.ClearWrongDoors();
        }
        
        //sets the player to be at the spawn point
        PlayerSpawnPoint psp = FindObjectOfType<PlayerSpawnPoint>();
        psp.spawn();

        //builds navigation
        GetComponent<BakeLevelNav>().BuildNavigation();

        //Records all of the patrol points
        SetPatrolPoints();

        //Debug.LogError("Before spawn items");

        //Randomly spawns items and enemies
        itemSetup();

        //Debug.LogError("Before Wake Enemies items");

        //Start the enemy patrols
        WakeEnemies();

        //Debug.LogError("After Wake Enemies items");
    }

    private void itemSetup()
    {
        // Make sure we select a room which isn't the starting room
        RoomBudgeting[] rb = FindObjectsOfType<RoomBudgeting>();
        int randIndex;
        do {
            randIndex = (int) Random.Range(0, rb.GetLength(0));
        } while (rb[randIndex].IsStartingRoom());

        int counter = 0;
        foreach (RoomBudgeting budgeting in FindObjectsOfType<RoomBudgeting>()) {
            if (budgeting == null) { continue; }
            
            budgeting.Go(randIndex == counter);
            budgeting.EnemyGo();
            counter++;
        }
        //Debug.LogError("Before subscribe");
        FindObjectOfType<InventorySystem>().SubscribeToItemsInScene();
    }

    private void SetPatrolPoints()
    {
        patrolPoints = GetComponentsInChildren<PatrolPoint>();
    }

    private void WakeEnemies()
    {
        foreach (patrolManager pm in FindObjectsOfType<patrolManager>())
        {
            pm.WakeEnemy(patrolPoints);
            CustomEvent.Trigger(pm.gameObject, "wakeUp");
        }
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

        InitializeRooms();
        Seperate();
        GabrielEdges = GabrielGraph(FinalRoomPlan);
        GenerateGrid();
    }

    public void InitializeRooms()
    {
        RoomRects = new List<Rect>();
        FinalRoomPlan = new Room[FinalRoomCount];
        int remainingFinalRooms = FinalRoomCount;
        int remainingNormRooms = TotalRoomCount - FinalRoomCount;
        bool spawnedStart = false;
        bool spawnedLast = false;
        for (int i = 0; i < TotalRoomCount; i++)
        {
            if (Random.Range(0, remainingNormRooms + remainingFinalRooms) < remainingNormRooms)
            {
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
            }
            else
            {
                remainingFinalRooms--;
                Vector2 randomPoint = getRandomPointInCircle(inRadius);

                RoomScriptableObject roomType;
                if (!spawnedStart)
                {
                    roomType = StartRoom;
                    spawnedStart = true;
                } else if (!spawnedLast)
                {
                    roomType = EndRoom;
                    spawnedLast = true;
                } else
                {
                    roomType = FloorRooms[Random.Range(0, FloorRooms.Length)];
                }
                int rotation = Random.Range(0, 4);
                float roomWidth = (int)roomType.dimensions.x + MinRoomSeparation;
                float roomDepth = (int)roomType.dimensions.z + MinRoomSeparation;
                if (rotation % 2 == 1)
                {
                    float temp = roomWidth;
                    roomWidth = roomDepth;
                    roomDepth = temp;
                }

                FinalRoomPlan[remainingFinalRooms] = new Room(roomType, rotation, i, scaleMultiplier);
                RoomRects.Add(new Rect(randomPoint.x - (roomWidth / 2), randomPoint.y - (roomDepth / 2), roomWidth + 0.1f, roomDepth + 0.1f));
            }
        }
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

        //Draws the temporary rooms. Unneeded in final design,
        //and currently does not work due to RoomRects randomization changes.
        //DrawRooms(RoomRects.GetRange(0, RoomRects.Count - FinalRoomCount), DefaultFloorMaterial);
        DrawRooms(FinalRoomPlan);
        DrawHallways();

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
    // This function is currently unused after the shift to using arrays to represent the rooms.
    /* private void DrawRooms(List<Rect> rooms, Material roomColor)
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
            // testFloor.transform.Translate(new Vector3(-0.5f, 0, -0.5f));

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
    } */

    private void DrawRooms(Room[] rooms) {
        foreach (Room finalRoom in rooms)
        {
            Vector3 inPos = new Vector3(Mathf.RoundToInt(finalRoom.roomRect.center.x), finalRoom.yOffset, Mathf.RoundToInt(finalRoom.roomRect.center.y));
            //GameObject testFloor = Instantiate(finalRoom.physicalRoom, inPos, Quaternion.identity, RoomParent.transform);
            GameObject testFloor = Instantiate(finalRoom.physicalRoom, inPos * scaleMultiplier, Quaternion.identity, RoomParent.transform);
            #if UNITY_EDITOR
                //testFloor.GetComponent<RoomGenerator>().EditorAwake();
            #endif

            testFloor.transform.Rotate(new Vector3(0, 90, 0) * finalRoom.rotation);
        }
    }

    // Checks points adjacent to the input point for hallways or doorways.
    // Returns the number of adjacent hallways and doorways.
    private int HallwayDFSHelper(int[] point, Stack<int[]> dfsStack, ref bool isStraight) {
        int count = 0;
        bool isVertical = false;
        isStraight = true;

        if ((GridPoint.Hallway | GridPoint.Doorway).HasFlag(grid[point[0] + 1, point[1]])) {
            dfsStack.Push(new int[]{point[0] + 1, point[1]});
            count++;
            isVertical = true;
        }
        if ((GridPoint.Hallway | GridPoint.Doorway).HasFlag(grid[point[0] - 1, point[1]])) {
            dfsStack.Push(new int[]{point[0] - 1, point[1]});
            count++;
            if (isVertical == false) isStraight = false;
            isVertical = true;
        }
        if ((GridPoint.Hallway | GridPoint.Doorway).HasFlag(grid[point[0], point[1] + 1])) {
            dfsStack.Push(new int[]{point[0], point[1] + 1});
            count++;
            if (isVertical == true) isStraight = false;
        }
        if ((GridPoint.Hallway | GridPoint.Doorway).HasFlag(grid[point[0], point[1] - 1])) {
            dfsStack.Push(new int[]{point[0], point[1] - 1});
            count++;
            if (isVertical == true) isStraight = false;
        }

        if (count != 2) isStraight = false;

        return count;
    }

    private void DrawHallways()
    {
        Vector2[] GridBounds = GetRoomBounds();
        Vector3 GridOffset = new Vector3(GridBounds[0][0], 0, GridBounds[1][0]);
        Stack<int[]> dfsStack = new Stack<int[]>();
        bool[,] visited = new bool[grid.GetLength(0), grid.GetLength(1)];

        // If hallways start missing make sure that FinalRoomCount is equal to the number of rooms in the scene.
        // Goal: Iterate through all of the hallway points running depth first search.
        // In the end the long straight paths will be one object each joined with some form of a T or L corridor.
        // TODO: Modify the DFS algorithm to easily create long hallway objects by continually checking if the
        // points perpendicular to the long hallway exist as a hallway branch or a doorway. When these branches 
        // are found, add the intersection point (T, L, or X) to the stack and break out.
        foreach (Room finalRoom in FinalRoomPlan) {
            bool isStraight = false;
            foreach (Vector3 localDoorway in finalRoom.getHallways()) {
                int[] gridDoorwayPos = new int[]{finalRoom.gridX + Mathf.RoundToInt(localDoorway.x), finalRoom.gridY + Mathf.RoundToInt(localDoorway.z)};
                if (HallwayDFSHelper(gridDoorwayPos, dfsStack, ref isStraight) == 0) {
                    // There are no hallways adjacent to this doorway.
                    // TODO: Close this doorway.
                }
                
            }
            //Debug.Log(dfsStack.Count);

            int hallwayLength = 0;
            while (dfsStack.Count != 0) {
                int[] currentPoint = dfsStack.Pop();
                if (grid[currentPoint[0], currentPoint[1]] != GridPoint.Hallway) continue;
                if (visited[currentPoint[0], currentPoint[1]] == true) continue;
                visited[currentPoint[0], currentPoint[1]] = true;

                // Check if the current hallway point is part of a long hallway or a collision point.
                // If it is a collision point and there was a long hallway before hand, instantiate the hallway.
                int adjCount = HallwayDFSHelper(currentPoint, dfsStack, ref isStraight);
                if (isStraight == true) {
                    hallwayLength++;
                    //Debug.Log(hallwayLength);
                } else {
                    hallwayLength = 0;
                }
            }
        }


        for (int i = 1; i < grid.GetLength(0) - 1; i++)
        {
            for (int j = 1; j < grid.GetLength(1) - 1; j++)
            {
                if (grid[i, j] != GridPoint.Hallway) continue;
                Vector3 pos = new Vector3(i, 0, j) + GridOffset;
                //GameObject CurrentHallway = Instantiate(HallwayPrefab, pos, Quaternion.identity, RoomParent.transform);
                GameObject CurrentHallway = Instantiate(HallwayPrefab, pos * scaleMultiplier, Quaternion.identity, RoomParent.transform);
                Vector3 ParentScale = CurrentHallway.transform.localScale;
                bool[] isSideDestroyed = new bool[4];
                // Left
                if ((GridPoint.Hallway | GridPoint.Doorway).HasFlag(grid[i - 1, j])) {
                    if (grid[i - 1, j] == GridPoint.Doorway) {
                        DestroyImmediate(CurrentHallway.transform.Find("Left/Bottom/Wall").gameObject);
                        DestroyImmediate(CurrentHallway.transform.Find("Left/Top/Wall").gameObject);
                    } else {
                        DestroyImmediate(CurrentHallway.transform.Find("Left").gameObject);
                    }
                    if ((GridPoint.Hallway | GridPoint.Doorway).HasFlag(grid[i, j + 1])) {
                        DestroyImmediate(CurrentHallway.transform.Find("TopLeft/Top").gameObject);
                        DestroyImmediate(CurrentHallway.transform.Find("TopLeft/Left").gameObject);
                    } else if (grid[i - 1, j] == GridPoint.Doorway) {
                        DestroyImmediate(CurrentHallway.transform.Find("TopLeft/Left").gameObject);
                    } else {
                        DestroyImmediate(CurrentHallway.transform.Find("TopLeft").gameObject);
                    }
                    if ((GridPoint.Hallway | GridPoint.Doorway).HasFlag(grid[i, j - 1])) {
                        DestroyImmediate(CurrentHallway.transform.Find("BottomLeft/Bottom").gameObject);
                        DestroyImmediate(CurrentHallway.transform.Find("BottomLeft/Left").gameObject);
                    } else if (grid[i - 1, j] == GridPoint.Doorway) {
                        DestroyImmediate(CurrentHallway.transform.Find("BottomLeft/Left").gameObject);
                    } else {
                        DestroyImmediate(CurrentHallway.transform.Find("BottomLeft").gameObject);
                    }
                    isSideDestroyed[0] = true;
                }
                // Right
                if ((GridPoint.Hallway | GridPoint.Doorway).HasFlag(grid[i + 1, j])) {
                    if (grid[i + 1, j] == GridPoint.Doorway) {
                        DestroyImmediate(CurrentHallway.transform.Find("Right/Bottom/Wall").gameObject);
                        DestroyImmediate(CurrentHallway.transform.Find("Right/Top/Wall").gameObject);
                    } else {
                        DestroyImmediate(CurrentHallway.transform.Find("Right").gameObject);
                    }
                    if ((GridPoint.Hallway | GridPoint.Doorway).HasFlag(grid[i, j + 1])) {
                        DestroyImmediate(CurrentHallway.transform.Find("TopRight/Top").gameObject);
                        DestroyImmediate(CurrentHallway.transform.Find("TopRight/Right").gameObject);
                    } else if (grid[i + 1, j] == GridPoint.Doorway) {
                        DestroyImmediate(CurrentHallway.transform.Find("TopRight/Right").gameObject);
                    } else {
                        DestroyImmediate(CurrentHallway.transform.Find("TopRight").gameObject);
                    }
                    if ((GridPoint.Hallway | GridPoint.Doorway).HasFlag(grid[i, j - 1])) {
                        DestroyImmediate(CurrentHallway.transform.Find("BottomRight/Bottom").gameObject);
                        DestroyImmediate(CurrentHallway.transform.Find("BottomRight/Right").gameObject);
                    } else if (grid[i + 1, j] == GridPoint.Doorway) {
                        DestroyImmediate(CurrentHallway.transform.Find("BottomRight/Right").gameObject);
                    } else {
                        DestroyImmediate(CurrentHallway.transform.Find("BottomRight").gameObject);
                    }
                    isSideDestroyed[2] = true;
                }
                // Top
                if ((GridPoint.Hallway | GridPoint.Doorway).HasFlag(grid[i, j + 1])) {
                    if (grid[i, j + 1] == GridPoint.Doorway) {
                        DestroyImmediate(CurrentHallway.transform.Find("Top/Bottom/Wall").gameObject);
                        DestroyImmediate(CurrentHallway.transform.Find("Top/Top/Wall").gameObject);
                        if (!(GridPoint.Hallway | GridPoint.Doorway).HasFlag(grid[i + 1, j])) {
                            DestroyImmediate(CurrentHallway.transform.Find("TopRight/Top").gameObject);
                        }
                        if (!(GridPoint.Hallway | GridPoint.Doorway).HasFlag(grid[i - 1, j])) {
                            DestroyImmediate(CurrentHallway.transform.Find("TopLeft/Top").gameObject);
                        }
                    } else {
                        DestroyImmediate(CurrentHallway.transform.Find("Top").gameObject);
                        if (!isSideDestroyed[0]) {
                            DestroyImmediate(CurrentHallway.transform.Find("TopLeft").gameObject);
                        }
                        if (!isSideDestroyed[2]) {
                            DestroyImmediate(CurrentHallway.transform.Find("TopRight").gameObject);
                        }
                    }
                    isSideDestroyed[1] = true;
                }
                // Bottom
                if ((GridPoint.Hallway | GridPoint.Doorway).HasFlag(grid[i, j - 1])) {
                    if (grid[i, j - 1] == GridPoint.Doorway) {
                        DestroyImmediate(CurrentHallway.transform.Find("Bottom/Bottom/Wall").gameObject);
                        DestroyImmediate(CurrentHallway.transform.Find("Bottom/Top/Wall").gameObject);
                        if (!(GridPoint.Hallway | GridPoint.Doorway).HasFlag(grid[i + 1, j])) {
                            DestroyImmediate(CurrentHallway.transform.Find("BottomRight/Bottom").gameObject);
                        }
                        if (!(GridPoint.Hallway | GridPoint.Doorway).HasFlag(grid[i - 1, j])) {
                            DestroyImmediate(CurrentHallway.transform.Find("BottomLeft/Bottom").gameObject);
                        }
                    } else {
                        DestroyImmediate(CurrentHallway.transform.Find("Bottom").gameObject);
                        if (!isSideDestroyed[0]) {
                            DestroyImmediate(CurrentHallway.transform.Find("BottomLeft").gameObject);
                        }
                        if (!isSideDestroyed[2]) {
                            DestroyImmediate(CurrentHallway.transform.Find("BottomRight").gameObject);
                        }
                    }
                    isSideDestroyed[3] = true;
                }
                // TopLeft
                if ((GridPoint.Hallway | GridPoint.Doorway).HasFlag(grid[i - 1, j + 1])) {
                    if (!isSideDestroyed[1]) {
                        DestroyImmediate(CurrentHallway.transform.Find("Top/Bottom").gameObject);
                    }
                    if (!isSideDestroyed[0]) {
                        DestroyImmediate(CurrentHallway.transform.Find("Left/Top").gameObject);
                    }
                }
                // TopRight
                if ((GridPoint.Hallway | GridPoint.Doorway).HasFlag(grid[i + 1, j + 1])) {
                    if (!isSideDestroyed[1]) {
                        DestroyImmediate(CurrentHallway.transform.Find("Top/Top").gameObject);
                    }
                    if (!isSideDestroyed[2]) {
                        DestroyImmediate(CurrentHallway.transform.Find("Right/Bottom").gameObject);
                    }
                }
                // BottomRight
                if ((GridPoint.Hallway | GridPoint.Doorway).HasFlag(grid[i + 1, j - 1])) {
                    if (!isSideDestroyed[3]) {
                        DestroyImmediate(CurrentHallway.transform.Find("Bottom/Bottom").gameObject);
                    }
                    if (!isSideDestroyed[2]) {
                        DestroyImmediate(CurrentHallway.transform.Find("Right/Top").gameObject);
                    }
                }
                // BottomLeft
                if ((GridPoint.Hallway | GridPoint.Doorway).HasFlag(grid[i - 1, j - 1])) {
                    if (!isSideDestroyed[3]) {
                        DestroyImmediate(CurrentHallway.transform.Find("Bottom/Top").gameObject);
                    }
                    if (!isSideDestroyed[0]) {
                        DestroyImmediate(CurrentHallway.transform.Find("Left/Bottom").gameObject);
                    }
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

    private List<int[]> GabrielGraph(Room[] rooms)
    {
        List<int[]> FinalEdges = new List<int[]>();

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
                    FinalEdges.Add(new int[]{ i, j });
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
        float[] minDims = new float[] {FinalRoomPlan[0].roomRect.xMin, FinalRoomPlan[0].roomRect.yMin};
        float[] maxDims = new float[] {FinalRoomPlan[0].roomRect.xMax, FinalRoomPlan[0].roomRect.yMax};
        // Loop over each room
        foreach (Room room in FinalRoomPlan) {
            // Get dimensions for the room
            float[] roomMinDims = new float[] {room.roomRect.xMin, room.roomRect.yMin};
            float[] roomMaxDims = new float[] {room.roomRect.xMax, room.roomRect.yMax};
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
        return new Vector2[] {new Vector2(minDims[0] - gridPadding, maxDims[0] + gridPadding),
                              new Vector2(minDims[1] - gridPadding, maxDims[1] + gridPadding)};
    }

    void GenerateGrid() {
        // Returns [(minX, maxX), (minY, maxY)]
        // Initialize the grid, with the size proportional to tileSize
        Vector2[] bounds = GetRoomBounds();
        int ySize = (int) (Mathf.Abs(bounds[1][0] - bounds[1][1]));
        int xSize = (int) (Mathf.Abs(bounds[0][0] - bounds[0][1]));
        grid = new GridPoint[xSize, ySize];

        // Uh oh: n^3 algo
        float xMin = bounds[0][0];
        float yMin = bounds[1][0];
        for (int i = 0; i < grid.GetLength(0); i++) {
            for (int j = 0; j < grid.GetLength(1); j++) {
                if (grid[i, j] == GridPoint.Doorway) continue;
                grid[i, j] = GridPoint.Empty;
                for (int k = 0; k < FinalRoomCount; k++) {
                    Vector2 position = new Vector2(xMin + i, yMin + j);
                    if (FinalRoomPlan[k].roomRect.Contains(position)) {
                        grid[i, j] = GridPoint.Room;
                        FinalRoomPlan[k].SetGridPoint(i, j, grid);
                    }
                }
            }
        }

        GenerateHallwayGrid();
    }

    void GenerateHallwayGrid() {

        // aStarSearch(new int[][] { new int[] {50,75}, new int[] {0,0} });
        // aStarSearch(FinalRoomPlan[GabrielEdges[0][0]].ClosestHallways(FinalRoomPlan[GabrielEdges[0][1]]));
        // Now try to make hallways

        for (int i = 0; i < GabrielEdges.Count; i++) {
            // Run A* with the start and end points
            aStarSearch(FinalRoomPlan[GabrielEdges[i][0]].ClosestHallways(FinalRoomPlan[GabrielEdges[i][1]]));
        }
    }

    void aStarSearch(int[][] points) {
        bool[,] closedList = new bool[grid.GetLength(0), grid.GetLength(1)];
        cell[,] cellDetails = new cell[grid.GetLength(0), grid.GetLength(1)];
        int i, j;
        for (i = 0; i < grid.GetLength(0); i++) {
            for (j = 0; j < grid.GetLength(1); j++) {
                cellDetails[i, j].setValues(-1, -1, float.MaxValue, float.MaxValue, float.MaxValue);
            }
        }

        i = points[0][0];
        j = points[0][1];

        cellDetails[i, j].setValues(i, j, 0.0, 0.0, 0.0);

        SortedDictionary<double, List<int[]>> openList = new SortedDictionary<double, List<int[]>>();
        // Put the starting cell on the open list and set its
        // 'f' as 0
        openList.Add(0.0, new List<int[]>{new int[]{i, j}});

        bool foundDest = false;

        while (openList.Count > 0) {
            SortedDictionary<double, List<int[]>>.Enumerator dictEnumerator= openList.GetEnumerator();

            if (dictEnumerator.MoveNext() == false) {
                Debug.Log("Failed to Enumerate");
                return;
            }

            KeyValuePair<double, List<int[]>> p = dictEnumerator.Current;

            // Add this vertex to the closed list
            i = p.Value[0][0];
            j = p.Value[0][1];
            closedList[i, j] = true;
            
            openList[dictEnumerator.Current.Key].RemoveAt(0);
            if (openList[dictEnumerator.Current.Key].Count == 0) {
                openList.Remove(dictEnumerator.Current.Key);
            }

            // To store the 'g', 'h' and 'f' of the 8 successors
            double gNew, hNew, fNew;

            // Used to loop over north, south, east, and west surrounding points
            int[] xIndices = new int[] {i - 1, i + 1, i, i};
            int[] yIndices = new int[] {j, j, j - 1, j + 1};

            for (int k = 0; k < xIndices.Length; k++) {
                int x = xIndices[k];
                int y = yIndices[k];
                if (isValid(x, y)) {
                    // If the destination cell is the same as the
                    // current successor
                    if (isDestination(x, y, points[1])) {
                        // Set the Parent of the destination cell
                        cellDetails[x, y].parent_i = i;
                        cellDetails[x, y].parent_j = j;
                        tracePath(cellDetails, points);
                        foundDest = true;
                        return;
                    } else if (!closedList[x, y] && isUnBlocked(x, y)) {
                        if (grid[i, j] == GridPoint.Hallway) {
                            gNew = cellDetails[i, j].g + 0.5f;
                        } else {
                            gNew = cellDetails[i, j].g + 2.0f;
                        }
                        hNew = calculateHValue(x, y, points[1]);
                        fNew = gNew + hNew;
        
                        // If it isn’t on the open list, add it to 
                        // the open list. Make the current square
                        // the parent of this square. Record the
                        // f, g, and h costs of the square cell
                        //                OR
                        // If it is on the open list already, check
                        // to see if this path to that square is
                        // better, using 'f' cost as the measure.
                        if (cellDetails[x, y].f == float.MaxValue
                            || cellDetails[x, y].f > fNew) {
                            if (openList.ContainsKey(fNew)) {
                                openList[fNew].Add(new int[]{x,y});
                            } else {
                                openList.Add(fNew, new List<int []>{new int[]{x,y}});
                            }
                            // Update the details of this cell
                            cellDetails[x, y].setValues(i, j, fNew, gNew, hNew);
                        }
                    }
                }
            }
        }
        if (!foundDest) Debug.Log("A* Failed to Find Destination");
        return;
    }

    // A Utility Function to check whether given cell (row, col)
    // is a valid cell or not.
    bool isValid(int row, int col)
    {
        // Returns true if row number and column number
        // is in range
        return (row >= 0) && (row < grid.GetLength(0)) && (col >= 0)
            && (col < grid.GetLength(1));
    }
    
    // A Utility Function to check whether the given cell is
    // blocked or not
    bool isUnBlocked(int row, int col)
    {
        // Returns true if the cell is not blocked else false
        if (grid[row, col] == GridPoint.Room)
            return (false);
        else
            return (true);
    }
    
    // A Utility Function to check whether destination cell has
    // been reached or not
    bool isDestination(int row, int col, int[] dest)
    {
        return row == dest[0] && col == dest[1];
    }
    
    // A Utility Function to calculate the 'h' heuristics.
    double calculateHValue(int row, int col, int[] dest)
    {
        // Return using the distance formula
        // return (row - dest[0]) * (row - dest[0]) + (col - dest[1]) * (col - dest[1]);
        return Mathf.Abs(row - dest[0]) + Mathf.Abs(col - dest[1]);
    }
    
    // A Utility Function to trace the path from the source
    // to destination
    void tracePath(cell[,] cellDetails, int[][] points)
    {
        int row = points[1][0];
        int col = points[1][1];
    
        Stack<int[]> Path = new Stack<int[]>();
    
        while (!isDestination(row, col, points[0])) {
            // Path.Push(new int[]{row, col});
            int temp_row = cellDetails[row, col].parent_i;
            int temp_col = cellDetails[row, col].parent_j;
            if (grid[row, col] != GridPoint.Doorway) {
                grid[row, col] = GridPoint.Hallway;
            }
            row = temp_row;
            col = temp_col;
        }
    }

    void OnDrawGizmos()
    {
        if (DrawGizmos && DoDebug)
        {
            if (GabrielEdges != null) {
                // Direct connections from room to room
                for (int i = 0; i < GabrielEdges.Count / 2; i++)
                {
                    Rect room1 = FinalRoomPlan[GabrielEdges[i][0]].roomRect;
                    Rect room2 = FinalRoomPlan[GabrielEdges[i][1]].roomRect;
                    Gizmos.color = Color.blue;
                    Vector3 vectA = new Vector3(room1.center.x, 0, room1.center.y) * scaleMultiplier;
                    Vector3 vectB = new Vector3(room2.center.x, 0, room2.center.y) * scaleMultiplier;
                    Gizmos.DrawLine(vectA, vectB);
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
                        Gizmos.DrawSphere(new Vector3(xMin + i, 0, yMin + j) * scaleMultiplier, 0.3f);
                    }
                }
            }
        }
    }

    private void fixOpenDoors() {
        for (int i = 0; i < FinalRoomPlan.Length; i++) {    //For each room

            Vector3[] roomHallways = FinalRoomPlan[i].getHallways();
            for (int j = 0; j < roomHallways.Length; j++) {     //For each room's door
                
                //Both are relative to the center of the room
                int x = Mathf.RoundToInt(roomHallways[j].x);
                int y = Mathf.RoundToInt(roomHallways[j].z);
                int[] xIndices = new int[] {x - 1, x + 1, x, x};
                int[] yIndices = new int[] {y, y, y - 1, y + 1};
                Vector3 hallwayPoint = Vector3.positiveInfinity;

                // Check to see if there's a surrounding hallway
                for (int k = 0; k < xIndices.Length; k++) {     //For adjacent points
                    int mapx = xIndices[k] + FinalRoomPlan[i].gridX;
                    int mapy = yIndices[k] + FinalRoomPlan[i].gridY;
                    if (isValid(mapx, mapy)) {    //If position exists
                        if (grid[mapx, mapy] == GridPoint.Empty) {  //If point is empty, must be filled
                            hallwayPoint = new Vector3(xIndices[k], 0, yIndices[k]);
                            break;
                        }
                    }
                }

                // If not, we need to fill the hole
                if (hallwayPoint.magnitude != float.PositiveInfinity) {
                    // Find the direction in which we need to draw the plane
                    Vector3 diff = hallwayPoint - new Vector3(roomHallways[j].x, 0, roomHallways[j].z);

                    Vector2[] GridBounds = GetRoomBounds();
                    Vector3 GridOffset = new Vector3(GridBounds[0][0], 0, GridBounds[1][0]);

                    Vector3 pos = hallwayPoint + new Vector3(FinalRoomPlan[i].gridX, 0, FinalRoomPlan[i].gridY) + GridOffset;
                    Instantiate(blockerPlane, pos * scaleMultiplier, Quaternion.LookRotation(diff, Vector3.up), RoomParent.transform);
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
            case GridPoint.Doorway:
                return Color.yellow;
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
