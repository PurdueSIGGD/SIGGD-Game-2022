using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class IndependantMelee : attackManager
{
    [SerializeField] private attackTuple IndependantAttack;

    private new void Start()
    {
        base.Start();
        IEnumerator coroutine = doAttack(IndependantAttack.attack);
        StartCoroutine(coroutine);
    }
}
