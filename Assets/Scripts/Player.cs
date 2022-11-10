using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Movement))]
[RequireComponent(typeof(SEManager))]
public class Player : MonoBehaviour
{
    // Player Systems
    private Movement movement;
    private SEManager _SEManager;

    public SEManager SEManager => _SEManager;
    void Start()
    {
        // Player Systems
        movement = GetComponent<Movement>();
        _SEManager = GetComponent<SEManager>();
    }

    void FixedUpdate()
    {
        movement.MovePlayer();
    }

    void Update()
    {
        _SEManager.UpdateStatusEffects();
    }

    void OnMove(InputValue movementValue)
    {
        movement.SetInput(movementValue.Get<Vector2>());
    }

    void OnLook(InputValue lookValue)
    {
        movement.SetLookInput(lookValue.Get<Vector2>());
    }

    // test slow debuff
    void OnFire()
    {
        _SEManager.AddDebuff(new MovementSE(3f, 0.3f));
    }
}
