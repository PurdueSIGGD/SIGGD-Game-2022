using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Movement))]
[RequireComponent(typeof(Inventory))]
[RequireComponent(typeof(Stamina))]
class Player : MonoBehaviour
{
    private new Rigidbody rigidbody;
    // Player Systems
    private Movement movement;
    private Inventory inventory;
    private Stamina stamina;

    // private variables
    private Vector2 input;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();

        // Player Systems
        movement = GetComponent<Movement>();
        inventory = GetComponent<Inventory>();
        stamina = GetComponent<Stamina>();
    }

    void FixedUpdate()
    {
        rigidbody.MovePosition(rigidbody.position + movement.GetMovement(input, Time.fixedDeltaTime));
    }

    void OnMove(InputValue movementValue)
    {
        input = movementValue.Get<Vector2>();
        // Debug.Log(Input);
    }

}