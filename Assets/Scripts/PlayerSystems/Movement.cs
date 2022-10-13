using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] private float MaxSpeed = 10;
    [SerializeField] private float Friction = 100;
    [SerializeField] private float Acceleration = 100;

    private new Rigidbody rigidbody;
    private Vector2 input;
    private Vector2 velocity;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    public void SetInput(Vector2 input)
    {
        this.input = input;
    }

    /// <summary>
    /// Moves a vector2 towards a target vector2 by a given amount
    /// </summary>
    private Vector2 MoveTowards(Vector2 curr, Vector2 target, float amount)
    {
        Vector2 diff = target - curr;
        Vector2 change = diff.normalized * amount;
        // clamp value based to the difference
        change = (diff.sqrMagnitude < change.sqrMagnitude ? diff : change);
        return curr + change;
    }

    public void MovePlayer()
    {
        if (input.Equals(Vector2.zero))
        {
            // if no input, slow down
            velocity = MoveTowards(velocity, Vector2.zero, Friction * Time.fixedDeltaTime);
        }
        else
        {
            // if input, accelerate
            velocity = MoveTowards(velocity, input * MaxSpeed, Acceleration * Time.fixedDeltaTime);
        }

        Vector3 move = new Vector3(velocity.x, 0, velocity.y);
        
        rigidbody.velocity = move;
    }

}
