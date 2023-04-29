using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    public GameObject doorFrame;
    public GameObject door;
    private float moveSpeed = 8f;
    private bool isPasswordCorrect = false;
    private bool isPlayerNextToDoor = false;

    // Update is called once per frame
    void Update()
    {
        if (isPlayerNextToDoor) getPassword();
        if (isPasswordCorrect)
        {
            doorFrame.transform.localPosition = Vector3.MoveTowards(doorFrame.transform.localPosition, 
                new Vector3(0, 3.5f, 0), moveSpeed * Time.deltaTime);
        }
        else if (!isPasswordCorrect)
        {
            doorFrame.transform.localPosition = Vector3.MoveTowards(doorFrame.transform.localPosition,
                new Vector3(0, 0, 0), moveSpeed * Time.deltaTime);
        }
    }

    // Activates when player enters trigger
    private void OnTriggerEnter(Collider other)
    {
        // Activate password panel
        if (other.tag == "Player")
        {
            if (!door.GetComponent<GatesObj>().keyNeeded) FindObjectOfType<PasswordLogic>().hasEntered(door, this);
            isPlayerNextToDoor = true;
        }
    }

    // Activates when player leaves trigger
    public void OnTriggerExit(Collider other)
    {
        // Close door frame and deactivate password panel
        if (other.tag == "Player")
        {
            isPasswordCorrect = false;
            FindObjectOfType<PasswordLogic>().hasExited();
            isPlayerNextToDoor = false;
        }
    }

    // Physically opens door frame
    public void openDoorFrame()
    {
        for (int i = 0; i < 10; i++)
        {
            doorFrame.transform.position = doorFrame.transform.position + (Vector3.up * moveSpeed) * Time.deltaTime;
        }
    }

    // Get whether the password entered is correct or not
    public void getPassword()
    {
        isPasswordCorrect = door.GetComponent<GatesObj>().openObj() || FindObjectOfType<PasswordLogic>().openDoorWithGateCrasher;
    }
}
