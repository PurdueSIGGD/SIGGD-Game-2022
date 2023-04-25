using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCam : MonoBehaviour
{
    private Camera mainCam;

    private void Start()
    {
        mainCam = Camera.main;
    }

    void Update()
    {
        if (mainCam != null)
        {
            Vector3 diff = mainCam.transform.position - transform.position;
            transform.rotation = Quaternion.LookRotation(-diff, Vector3.up);
        }
    }
}
