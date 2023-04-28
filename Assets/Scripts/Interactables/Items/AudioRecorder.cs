using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AudioRecorder : Item
{
    [SerializeField] private AudioClip audioClip;

    public override void Use()
    {
        // Based on the location, select one of the audio log files
        Transform playerTrans = (Transform)Variables.ActiveScene.Get("player");
        AudioSource audioSource = playerTrans.GetComponent<AudioSource>();
        audioSource.clip = audioClip;
        audioSource.Play();
    }
}
