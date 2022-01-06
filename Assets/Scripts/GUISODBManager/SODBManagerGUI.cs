using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

[ExecuteInEditMode]
public class SODBManagerGUI : EditorWindow
{
    public VisualTreeAsset uxml;
    private VisualElement root;

    // TODO: this window should give is a view of what the current objects are that are
    // loaded into the DB. Double clicking any entry will open the inspector to that
    // specific SO in the project. Relies on same data object that the validator &
    // compiler will eventually rely on to check for broken references.

    [MenuItem("Window/SODB Manager")]
    public static void ShowWindow() {
        SODBManagerGUI wnd = GetWindow<SODBManagerGUI>();
        wnd.titleContent = new GUIContent("SODB Manager");
    }

    public void CreateGUI() {
        // Each editor window contains a root VisualElement object
        root = rootVisualElement;

        // Generate GUI tree.
        uxml.CloneTree(root);

        // Register callbacks.
        root.Q<Button>("btn_RefreshSODB").clicked += onRefreshDBClicked;
    }

    // Reload DB from SOs found in project.
    public void onRefreshDBClicked() {
        SODB.InitLibs();
    }
}
