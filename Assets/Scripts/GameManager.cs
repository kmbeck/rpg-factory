using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager inst;

    // Start is called before the first frame update
    void Start()
    {   
        if (inst != null) {
            Destroy(this.gameObject);
        }
        else {
            inst = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }
}
