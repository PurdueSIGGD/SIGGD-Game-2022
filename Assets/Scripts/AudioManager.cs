using UnityEngine;
using UnityEngine.Rendering.Universal;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] footsteps;
    [SerializeField]
    private float distPerStep;
    private float stepMeter = 0;
    [SerializeField]
    private GameObject sightedSound;
    [SerializeField]
    private GameObject stunSound;
    [SerializeField]
    private GameObject aggroSound;
    [SerializeField]
    private GameObject deaggroSound;

    //used to calculate and play footsteps, intended to be called every frame of movement with the current speed
    public void tryStep(float ammount) //ammount is the speed moved at the time
    {
        stepMeter += ammount * Time.deltaTime;
        if (stepMeter >= distPerStep)
        {
            Instantiate(footsteps[Random.Range(0, footsteps.Length)], transform.position, Quaternion.identity);
            stepMeter -= distPerStep;
        }
    }

    public void playAggro()
    {
        Instantiate(aggroSound, transform.position, Quaternion.identity);
    }

    public void playDeAggro()
    {
        Instantiate(deaggroSound, transform.position, Quaternion.identity);
    }

    public void playStun()
    {
        Instantiate(stunSound, transform.position, Quaternion.identity);
    }

    public void playSighted()
    {
        Instantiate(sightedSound, transform.position, Quaternion.identity);
    }
}
