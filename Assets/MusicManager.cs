using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private GameObject First;
    [SerializeField] private GameObject Last;
    public int progress = 0;
    private IEnumerator MyCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(First.GetComponent<AudioSource>().clip.length);
            First = MusicLoop(First);
            if (First == null)
            {
                Instantiate(Last);
                break;
            }
            Instantiate(First);
        }
    }
    private GameObject MusicLoop(GameObject CurrentSection)
    {
        AudioSection audioSection = CurrentSection.GetComponent<AudioSection>();
        if (audioSection.condition < 0)
        {
            return null;
        }
        if (audioSection.condition <= progress)
        {
            return audioSection.next2;
        }
        else { return audioSection.next; }
    }
    // Start is called before the first frame update
    void Start()
    {
        Instantiate(First);

        StartCoroutine(MyCoroutine());
    }
}

