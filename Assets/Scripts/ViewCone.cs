using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

public class ViewCone : MonoBehaviour
{
    [SerializeField]
    private float viewAngle;
    [SerializeField]
    private float sightDistance;
    [SerializeField]
    private Transform eyePos;
    [SerializeField]
    private LayerMask layersToCheck;

    // Returns true if the player is visible
    public bool isPlayerVisible()
    {
        Transform playerTrans = (Transform)Variables.ActiveScene.Get("player");
        //to show what the enemy is doing
        Debug.DrawRay(eyePos.position, eyePos.forward * sightDistance, Color.white);

        //if player outside angle of cone, not seen
        if (Vector3.Angle(eyePos.forward, playerTrans.position - eyePos.position) > viewAngle)
        {
            return false;
        }

        RaycastHit hit;
        if (Physics.Raycast(eyePos.position, playerTrans.position - eyePos.position, out hit, sightDistance, layersToCheck))
        {
            //player is only seen if it is the thing hit by the ray
            Debug.DrawRay(eyePos.position, hit.point - eyePos.position, Color.red);
            return (hit.transform == playerTrans);
        }
        //if ray does not hit anything, not seen
        return false;
    }
}
