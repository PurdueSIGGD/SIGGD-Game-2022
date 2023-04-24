using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class KillAfterTime : MonoBehaviour
{
    [SerializeField] private float windupTime;
    [SerializeField] private float maxRange;
    private Transform playerTrans;

    // Start is called before the first frame update
    void Start()
    {
        playerTrans = (Transform)Variables.ActiveScene.Get("player");
        IEnumerator coroutine = tryKill();
        StartCoroutine(coroutine);
    }

    IEnumerator tryKill()
    {
        yield return new WaitForSeconds(windupTime);

        Vector3 toPlayerVector = playerTrans.position - transform.position;

        if (Vector3.SqrMagnitude(toPlayerVector) <= maxRange * maxRange)
        {
            playerTrans.GetComponent<Player>().kill();
        }
        else
        {
            Debug.Log(transform.name + "'s attack missed");
        }
    }
}
