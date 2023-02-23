using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PasswordLogic : MonoBehaviour
{
    public InputField InputField;
    public Text PasswordText;
    public GameObject canvas;
    private GameObject door;
    private DoorTrigger doorTrigger;
    private string input;
    private Item key = null;
    private bool called = false;

    private void Update()
    {
        // If the doorTrigger is not set to anything, leave the canvas and textfield blank
        if (doorTrigger == null)
        {
            canvas.SetActive(false);
            InputField.text = "";
            FindObjectOfType<Player>().lockInputs = false;
        }

        // Else if the password entered by the player is correct, take the canvas out of view
        // Entered Textfield should not be empty yet at this point
        else if (door != null && door.GetComponent<GatesObj>().openObj(key))
        {
            if (!called)
            {
                StartCoroutine("waitUI");
                called = true;
            }
        }

        // Else set canvas to active (for first time touching door)
        else
        {
            canvas.SetActive(true);
            InputField.ActivateInputField();
            FindObjectOfType<Player>().lockInputs = true;
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

    // If the password entered is correct, change the color of the password to green
    public void passwordCorrect()
    {

    }

    // If the password entered starts to be incorrect,
    // change the color of the password to red
    public void passwordIncorrect()
    {

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

    // if "esc" is pressed, the canvas is disabled
    public void ifEscPressed()
    {
        canvas.SetActive(false);
        FindObjectOfType<Player>().lockInputs = false;
        hasExited();
    }


    public IEnumerator waitUI() 
    {
        yield return new WaitForSeconds(0.4f);
        canvas.SetActive(false);
        FindObjectOfType<Player>().lockInputs = false;
        called = false;
    }
}
