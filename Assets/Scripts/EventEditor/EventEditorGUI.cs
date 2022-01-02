using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

public class EventEditorGUI : EditorWindow
{
    public VisualTreeAsset uxml;

    [MenuItem("Window/Event Editor")]
    public static void ShowExample()
    {
        EventEditorGUI wnd = GetWindow<EventEditorGUI>();
        wnd.titleContent = new GUIContent("Event Editor");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        uxml.CloneTree(root);
    }
}