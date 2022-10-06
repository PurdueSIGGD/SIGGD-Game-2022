using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stamina : MonoBehaviour
{
    private int stamina;
    // Start is called before the first frame update
    void Start()
    {
        stamina = 100;
    }

    public void setStamina(int s) {
        stamina = s;
        if (stamina > 100) {
            stamina = 100;
        }
    }

    public int getStamina() {
        return stamina;
    }

    public void addStamina(int s) {
        stamina += s;
        if (stamina > 100) {
            stamina = 100;
        }
    }
    
    public void subtractStamia(int s) {
        stamina -= s;
        if (stamina < 0) {
            stamina = 0;
        }
    }
}
