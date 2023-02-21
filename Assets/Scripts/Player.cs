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

    [SerializeField] private LayerMask cameraRaycastMask;

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

        correctCameraDistance();

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

    [SerializeField] private float angleSpread;
    [SerializeField] private float maxCameraCastDist;

    private void correctCameraDistance() {
        var camTrans = GetComponentInChildren<Camera>().transform;
        var camHolderTrans = camTrans.transform.parent;
        var sideAmount = maxCameraCastDist * Mathf.Tan(angleSpread);
            
        var minDistFound = maxCameraCastDist;
        var middleDist = maxCameraCastDist;
        var totalDistFound = 0.0f;
        for (int i = -1; i <= 1; i++) {
            for (int j = -1; j <= 1; j++) {
                var holdSideDir = camHolderTrans.TransformDirection(Vector3.right);
                var holdUpDir = camHolderTrans.TransformDirection(Vector3.up);

                var rayDisp = -camHolderTrans.forward * maxCameraCastDist + holdSideDir * i * sideAmount + holdUpDir * j * sideAmount;

                RaycastHit castInfo;
                if (Physics.Raycast(camHolderTrans.position, rayDisp.normalized, out castInfo, maxCameraCastDist, cameraRaycastMask)) {
                    totalDistFound += castInfo.distance;
                    minDistFound = Mathf.Min(castInfo.distance, minDistFound);
                    if (i == 0 && j == 0) {
                        middleDist = castInfo.distance;
                    }
                } else {
                    totalDistFound += maxCameraCastDist;
                }
            }
        }

        var bias = 0.05f;

        var newDist = Mathf.MoveTowards(-camTrans.localPosition.z, minDistFound - bias, 4.0f * Time.deltaTime);
        camTrans.localPosition = Vector3.back * newDist;

        if (-camTrans.localPosition.z > middleDist - bias) {
            camTrans.localPosition = Vector3.back * (middleDist - bias);
        }
    }


}
