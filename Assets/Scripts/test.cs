using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    [SerializeField]
    private CapsuleCollider CC;
    [SerializeField]
    private int val = 8;
    
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.up * Time.deltaTime;
        Debug.Log("Test");
        CC.height = Random.value * val;
    }
}
