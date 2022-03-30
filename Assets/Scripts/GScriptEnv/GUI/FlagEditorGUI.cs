using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.IO;

public class FlagEditorGUI : EditorWindow
{
    public VisualTreeAsset uxml;
    private VisualElement root;

    [MenuItem("Window/GScript/Flag Editor")]
    public static void ShowWindow() {
        EventEditorGUI wnd = GetWindow<EventEditorGUI>();
        wnd.titleContent = new GUIContent("Flag Editor");
    }

    public void CreateGUI() {
        root = rootVisualElement;

        // Each editor window contains a root VisualElement object
        root = rootVisualElement;

        // Generate GUI tree.
        uxml.CloneTree(root);

        // Register callbacks.

        // Populate the Flag Library view.
    }

    public void updateFlagLibraryView() {
        // Start by refreshing event library in SODB.
        SODB.Init();


    }
}
