using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PulseScanner : Item
{

    [SerializeField] public float scanDuration = 10f;
    GameObject[] enemies = null;
    static int active_count = 0;


    public override void Use()
    {
        Debug.Log("SCAN USING");
        active_count++;
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        Debug.Log("SCAN ENEMIES FOUND: " + enemies.Length);

        if (active_count == 1)
        {
            foreach (GameObject enemy in enemies)
            {
                enemy.transform.GetChild(1).GetChild(1).gameObject.SetActive(true);
                Debug.Log("ICON ON: " + enemy.transform.GetChild(1).GetChild(1).gameObject.name);
            }
        }
    }

    public override void End()
    {
        if (active_count == 1)
        {
            foreach (GameObject enemy in enemies)
            {
                enemy.transform.GetChild(1).GetChild(1).gameObject.SetActive(false);
            }
        }
        active_count--;
    }

}
