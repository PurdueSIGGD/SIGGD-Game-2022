using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeployedObject : MonoBehaviour
{
    // ASSUMES Enemies are on layer "Enemy"
    const string ENEMY_LAYER_NAME = "Enemy";

    [SerializeField] float timeUntilDestruction = 5;

    void Start()
    {
        Invoke("destroyDeployable", timeUntilDestruction);
    }

    void Update()
    {
        
    }

    internal virtual void affectEnemy(Collision enemyCollision)
    {
        // destroys enemy for testing        
        Destroy(enemyCollision.gameObject);
    }

    void destroyDeployable()
    {
        Destroy(gameObject);
    }

    void OnCollisionEnter(Collision other)
    {        
        // makes sure what has been touched is an enemy
        if (other != null && other.gameObject.layer == LayerMask.NameToLayer(ENEMY_LAYER_NAME))
            affectEnemy(other);
    }
}
