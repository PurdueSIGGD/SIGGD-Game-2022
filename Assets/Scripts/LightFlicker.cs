using System.Collections;
using System.Collections.Generic;
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
    }

    private IEnumerator flickerer()
    {
        while(true)
        {
            //choose on or off
            if (Random.value < offChance)
            {
                lightSource.intensity = 0;
            }
            else
            {
                lightSource.intensity = intensity;
            }
            //wait
            yield return new WaitForSeconds(hz + Random.value * hzOffset);
        }
    }
}
