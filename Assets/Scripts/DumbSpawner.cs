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
        Instantiate(items[Random.Range(0, items.Length)], transform.position + Vector3.up*0.85f, Quaternion.identity);
    }
}
