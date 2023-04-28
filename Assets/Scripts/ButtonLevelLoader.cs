using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonLevelLoader : MonoBehaviour
{
    [SerializeField] private int toLoad;
    public void load()
    {
        SceneManager.LoadScene(toLoad);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
