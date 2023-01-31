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
        /* debuffsManager.AddDebuff(new Slow(3f, 0.5f)); */
    }

    private void OnTriggerEnter(Collider other)
    {
        other.GetComponent<GatesObj>()?.openObj(null, "string");
    }

    
}
