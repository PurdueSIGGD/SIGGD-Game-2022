using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

[RequireComponent(typeof(SphereCollider))]
public class StunTrap : DeployedObject
{
    [Header("Stun Radius Parameters")]
    [SerializeField] float detectionRadius;
    [SerializeField] float blastRadius;

    [Header("Stun Effect Parameters")]
    [SerializeField] float enemyStunDuration;

    SphereCollider col;

    // State of the trap
        // after an enemy triggers the trap, it isStunning for the remainder of the object's lifetime
    bool isStunning;

    internal override void Start()
    {
        base.Start();

        col = GetComponent<SphereCollider>();

        // initially sets the trigger collider's radius
        setColliderRadius(detectionRadius);
    }

    internal override void Update()
    {
        base.Update();

        // ensures the collider radius is properly adjusted if radii parameters are adjusted
        if (col.enabled && col.radius != detectionRadius)
            setColliderRadius(detectionRadius);
    }

    internal override void onEnemyTrigger(Collider enemyCollider)
    {
        Debug.Log("stunTrap triggered by: " + enemyCollider.gameObject.name);

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, blastRadius, layersToTriggerOn);
        foreach (Collider c in hitColliders)
        {            
            // only use trigger colliders
            if (c == null)
                continue;

            // stun enemy
            Debug.Log("stunTrap stunning " + c.name);
            CustomEvent.Trigger(c.gameObject, "stun");
        }

        // destroys this trap
        col.enabled = false; // to ensure that more collisions don't occur before the gameobject is actually destroyed
        Destroy(gameObject);
    }

    void setColliderRadius(float radius)
    {
        col.radius = radius;
    }

    private void OnDrawGizmos()
    {
        // draws the radii for debugging
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.DrawWireSphere(transform.position, blastRadius);
    }
}
