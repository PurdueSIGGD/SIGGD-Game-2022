using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Movement))]
//[RequireComponent(typeof(Inventory))]
class Player : MonoBehaviour
{
    // Player Systems
    private Movement movement;
    //private Inventory inventory;

    void Start()
    {
        // Player Systems
        movement = GetComponent<Movement>();
        //inventory = GetComponent<Inventory>();
    }

    void FixedUpdate()
    {
        movement.MovePlayer();
    }

    void Update()
    {

    }

    void OnMove(InputValue movementValue)
    {
        movement.SetInput(movementValue.Get<Vector2>());
    }

    void OnLook(InputValue lookValue)
    {
        movement.SetLookInput(lookValue.Get<Vector2>());
    }
}