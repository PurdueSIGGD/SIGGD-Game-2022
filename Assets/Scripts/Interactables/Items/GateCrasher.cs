using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateCrasher : Item
{
    public override void Use()
    {
        FindObjectOfType<PasswordLogic>().activeGateBreaker++;
    }

    public override void End()
    {
        FindObjectOfType<PasswordLogic>().activeGateBreaker--;
    }
}
