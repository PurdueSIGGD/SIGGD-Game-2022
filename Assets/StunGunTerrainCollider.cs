using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StunGunTerrainCollider : MonoBehaviour


{

    private void OnTriggerEnter(Collider collision)
    {
        if (collision == null)
            return;

        //Debug.Log("STUN GUN GENERAL COLLISION DETECTED");
        if ((collision.gameObject.layer != 10) &&
            (collision.transform != gameObject.transform.parent.transform) &&
            (collision.transform != (Transform)Variables.ActiveScene.Get("player")))
        {
            //Debug.Log("STUN GUN TERRAIN COLLISION DETECTED");
            StunGunProjectile enemyProjectile = GetComponentInParent<StunGunProjectile>();
            if (enemyProjectile != null)
            {
                enemyProjectile.hitTerrain();
            }
            Destroy(gameObject);
        }
    }
}
