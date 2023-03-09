using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deployer : Item
{
    [SerializeField] GameObject objectToDeploy;

    public override void Use()
    {
        Instantiate(objectToDeploy, transform.position, Quaternion.identity);

        base.Use();
    }
}
