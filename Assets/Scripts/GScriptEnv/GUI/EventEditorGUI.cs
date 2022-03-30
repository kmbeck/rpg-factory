using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;
using System.IO;

public class EventEditorGUI : EditorWindow
{
    public VisualTreeAsset uxml;
    private VisualElement root;

    //TODO: Variablize filepaths in this script!
    //TODO: listview event library not using unityFont I am setting???

    [MenuItem("Window/GScript/Event Editor")]
    public static void ShowWindow()
    {
        EventEditorGUI wnd = GetWindow<EventEditorGUI>();
        wnd.titleContent = new GUIContent("Event Editor");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        root = rootVisualElement;

        // Generate GUI tree.
        uxml.CloneTree(root);

        // Register callbacks.
        root.Q<Button>("btn_SaveScript").clicked += onSaveClicked;
        root.Q<Button>("btn_ValidateScript").clicked += onValidateClicked;
        root.Q<Button>("btn_CompileAllScripts").clicked += onComipleAllClicked;
        root.Q<Button>("btn_Load").clicked += onLoadClicked;
        root.Q<Button>("btn_Refresh").clicked += updateEventLibraryView;

        // Populate the Event Library view.
        updateEventLibraryView();

    }

    // Save current editor fields to a scriptable object named <uniqueID>.asset
    public void onSaveClicked() {
        // TODO: validation will go here before any saving is done...
        SOEvent newEvent =  (SOEvent)SOEvent.CreateInstance(typeof(SOEvent));
        newEvent.uniqueID = root.Q<TextField>("tf_UniqueID").value;
        newEvent.triggerFlagUniqueID = root.Q<TextField>("tf_FlagTrigger").value;
        newEvent.parallel = root.Q<Toggle>("chk_Parallel").value;
        newEvent.script = root.Q<TextField>("tf_EventScript").value;
        AssetDatabase.CreateAsset(newEvent, $"Assets/Resources/SOs/Events/{newEvent.uniqueID}.asset");
        updateEventLibraryView();
    }

    // Validate the current contents of the Event Script text field.
    public void onValidateClicked() {
        GScriptCompiler compiler = new GScriptCompiler();
        compiler.validate(root.Q<TextField>("tf_EventScript").value);
    }

    public void onComipleAllClicked() {
        GScriptCompiler compiler = new GScriptCompiler();
        compiler.compileAllEvents();
    }

    public void onLoadClicked() {
        if (root.Q<ListView>("listview_EventLibrary") == null) {
            return;
        }
        string selectedVal = (root.Q<ListView>("listview_EventLibrary").selectedItem.ToString());
        if (SODB.LIB_EVENT.lib.ContainsKey(selectedVal)) {
            SOEvent evt = SODB.LIB_EVENT.lib[selectedVal];
            root.Q<TextField>("tf_UniqueID").value = evt.uniqueID;
            root.Q<TextField>("tf_FlagTrigger").value = evt.triggerFlagUniqueID;
            root.Q<Toggle>("chk_Parallel").value = evt.parallel;
            root.Q<TextField>("tf_EventScript").value = evt.script;
        }
    }

    // Initialize event library view for the first time when the window is opened.
    public void updateEventLibraryView() {
        // Start by refreshing event library in SODB.
        SODB.InitEventLib();

        // Remove pre-existing list view if it exists.
        if (root.Q<VisualElement>("listview_EventLibrary") != null) {
            root.Q<VisualElement>("container_EventListView").Remove(root.Q<VisualElement>("listview_EventLibrary"));
        }

        // Get list of event SOs.
        string topDir = Directory.GetCurrentDirectory();

        string[] files = Directory.GetFiles(
            topDir + "/Assets/Resources/SOs/Events/", "*.asset", SearchOption.TopDirectoryOnly
        );

        for (int i = 0; i < files.Length; i++) {
            files[i] = files[i].Split('/')[files[i].Split('/').Length - 1].Split('.')[0];
        }

        Func<VisualElement> makeItem = createEventLibraryLabel;
        Action<VisualElement, int> bindItem = (e, i) => (e as Label).text = files[i];
        const int itemHeight = 18;
        var listView = new ListView(files, itemHeight, makeItem, bindItem);
        listView.name = "listview_EventLibrary";
        listView.selectionType = SelectionType.Single;
        listView.showAlternatingRowBackgrounds = AlternatingRowBackground.All;
        listView.style.flexGrow = 1.0f;
        listView.style.fontSize = 14;
        listView.style.unityFont = AssetDatabase.LoadAssetAtPath<Font>("Assets/Resources/Fonts/Source Code Pro");
        listView.onItemChosen += obj => onLoadClicked();
        root.Q<VisualElement>("container_EventListView").Add(listView);
        listView.PlaceBehind(root.Q<VisualElement>("container_EditorFooterButtonsRight"));
    }

    // Helps construct Event Library View.
    private Label createEventLibraryLabel() {
        Label newLabel = new Label();
        newLabel.style.unityFont = AssetDatabase.LoadAssetAtPath<Font>("Assets/Resources/Fonts/Source Code Pro");
        return newLabel;
    }
}