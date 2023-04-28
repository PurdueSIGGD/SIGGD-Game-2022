using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

/** If player enters trigger, the player is moved to the next scene
 */

public class ExitTrigger : MonoBehaviour
{
    private bool triggeredYet = false;
    
    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && !triggeredYet)
        {
            triggeredYet= true;
            float T = other.GetComponentInChildren<PavelAudio>().endLevel();
            if (T > 0)
            {
                IEnumerator coroutine = waitToLeave(T);
                StartCoroutine(coroutine);
            }
        }
    }

    public IEnumerator waitToLeave(float waitTime)
    {
        yield return new WaitForSeconds(waitTime + 0.5f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
