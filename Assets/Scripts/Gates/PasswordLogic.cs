using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PasswordLogic : MonoBehaviour
{
    public InputField InputField;
    public GameObject canvas;
    private GameObject door;
    private DoorTrigger doorTrigger;
    private string input;
    private Item key = null;

    // Start is called before the first frame update
    void Start()
    {

    }

    private void Update()
    {
        // If the doorTrigger is not set to anything, leave the canvas and textfield blank
        if (doorTrigger == null)
        {
            canvas.SetActive(false);
            InputField.text = "";
        }

        // Else if the password entered by the player is correct, take the canvas out of view
        // Entered Textfield should not be empty yet at this point
        else if (door != null && door.GetComponent<GatesObj>().openObj(key))
        {
            canvas.SetActive(false);
        }

        // Else set canvas to active (for first time touching door)
        else 
        {
            canvas.SetActive(true);
            InputField.ActivateInputField();
        }
    }

    // Records the password the player inputs
    public void readInput(string input)
    {
        this.input = input;
    }

    // Reads the password the player inputs
    public string getInput()
    {
        return input;
    }

    // Sets door and door trigger as an object when player enters the door trigger
    public void hasEntered(GameObject door, DoorTrigger doorTrigger)
    {
        this.door = door;
        this.doorTrigger = doorTrigger;
    }

    // Sets door and door trigger null when player enters the door trigger
    public void hasExited()
    {
        doorTrigger = null;
        door = null;
    }
}
