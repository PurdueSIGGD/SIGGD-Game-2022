using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

class Player : MonoBehaviour
{
    [SerializeField] private float MaxSpeed = 10;
    [SerializeField] private float Friction = 100;
    [SerializeField] private float Acceleration = 100;

    private Vector2 input;
    private Vector2 velocity;

    private new Rigidbody rigidbody;

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
        rigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (input.Equals(Vector2.zero))
        {
            // if no input, slow down
            velocity = MoveTowards(velocity, Vector2.zero, Friction * Time.deltaTime);
        }
        else
        {
            // if input, accelerate
            velocity = MoveTowards(velocity, input * MaxSpeed, Acceleration * Time.deltaTime);
        }
        
        Vector3 movement = new Vector3(velocity.x, 0, velocity.y) * Time.deltaTime;
        rigidbody.MovePosition(rigidbody.position + movement);
    }

    void OnMove(InputValue movementValue)
    {
        input = movementValue.Get<Vector2>();
        // Debug.Log(Input);
    }

}