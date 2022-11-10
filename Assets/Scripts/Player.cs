using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Movement))]
[RequireComponent(typeof(DebuffsManager))]
public class Player : MonoBehaviour
{
    // Player Systems
    private Movement movement;
    private DebuffsManager debuffsManager;

    public DebuffsManager Debuffs => debuffsManager;

    void Start()
    {
        // Player Systems
        movement = GetComponent<Movement>();
        debuffsManager = GetComponent<DebuffsManager>();
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

    // test slow debuff
    void OnFire()
    {
        debuffsManager.AddDebuff(new Slow(3f, 0.5f));
    }
}
