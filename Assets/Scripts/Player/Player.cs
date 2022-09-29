using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

class Player : MonoBehaviour
{
    private const float MAX_SPEED = 10;
    private const float FRICTION = 100;
    private const float ACCELERATION = 100;

    private Vector2 Input;
    private Vector2 Velocity;

    /// <summary>
    /// Moves a vector2 towards a target vector2 by a given amount
    /// </summary>
    Vector2 MoveTowards(Vector2 curr, Vector2 target, float amount)
    {
        Vector2 diff = target - curr;
        Vector2 change = diff.normalized * amount;
        // clamp value based to the difference
        change = (diff.sqrMagnitude < change.sqrMagnitude ? diff : change);
        return curr + change;
    }

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.Equals(Vector2.zero))
        {
            Velocity = MoveTowards(Velocity, Vector2.zero, FRICTION * Time.deltaTime);
        }
        else
        {
            Velocity = MoveTowards(Velocity, Input * MAX_SPEED, ACCELERATION * Time.deltaTime);
        }
        
        Vector3 movement = new Vector3(Velocity.x, 0, Velocity.y) * Time.deltaTime;
        this.transform.Translate(movement);
    }

    void OnMove(InputValue movementValue)
    {
        Input = movementValue.Get<Vector2>();
        // Debug.Log(Input);
    }

}