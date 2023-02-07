using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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

        //hide/lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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

    public void kill()
    {
        if (!Invincible.isInvincible())
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
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
}
