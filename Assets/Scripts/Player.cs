using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Movement))]
[RequireComponent(typeof(DebuffsManager))]
[RequireComponent(typeof(BuffsManager))]
public class Player : MonoBehaviour
{
    // Player Systems
    private Movement movement;
    private DebuffsManager debuffsManager;
    private BuffsManager buffsManager;

    public Movement Movement => movement;

    void Start()
    {
        // Player Systems
        movement = GetComponent<Movement>();
        debuffsManager = GetComponent<DebuffsManager>();
        buffsManager = GetComponent<BuffsManager>();
    }

    void FixedUpdate()
    {
        movement.MovePlayer();
    }

    void Update()
    {
        debuffsManager.UpdateDebuffs();
        buffsManager.UpdateBuffs();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        movement.SetInput(context.ReadValue<Vector2>());
    }

    public void OnFire()
    {
        /* debuffsManager.AddDebuff(new Slow(3f, 0.5f)); */
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        movement.SetLookInput(context.ReadValue<Vector2>());
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        movement.SetCrouch(context.ReadValueAsButton());
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        movement.SetSprint(context.ReadValueAsButton());
    }
}
