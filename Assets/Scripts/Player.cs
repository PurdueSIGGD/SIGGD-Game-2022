using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Movement))]
[RequireComponent(typeof(Inventory))]
[RequireComponent(typeof(Stamina))]
class Player : MonoBehaviour
{
    // Player Systems
    private Movement movement;
    private Inventory inventory;
    private Stamina stamina;

    void Start()
    {
        // Player Systems
        movement = GetComponent<Movement>();
        inventory = GetComponent<Inventory>();
        stamina = GetComponent<Stamina>();
    }

    void FixedUpdate()
    {
        movement.MovePlayer();
    }

    void OnMove(InputValue movementValue)
    {
        movement.SetInput(movementValue.Get<Vector2>());
    }

}