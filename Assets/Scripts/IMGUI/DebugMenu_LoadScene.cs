using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugMenu_LoadScene : MonoBehaviour
{
    public float width;
    public float height;
    public string scene;

    void OnGUI() {
        if (GUI.Button(new Rect(Screen.width/2 - width/2, Screen.height/2 - height/2, width, height), "Load Test Scene")) {
            SceneManager.LoadScene(scene);
        }
    }
}
