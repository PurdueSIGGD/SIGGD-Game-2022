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
    private float nearDistance;
    [SerializeField]
    private Transform eyePos;
    [SerializeField]
    private LayerMask layersToCheck;

    // Returns true if the player is visible
    public bool isPlayerVisible()
    {

        if (Invisible.isInvisible())
        {
            return false;
        }

        Transform playerTrans = (Transform)Variables.ActiveScene.Get("player");
        //to show what the enemy is doing
        Debug.DrawRay(eyePos.position, eyePos.forward * sightDistance, Color.white);

        Vector3 playerPosCorrected = playerTrans.position + Vector3.up * 0.425f; //makes enemies try to look "eye-to-eye" rather than at the torso

        //if player is close enough, can "see" outside of the normal angle, othersize check if the angle is correct
        if (Vector3.Distance(eyePos.position, playerPosCorrected) > nearDistance)
        {
            //if player outside angle of cone, not seen
            if (Vector3.Angle(eyePos.forward, playerPosCorrected - eyePos.position) > viewAngle)
            {
                return false;
            }
            RaycastHit hit;
            if (Physics.Raycast(eyePos.position, playerPosCorrected - eyePos.position, out hit, sightDistance, layersToCheck))
            {
                //player is only seen if it is the thing hit by the ray
                Debug.DrawRay(eyePos.position, hit.point - eyePos.position, Color.red);
                return (hit.transform == playerTrans);
            }
            else
            {
                Debug.DrawRay(eyePos.position, playerPosCorrected - eyePos.position);
            }
        }

        //if ray does not hit anything or outside of distance, not seen
        return false;
    }
}
