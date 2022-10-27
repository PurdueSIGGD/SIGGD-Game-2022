using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Movement))]
[RequireComponent(typeof(Inventory))]
[RequireComponent(typeof(DebuffsManager))]
public class Player : MonoBehaviour
{
    // Player Systems
    private Movement movement;
    private Inventory inventory;
    private DebuffsManager debuffsManager;

    public Movement Movement => movement;

    void Start()
    {
        // Player Systems
        movement = GetComponent<Movement>();
        inventory = GetComponent<Inventory>();
    }

    void FixedUpdate()
    {
        movement.MovePlayer();
    }

    void Update()
    {
        debuffsManager.UpdateDebuffs();
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