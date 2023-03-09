using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomDelete : MonoBehaviour
{
    [SerializeField]
    private float deletionChance = 0.5f;
    
    // Start is called before the first frame update
    void Start()
    {
        if (Random.value < deletionChance)
        {
            Destroy(gameObject);
        }
    }

}
