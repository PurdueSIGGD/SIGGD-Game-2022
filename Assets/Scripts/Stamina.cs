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

    public setStamina(int s) {
        stamina = s;
        if (stamina > 100) {
            stamina = 100;
        }
    }

    public getStamina() {
        return stamina;
    }

    public addStamina(int s) {
        stamina += s;
        if (stamina > 100) {
            stamina = 100;
        }
    }
    
    public subtractStamia(int s) {
        stamina -= s;
        if (stamina < 0) {
            stamina = 0;
        }
    }
}
