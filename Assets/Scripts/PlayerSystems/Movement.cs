using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] private float MaxSpeed = 10;
    [SerializeField] private float Friction = 100;
    [SerializeField] private float Acceleration = 100;
    [SerializeField] private float CamRotXSpeed = 0.2f;
    [SerializeField] private float CamRotYSpeed = 0.2f;

    private new Rigidbody rigidbody;
    private DebuffsManager debuffs;
    private Transform camHolderTransform;
    private Vector2 input;
    private Vector2 lookInput;
    private Vector2 velocity;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        debuffs = GetComponent<DebuffsManager>();
        camHolderTransform = GetComponentInChildren<Camera>().transform.parent;
    }

    public void SetInput(Vector2 input)
    {
        this.input = input;
    }

    public void SetLookInput(Vector2 lookInput)
    {
        this.lookInput = lookInput;
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
        // Movement
        if (input.Equals(Vector2.zero))
        {
            // if no input, slow down
            velocity = MoveTowards(velocity, Vector2.zero, Friction * Time.fixedDeltaTime);
        }
        else
        {
            // if input, accelerate
            //corrects the input to be in local space
            Vector3 localDir = transform.forward * input.y + transform.right * input.x;
            //corrects the 3dDir to be a 2d vector
            Vector2 corrInput = new Vector2(localDir.x, localDir.z);
            velocity = MoveTowards(velocity, corrInput * MaxSpeed, Acceleration * Time.fixedDeltaTime);
            //velocity = MoveTowards(velocity, input * MaxSpeed, Acceleration * Time.fixedDeltaTime);
        }

        //copies the y velocity so that velocity due to gravity is not removed
        Vector2 debuffedVelocity = debuffs.ApplySlow(velocity);
        Debug.Log(debuffedVelocity);
        Vector3 move = new Vector3(debuffedVelocity.x, rigidbody.velocity.y, debuffedVelocity.y) * Time.fixedDeltaTime;
        rigidbody.MovePosition(rigidbody.position + move);

        //Rotation

        if (!lookInput.Equals(Vector2.zero))
        {
            // if mouse movement, rotate player along global y and camera along local x (does not allow for camera orbiting, but allows for player rotation)
            transform.Rotate(new Vector3(0, lookInput.x * CamRotXSpeed, 0), Space.World);
            // if statement used to stop camera from going upside down
            float vertRotation = - lookInput.y * CamRotYSpeed;
            if ((camHolderTransform.localRotation.eulerAngles.x + vertRotation < 90) || camHolderTransform.localRotation.eulerAngles.x + vertRotation > 270) {
                //Debug.Log(camHolderTransform.localRotation.eulerAngles.x);
                camHolderTransform.Rotate(new Vector3(vertRotation, 0, 0), Space.Self);
            }
        }
    }

}
