using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PasswordLogic : MonoBehaviour
{

    public InputField InputField;
    public GameObject Door;

    // Start is called before the first frame update
    void Start()
    {
        InputField.ActivateInputField();
    }

    // Records the password the player inputs
    public string readInput(string input)
    {
        return input;
    }

    
}
