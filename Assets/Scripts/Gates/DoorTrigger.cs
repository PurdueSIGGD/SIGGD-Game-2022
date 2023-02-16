using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    public GameObject doorFrame;
    public GameObject door;
    private float moveSpeed = 8f;
    private bool isPasswordCorrect = false;
    private bool isDoorOpen = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isPasswordCorrect)
        {
            doorFrame.transform.localPosition = Vector3.MoveTowards(doorFrame.transform.localPosition, 
                new Vector3(0, 3.5f, 0), moveSpeed * Time.deltaTime);
            isDoorOpen = true;
        }
        else if (!isPasswordCorrect)
        {
            doorFrame.transform.localPosition = Vector3.MoveTowards(doorFrame.transform.localPosition,
                new Vector3(0, 0, 0), moveSpeed * Time.deltaTime);
            isDoorOpen = false;
        }
    }

    // Activates when player enters trigger
    private void OnTriggerEnter(Collider other)
    {
        // Activate password panel
        FindObjectOfType<PasswordLogic>().hasEntered(this);
    }

    // Activates when player leaves trigger
    public void OnTriggerExit(Collider other)
    {
        // Close door frame and deactivate password panel
        isPasswordCorrect = false;
        FindObjectOfType<PasswordLogic>().hasExited();
    }

    // Physically opens door frame
    public void openDoorFrame()
    {
        for (int i = 0; i < 10; i++)
        {
            doorFrame.transform.position = doorFrame.transform.position + (Vector3.up * moveSpeed) * Time.deltaTime;
        }
    }

    // testing if door open works
    [ContextMenu("Change 'isPassword Correct'")]
    public void testPass()
    {
        if (isPasswordCorrect) isPasswordCorrect = false;
        if (!isPasswordCorrect) isPasswordCorrect = true;
    }
}
