using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeployedObject : MonoBehaviour
{    
    [Header("Deploy Default Parameters")]
    [SerializeField] internal LayerMask layersToTriggerOn;
    [SerializeField] float timeUntilDestruction = 5;

    internal virtual void Start()
    {
        Invoke("destroyDeployable", timeUntilDestruction);
    }

    internal virtual void Update()
    {

    }

    internal virtual void onEnemyTrigger(Collider enemyCollider)
    {
        // destroys enemy for testing        
        Destroy(enemyCollider.gameObject);
    }

    void destroyDeployable()
    {
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        // makes sure what has been touched is an enemy
        if (other != null && IInteractable.isLayerInLayerMask(other.gameObject.layer, layersToTriggerOn))
            onEnemyTrigger(other);
    }
}
