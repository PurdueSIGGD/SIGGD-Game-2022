using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RemoveIfDoorNear : MonoBehaviour
{
    public void ClearWrongDoors()
    {
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("DoorBlocker"))
        {
            if (Vector3.SqrMagnitude(g.transform.position - transform.position) <= 0.85f * 0.85f)
            {
                Destroy(GetComponent<GatesObj>());
                break;
            }
        }
    }
}
