using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class EventOnPlayerCollide : MonoBehaviour
{

    [SerializeField]
    private UnityEvent unityEvent;
    private Transform playerTrans;

    private void Start()
    {
        playerTrans = (Transform)Variables.ActiveScene.Get("player");
    }

    private void OnTriggerEnter(Collider other)
    {
        //if player, apply debuff
        if (other.transform == playerTrans)
        {
            unityEvent.Invoke();
        }
    }
}
