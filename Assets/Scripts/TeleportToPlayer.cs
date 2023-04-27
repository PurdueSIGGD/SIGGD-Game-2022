using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TeleportToPlayer : MonoBehaviour
{
    [SerializeField] private float offset;
    
    // Start is called before the first frame update
    void Start()
    {
        Transform playerTrans = (Transform)Variables.ActiveScene.Get("player");
        transform.position = playerTrans.position + Vector3.up * offset;
    }
}
