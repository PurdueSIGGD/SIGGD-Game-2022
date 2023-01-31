using UnityEngine;

public class AudioDestroyer : MonoBehaviour
{
    private AudioSource AudioSource;
    
    // Start is called before the first frame update
    void Start()
    {
        AudioSource = GetComponent<AudioSource>();
        Destroy(gameObject, AudioSource.clip.length); //Destroys this gameobject after the audio clip is over
    }
}
