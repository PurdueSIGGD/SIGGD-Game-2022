using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

public class StunGunProjectile : MonoBehaviour
{
    [Header("Collision Layers")]
    [SerializeField] LayerMask enemyLayers;
    [SerializeField] LayerMask terrainLayers;

    [Header("Projectile Parameters")]
    [SerializeField] float speed = 5;
    [SerializeField] float radius = .5f;
    [SerializeField] float maxDistance = 30;
    [SerializeField] float stunTime = 4;

    InventorySystem inventorySystem;
    Transform stunGunShootPos;
    Collider col;

    Vector3 startPos;
    bool returningToPlayer;

    void Awake()
    {
        inventorySystem = InventorySystem.instance;
        stunGunShootPos = FindObjectOfType<StunGun>().shootPos;
        col = GetComponent<Collider>();

        // sets position to shooting position
        transform.position = stunGunShootPos.position;
        startPos = transform.position;

        // rotates to face the direction the player is trying to shoot        
        transform.rotation = stunGunShootPos.rotation;

        // scales the projectile according to the radius field, if improperly sized
        if (transform.localScale.x != radius)
            transform.localScale = Vector3.one * radius;

        // lets the inventory track that there is a Stun Gun Projectile out
        inventorySystem.hasNotReCapturedProjectile = true;
    }

    void Update()
    {
        if (returningToPlayer)
        {
            // come back to player
            transform.position = Vector3.MoveTowards(transform.position, stunGunShootPos.position, speed * Time.deltaTime);

            // destroy projectile if it has returned to the player
            if (Vector3.Distance(transform.position, stunGunShootPos.position) < .1f)
                destroyProjectile();

            return;
        }

        // normal behavior
        transform.position += transform.forward * speed * Time.deltaTime;

        // start returning to player if traveled too far
        if (Vector3.Distance(transform.position, startPos) > maxDistance)
            startReturningToPlayer();
    }

    void startReturningToPlayer()
    {
        returningToPlayer = true;
        col.enabled = false;
    }

    void hitEnemy(Collision collision)
    {
        CustomEvent.Trigger(collision.gameObject, "stun", stunTime);
        //destroy(collision.gameObject);
        InventorySystem.instance.decrementStunGunAmmo();

        startReturningToPlayer();

        destroyProjectile();
    }

    void destroyProjectile()
    {
        inventorySystem.hasNotReCapturedProjectile = false;
        Destroy(gameObject);
    }

    void hitTerrain()
    {
        startReturningToPlayer();
    }

    void OnCollisionEnter(Collision collision)
    {
        // collider should be disabled when returning to player, so this case should never occur
        if (collision == null || returningToPlayer)
            return;

        //temporary replacement as commented code was not working
        if (collision.gameObject.layer == 10)
        {
            Debug.Log("Hit Enemy: " + collision.transform.name);
            hitEnemy(collision);
        } else
        {
            Debug.Log("Hit Terrain: " + collision.transform.name);
            hitTerrain();
        }

        /*
        int collisionLayer = collision.gameObject.layer;        
        if (IInteractable.isLayerInLayerMask(collisionLayer, enemyLayers))
            hitEnemy(collision);
        else if (IInteractable.isLayerInLayerMask(collisionLayer, terrainLayers))
            hitTerrain();
        */
    }
}
