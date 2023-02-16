using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PasswordLogic : MonoBehaviour
{
    public InputField InputField;
    public DoorTrigger doorTrigger;
    public GameObject canvas;

    // Start is called before the first frame update
    void Start()
    {
        InputField.ActivateInputField();
    }

    private void Update()
    {
        if (doorTrigger != null)
        {
            canvas.SetActive(true);
        }
        else
        {
            canvas.SetActive(false);
        }
    }

    // Records the password the player inputs
    public void readInput(string input)
    {
        return;
    }

    // Sets door trigger as an object when player enters the door trigger
    public void hasEntered(DoorTrigger doorTrigger)
    {
        this.doorTrigger = doorTrigger;
    }

    // Sets door trigger null when player enters the door trigger
    public void hasExited()
    {
        doorTrigger = null;
    }
}
