using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralGenerator : MonoBehaviour
{
    [SerializeField]
    int RoomCount = 10;

    [SerializeField]
    GameObject randomObject;

    [SerializeField]
    float inRadius = 10;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 200; i++)
        {
            Instantiate(randomObject, getRandomPointInCircle(inRadius), Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector3 getRandomPointInCircle(float maxRadius)
    {
        float radius = Mathf.Sqrt(Random.Range(0.0f, maxRadius));
        float theta = Random.Range(0.0f, 2 * Mathf.PI);
        return new Vector3(radius * Mathf.Cos(theta), 0, radius * Mathf.Sin(theta));
    }
}
