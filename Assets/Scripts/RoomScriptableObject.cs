using UnityEngine;

[CreateAssetMenu(fileName = "RoomScriptableObject", menuName = "ScriptableObjects/Room")]
public class RoomScriptableObject : ScriptableObject
{
    public GameObject room;
    public Vector3 dimensions;
    public Vector3[] hallways;
}
