using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicSwitcher : MonoBehaviour
{
    [SerializeField] private int destroyIndex;
    
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex == destroyIndex)
        {
            Destroy(gameObject);
        }
    }
}
