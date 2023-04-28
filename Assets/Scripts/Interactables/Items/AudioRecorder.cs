using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioRecorder : Item
{
    [SerializeField] private Item audioRecorder;
    [SerializeField] private AudioSource audioSource;

    public override void Use()
    {
        // Based on the location, select one of the audio log files

        audioSource.Play();
    }
}
