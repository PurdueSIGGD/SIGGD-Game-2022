using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] footsteps;
    [SerializeField]
    private float distPerStep;
    private float stepMeter = 0;
    
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
}
