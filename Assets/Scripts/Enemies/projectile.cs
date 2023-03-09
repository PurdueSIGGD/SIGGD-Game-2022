using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class projectile : MonoBehaviour
{
    [SerializeField]
    private float speed;
    [SerializeField]
    private bool killOnHit; //Most ranged attacks will not on hit, and instead give debuffs
    //[SerializeField]
    //private Debuff debuff; //Does not do anything right now, will implement later
    private Rigidbody rb;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * speed;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Transform playerTrans = (Transform)Variables.ActiveScene.Get("player");

        //if close enough, kill or apply debuff
        if (other.transform == playerTrans)
        {
            if (killOnHit)
            {
                playerTrans.GetComponent<Player>().kill();
            } else if (false) //check for debuff existing here
            {
                //apply debuff to the player
            }
        }

        Destroy(gameObject);
    }
}
