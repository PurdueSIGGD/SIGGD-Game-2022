using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioRecorder : Item
{
    [SerializeField] private AudioSource[] audioLogs;
    [SerializeField] private AudioSource KGBLog1;
    [SerializeField] private AudioSource KGBLog2;
    [SerializeField] private AudioSource KGBLog3;
    [SerializeField] private int useAudio;
    private AudioSource audioToPlay;

    public override void Use()
    {
        // Based on the location, select one of the audio log files
        if (useAudio == 1)
        {
            audioToPlay = KGBLog1;
        }
        else if (useAudio == 2)
        {
            audioToPlay = KGBLog2;
        }
        else if (useAudio == 3)
        {
            audioToPlay = KGBLog3;
        }
        else
        {
            audioToPlay = audioLogs[Random.Range(0, audioLogs.Length - 1)];
        }

        audioToPlay.Play();
    }
}
