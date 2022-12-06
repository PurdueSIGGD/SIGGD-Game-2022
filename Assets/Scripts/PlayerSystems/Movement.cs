using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] private float MaxSpeed = 10;
    [SerializeField] private float Friction = 100;
    [SerializeField] private float Acceleration = 100;
    [SerializeField] private float gravity = 9.8f;
    [SerializeField] private float CamRotXSpeed = 0.2f;
    [SerializeField] private float CamRotYSpeed = 0.2f;

    private DebuffsManager debuffs;
    private Transform camHolderTransform;
    private Vector2 input;
    private Vector2 lookInput;
    private Vector3 velocity;

    private CharacterController charController;

    void Start()
    {
        charController = GetComponent<CharacterController>();
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

    private const float GROUND_SNAP = 0.5f;



    public void MovePlayer()
    {
        // Movement
        if (input.Equals(Vector2.zero))
        {
            // if no input, slow down
            velocity = Vector3.MoveTowards(velocity, Vector3.up * velocity.y, Friction * Time.fixedDeltaTime);
        }
        else
        {
            // if input, accelerate
            //corrects the input to be in local space
            Vector3 localDir = transform.forward * input.y + transform.right * input.x;
            //corrects the 3dDir to be a 2d vector
            // Vector2 corrInput = new Vector2(localDir.x, localDir.z);
            velocity = Vector3.MoveTowards(velocity, localDir * MaxSpeed + Vector3.up * velocity.y, Acceleration * Time.fixedDeltaTime);
        }

        //   Creates a new move vector with Buffs and Debuffs applied
        //copies the y velocity so that velocity due to gravity is not removed
        Vector2 modifiedVelocity = new Vector2(velocity.x, velocity.z);
        modifiedVelocity = Slow.CalculateVelocity(modifiedVelocity);
        modifiedVelocity = SpeedBoost.CalculateVelocity(modifiedVelocity);
        //Debug.Log(debuffedVelocity);
        Vector3 move = new Vector3(modifiedVelocity.x, velocity.y, modifiedVelocity.y) * Time.fixedDeltaTime;
        charController.Move(move);

        { // handle ground snap
            var savedPos = transform.position;
            charController.Move(Vector3.down * GROUND_SNAP);
            if (!charController.isGrounded) {
                transform.position = savedPos;
            }
        }

        if (charController.isGrounded) {
            velocity.y = -0.0001f;
        } else {
            velocity += Vector3.down * gravity * Time.deltaTime;
        }

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
