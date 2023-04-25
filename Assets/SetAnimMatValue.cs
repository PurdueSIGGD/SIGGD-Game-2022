using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SetAnimMatValue : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponentInChildren<DecalProjector>().material.SetFloat("StartTime", Time.deltaTime);
    }

}
