using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class stopMovingOnCollide : MonoBehaviour
{
    private Transform playerTrans;
    private NavMeshAgent agent;

    private void Start()
    {
        playerTrans = (Transform)Variables.ActiveScene.Get("player");
        agent = GetComponent<NavMeshAgent>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        //if player, stop moving
        if (collision.transform == playerTrans)
        {
            agent.isStopped = true;
        }
    }
}
