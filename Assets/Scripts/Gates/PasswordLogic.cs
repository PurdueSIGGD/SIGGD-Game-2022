using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PasswordLogic : MonoBehaviour
{
    public InputField InputField;
    public Text PasswordText;
    public Text GivenPassword;
    public GameObject canvas;
    public Image tape;
    private GameObject door;
    private string password;
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
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
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
            password = door.GetComponent<GatesObj>().password;
            GivenPassword.text = "Password:     " + password;
            canvas.SetActive(true);
            if (door.GetComponent<GatesObj>().passwordGiven)
            {
                tape.enabled = false;
                Debug.Log("hi");
            }
            InputField.ActivateInputField();
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            FindObjectOfType<Player>().lockInputs = true;
            checkPassword();
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

    // Checks if the password entered starts to be incorrect,
    // change the color of the password to red
    public void checkPassword()
    {
        int passwordLength = password.Length;
        int inputLength;
        if (input == null) inputLength = 0;
        else inputLength = input.Length;

        if (inputLength == 0)
        {
            PasswordText.color = new Color(255, 255, 255, 255);
        }
        else if (inputLength > passwordLength || !input.Equals(password.Substring(0, inputLength)))
        {
            PasswordText.color = Color.red;
        }
        else
        {
            PasswordText.color = new Color(255, 255, 255, 255);
        }
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
        // Changes color of the text to green and deactivates input field and canvas
        InputField.DeactivateInputField();
        PasswordText.color = Color.green;
        yield return new WaitForSeconds(0.1f);
        PasswordText.color = new Color(255, 255, 255, 255);
        yield return new WaitForSeconds(0.1f);
        PasswordText.color = Color.green;
        yield return new WaitForSeconds(0.1f);
        PasswordText.color = new Color(255, 255, 255, 255);
        yield return new WaitForSeconds(0.1f);
        PasswordText.color = Color.green;
        yield return new WaitForSeconds(0.2f);
        canvas.SetActive(false);
        PasswordText.color = new Color(255, 255, 255, 255);
        FindObjectOfType<Player>().lockInputs = false;
        called = false;
    }
}
