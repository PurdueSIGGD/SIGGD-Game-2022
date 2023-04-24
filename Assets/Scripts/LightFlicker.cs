using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    private Light lightSource;
    private float intensity;
    [SerializeField]
    private float flickerChance = 0.25f;
    [SerializeField]
    private float hz = 0.25f;
    [SerializeField]
    private float hzOffset = 0.1f; //This is to top too many calculations taking place on one frame
    [SerializeField]
    private float offChance = 0.25f;
    private Transform player;
    private bool off = false;
    
    // Start is called before the first frame update
    void Start()
    {
        lightSource = GetComponent<Light>();
        intensity = lightSource.intensity;
        if (Random.value < flickerChance)
        {
            IEnumerator coroutine = flickerer();
            StartCoroutine(coroutine);
        }
        player = (Transform)Variables.ActiveScene.Get("player");
    }

    private void Update()
    {
        //this recuces the overall active lights in the scene to increase the shadow res of the nearby ones
        float cullDist = 32.0f;
        if (!off)
        {
            if (Vector3.SqrMagnitude(transform.position - player.position) > cullDist * cullDist)
            {
                lightSource.enabled = false;
            }
            else
            {
                lightSource.enabled = true;
            }
        }
    }

    private IEnumerator flickerer()
    {
        while(true)
        {
            //choose on or off
            if (Random.value < offChance)
            {
                lightSource.enabled = false;
                off = true;
            }
            else
            {
                lightSource.enabled = true;
                off = false;
            }
            //wait
            yield return new WaitForSeconds(hz + Random.value * hzOffset);
        }
    }
}
