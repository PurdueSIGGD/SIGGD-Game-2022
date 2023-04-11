using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerSpawnPoint : MonoBehaviour
{
    public void spawn()
    {
        Transform playerTrans = (Transform)Variables.ActiveScene.Get("player");
        CharacterController CharC = playerTrans.GetComponent<CharacterController>();
        //can only teleport player if turn off the character controller
        CharC.enabled = false;
        playerTrans.position = transform.position + Vector3.up * 0.85f;
        CharC.enabled = true;
    }
}
