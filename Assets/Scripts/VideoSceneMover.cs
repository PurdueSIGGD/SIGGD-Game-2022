using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class VideoSceneMover : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        IEnumerator coroutine = MoveToNext();
        StartCoroutine(coroutine);
    }

    public IEnumerator MoveToNext() {
        double len = GetComponent<VideoPlayer>().length;
        yield return new WaitForSeconds((float)len);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
