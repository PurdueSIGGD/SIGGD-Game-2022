using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PavelAudio : MonoBehaviour
{
    [SerializeField] private AudioClip[] startClips;
    [SerializeField] private AudioClip[] endClips;
    private AudioSource audioSource;
    private int sceneNum;
    
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        sceneNum = SceneManager.GetActiveScene().buildIndex;
        AudioClip startClip = startClips[sceneNum];
        if (startClip != null)
        {
            audioSource.clip = startClip;
            audioSource.Play();
        }
    }

    // Update is called once per frame
    public float endLevel()
    {
        AudioClip endClip = endClips[sceneNum];
        if (endClip != null)
        {
            audioSource.clip = endClip;
            audioSource.Play();
            return endClip.length;
        }
        return 0;
    }
}
