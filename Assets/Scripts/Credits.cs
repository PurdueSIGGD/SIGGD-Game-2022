using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Credits : MonoBehaviour
{
    private float startY;
    private RectTransform rectTransform;
    [SerializeField] private float length;
    [SerializeField] private float stopLength;
    [SerializeField] private float speed;
    private float counter;

    private void Start()
    {
        startY = transform.position.y;
        rectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (rectTransform != null && rectTransform.anchoredPosition.y < stopLength)
        {
            rectTransform.anchoredPosition += Vector2.up * (length / speed) * Time.deltaTime;
        }
        counter += Time.deltaTime;
        if (counter > speed)
        {
            SceneManager.LoadScene(0);
        }
    }
}
