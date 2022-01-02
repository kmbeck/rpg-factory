using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EventEditorGUI : EditorWindow
{
    // Window code.
    void OnGUI() {
        GUILayout.Label("Unique ID: ",EditorStyles.boldLabel);
        string inUniqueID = EditorGUILayout.TextField("Name", "Unique ID");
    }

    [MenuItem("Window/Event Editor")]
    public static void ShowWindow() {
        EditorWindow.GetWindow<EventEditorGUI>("Event Editor");
    }
}
