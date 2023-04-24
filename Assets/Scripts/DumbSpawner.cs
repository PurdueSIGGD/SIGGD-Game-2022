using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DumbSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject[] items;
    
    // Start is called before the first frame update
    public void dumbSpawn()
    {
        GameObject toInstantiate = items[Random.Range(0, items.Length)];
        float offset = 0f;
        if (!toInstantiate.GetComponent<patrolManager>())
        {
            offset = 0.425f;
        }
        Instantiate(toInstantiate, transform.position + Vector3.up * offset, Quaternion.identity);
    }
}
