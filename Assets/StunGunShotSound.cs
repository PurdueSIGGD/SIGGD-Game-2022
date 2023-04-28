using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunGunShotSound : MonoBehaviour
{
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        Destroy(gameObject, audioSource.clip.length); //Destroys this gameobject after the audio clip is over
    }
}
