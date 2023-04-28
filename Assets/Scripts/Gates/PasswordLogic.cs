using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PasswordLogic : MonoBehaviour
{
    public int activeGateBreaker = 0;
    public bool openDoorWithGateCrasher = false;

    public InputField inputField;
    public Text passwordText;
    public Text psudoPasswordText;
    public Text givenPassword;
    public GameObject canvas;
    public GameObject gateCrasherText;
    public GameObject inputPassword;
    public GameObject psudoPassword;
    public Image tape;
    public Item gateCrasher;

    private GameObject door;
    private DoorTrigger doorTrigger;
    private string password;
    private string input;
    private bool called = false;
    private bool isDoorOpen = false;

    private void Update()
    {
        // If the doorTrigger is not set to anything, leave the canvas and textfield blank
        if (doorTrigger == null)
        {
            inputField.DeactivateInputField();
            canvas.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            inputField.text = "";
            openDoorWithGateCrasher = false;
            FindObjectOfType<Player>().lockInputs = false;
        }

        // If the door is not open, function normally, or else, don't do anything
        else if (!isDoorOpen)
        {
            // If the door need a key or if there are no gate breakers active
            // the door will open normally via key or password
            if (door.GetComponent<GatesObj>().keyNeeded || activeGateBreaker == 0)
            {
                // If the password entered by the player is correct, take the canvas out of view
                // Entered Textfield should not be empty yet at this point
                if (door != null && door.GetComponent<GatesObj>().openObj())
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
                    givenPassword.text = "     Password:     " + password;
                    canvas.SetActive(true);
                    psudoPassword.SetActive(false);
                    inputPassword.SetActive(true);
                    if (door.GetComponent<GatesObj>().passwordGiven) { tape.enabled = false; }
                    //inputField.ActivateInputField();
                    Cursor.lockState = CursorLockMode.Confined;
                    Cursor.visible = true;
                    FindObjectOfType<Player>().lockInputs = true;
                    checkPassword();
                }
            }

            // Otherwise, automatically enters password and open door;
            // then, inform the gate breaker script that the procedure has been completed
            // to decrement the active gate breaker
            else
            {
                password = door.GetComponent<GatesObj>().password;
                canvas.SetActive(true);
                if (door.GetComponent<GatesObj>().passwordGiven) { tape.enabled = false; }
                inputPassword.SetActive(false);
                psudoPassword.SetActive(true);
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
                FindObjectOfType<Player>().lockInputs = true;
                if (!called)
                {
                    StartCoroutine("gateBreakerWaitUI");
                    called = true;
                }
            }
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
            passwordText.color = new Color(255, 255, 255, 255);
        }
        else if (inputLength > passwordLength || !input.Equals(password.Substring(0, inputLength)))
        {
            passwordText.color = Color.red;
        }
        else
        {
            passwordText.color = new Color(255, 255, 255, 255);
        }
    }

    // Sets door and door trigger as an object when player enters the door trigger
    public void hasEntered(GameObject door, DoorTrigger doorTrigger)
    {
        this.door = door;
        this.doorTrigger = doorTrigger;
    }

    // Sets door and door trigger null when player exits the door trigger
    public void hasExited()
    {
        doorTrigger = null;
        door = null;
        isDoorOpen = false;
    }

    // If "esc" is pressed, the canvas is disabled
    public void ifEscPressed()
    {
        canvas.SetActive(false);
        FindObjectOfType<Player>().lockInputs = false;
        hasExited();
    }

    // When WASD is released, the input field is activated
    public void ifWASDReleased()
    {
        inputField.ActivateInputField();
    }

    // The delay for when the password is correct and blinking green
    public IEnumerator waitUI() 
    {
        // Changes color of the text to green and deactivates input field and canvas
        inputField.DeactivateInputField();
        passwordText.color = Color.green;
        yield return new WaitForSeconds(0.25f);
        canvas.SetActive(false);
        passwordText.color = new Color(255, 255, 255, 255);
        FindObjectOfType<Player>().lockInputs = false;
        called = false;
        isDoorOpen = true;
    }

    // The delay for when the gate breaker is used and password blinks green
    public IEnumerator gateBreakerWaitUI()
    {
        // Changes text to hidden password, color of the text to green, and deactivates canvas
        for (int i = 0; i < password.Length; i++)
        {
            psudoPasswordText.text += "*";
            yield return new WaitForSeconds(0.03f);
        }
        openDoorWithGateCrasher = true;
        psudoPasswordText.color = Color.green;
        yield return new WaitForSeconds(0.25f);
        canvas.SetActive(false);
        psudoPasswordText.color = new Color(255, 255, 255, 255);
        psudoPasswordText.text = "";
        FindObjectOfType<Player>().lockInputs = false;
        called = false;
        isDoorOpen = true;
    }
}
