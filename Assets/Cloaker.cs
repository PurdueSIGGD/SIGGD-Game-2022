using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloaker : Item
{
    [SerializeField] public float invisibilityDuration;


    public override void Use()
    {
        BuffsManager buffsManager = GameObject.Find("Player").GetComponent<BuffsManager>();
        if (buffsManager != null)
        {
            buffsManager.AddBuff(new Invisible(invisibilityDuration));
            Debug.Log($"Cloaker was called");
        }
    }
}
