using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugMenu_LoadScene : MonoBehaviour
{
    public float width;
    public float height;

    void OnGUI() {
        if (GUI.Button(new Rect(Screen.width/2 - width/2, Screen.height/2 - height/2, width, height), "Load Test Scene")) {
            SceneManager.LoadScene("SampleScene");
        }

        if (GUI.Button(new Rect(Screen.width/2f - width/2f, Screen.height/2f - height/2f + height + 10f, width, height), "Load Event Editor Scene")) {
            SceneManager.LoadScene("EventEditorScene");
        }
    }
}
