using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralGenerator : MonoBehaviour
{
    [SerializeField]
    public int FinalRoomCount, TotalRoomCount;

    [SerializeField]
    private int MinRoomWidth, MinRoomDepth, MaxRoomWidth, MaxRoomDepth, TileSize;

    [SerializeField]
    private GameObject randomObject, floor;

    private float inRadius = 10;

    private List<Rect> RoomRects;

    // Start is called before the first frame update
    void Awake()
    {
        RoomRects = new List<Rect>();
        for (int i = 0; i < TotalRoomCount; i++)
        {
            Vector2 randomPoint = getRandomPointInCircle(inRadius);

            //Debug display of the room points as 3D Spheres.
            //Instantiate(randomObject, new Vector3(randomPoint.x, 0, randomPoint.y), Quaternion.identity);

            //Randomly select room width and depth in range [MinRoom, MaxRoom] inclusively.
            //Width and Depth are both integers.
            int roomWidth = Random.Range(MinRoomWidth, MaxRoomWidth + 1);
            int roomDepth = Random.Range(MinRoomDepth, MaxRoomDepth + 1);

            //Initialize the room with a Rect data structure for easy manipulation.
            //The final display will convert the Rects into 3D rooms.
            RoomRects.Add(new Rect(randomPoint.x, randomPoint.y, roomWidth, roomDepth));
        }
        Seperate();
    }

    void Start()
    {
        DrawRooms();
    }

    public Vector2 getRandomPointInCircle(float maxRadius)
    {
        float radius = Mathf.Sqrt(Random.Range(0.0f, maxRadius));
        float theta = Random.Range(0.0f, 2 * Mathf.PI);
        return new Vector2(Mathf.Round(radius * Mathf.Cos(theta)), Mathf.Round(radius * Mathf.Sin(theta)));
    }

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

                    RoomsOverlap = true;

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

                    Vector2 direction = (RoomRects[current].center - RoomRects[other].center).normalized;
                    curRoomRect.position = curRoomRect.position + (direction * TileSize);
                    otherRoomRect.position = otherRoomRect.position + ((-direction) * TileSize);

                    RoomRects[current] = curRoomRect;
                    RoomRects[other] = otherRoomRect;
                }
            }
        } while (RoomsOverlap == true);
    }

    private void DrawRooms()
    {
        foreach (Rect rect in RoomRects)
        {
            GameObject testFloor = Instantiate(floor, new Vector3(rect.center.x, 0, rect.center.y), Quaternion.identity);
            testFloor.transform.localScale = (new Vector3(rect.width, 1, rect.height)) / 10;
        }
    }

    //UNUSED
    //Checks if any of the rectangles in RoomRects overlap
    private bool CheckCollisions()
    {
        for (int i = 0; i < RoomRects.Count; i++)
        {
            for (int j = i + 1; j < RoomRects.Count; j++)
            {
                if (RoomRects[i].Overlaps(RoomRects[j])) return true;
            }
        }

        return false;
    }


    // display a rectangle
    /*void OnGUI()
    {
        foreach (Rect rect in RoomRects)
        {
            GUI.Box(rect, "");
        }
    }*/
}
