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
        FlagEditorGUI wnd = GetWindow<FlagEditorGUI>();
        wnd.titleContent = new GUIContent("Flag Editor");
    }

    public void CreateGUI() {
        root = rootVisualElement;

        // Each editor window contains a root VisualElement object
        root = rootVisualElement;

        // Generate GUI tree.
        uxml.CloneTree(root);

        // Register callbacks.
        root.Q<Button>("btn_Save").clicked += onSaveClicked;
        root.Q<Button>("btn_Load").clicked += onLoadClicked;

        // Populate enum field with options.
        EnumField enumField = root.Q<EnumField>("enum_DataType");
        enumField.Init(FlagDataType.INT);

        // Populate the Flag Library view.
        updateFlagLibraryView();
    }

    // Save the data in the from as a new Flag SO. Will only write flag with data
    // for the specified dataType.
    public void onSaveClicked() {
        SOFlag newFlag = (SOFlag)SOEvent.CreateInstance(typeof(SOFlag));
        newFlag.uniqueID = root.Q<TextField>("tf_UniqueID").value;
        newFlag.dataType = (FlagDataType)root.Q<EnumField>("enum_DataType").value;
        newFlag.iVal = SOFlag.I_DEFAULT;
        newFlag.sVal = SOFlag.S_DEFAULT;
        newFlag.bVal = SOFlag.B_DEFAULT;
        newFlag.fVal = SOFlag.F_DEFAULT;

        switch (newFlag.dataType) {
            case FlagDataType.INT:
                newFlag.iVal = root.Q<IntegerField>("int_IntVal").value;
                break;
            case FlagDataType.STRING:
                newFlag.sVal = root.Q<TextField>("tf_StrVal").value;
                break;
            case FlagDataType.BOOL:
                newFlag.bVal = root.Q<Toggle>("chk_BoolVal").value;
                break;
            case FlagDataType.FLOAT:
                newFlag.fVal = root.Q<FloatField>("float_FloatVal").value;
                break;
        }

        AssetDatabase.CreateAsset(newFlag, $"Assets/Resources/SOs/Flags/{newFlag.uniqueID}.asset");
        updateFlagLibraryView();
    }

    public void onLoadClicked() {
        if (root.Q<ListView>("listview_FlagLibrary") == null) {
            return;
        }

        string selectedVal = (root.Q<ListView>("listview_FlagLibrary").selectedItem.ToString());
        if (SODB.LIB_FLAG.lib.ContainsKey(selectedVal)) {
            SOFlag flag = SODB.LIB_FLAG.lib[selectedVal];
            root.Q<TextField>("tf_UniqueID").value = flag.uniqueID;
            root.Q<EnumField>("enum_DataType").value = flag.dataType;
            root.Q<IntegerField>("int_IntVal").value = flag.iVal;
            root.Q<TextField>("tf_StrVal").value = flag.sVal;
            root.Q<Toggle>("chk_BoolVal").value = flag.bVal;
            root.Q<FloatField>("float_FloatVal").value = flag.fVal;
        }
    }

    public void onTypeSelectionChanged() {
        // TODO: when the player selects a type, reset and disable all fields that were
        // not equal to that type.
    }

    public void updateFlagLibraryView() {
        // Start by refreshing event library in SODB.
        SODB.InitFlagLib();

        // Remove pre-existing list view if it exists.
        if (root.Q<VisualElement>("listview_FlagLibrary") != null) {
            root.Q<VisualElement>("container_FlagListView").Remove(root.Q<VisualElement>("listview_FlagLibrary"));
        }

        // Get list of event SOs.
        string topDir = Directory.GetCurrentDirectory();
        string[] files = Directory.GetFiles(
            topDir + "/Assets/Resources/SOs/Flags/", "*.asset", SearchOption.TopDirectoryOnly
        );

        for (int i = 0; i < files.Length; i++) {
            files[i] = files[i].Split('/')[files[i].Split('/').Length - 1].Split('.')[0];
        }

        Func<VisualElement> makeItem = createFlagLibraryLabel;
        Action<VisualElement, int> bindItem = (e, i) => (e as Label).text = files[i];
        const int itemHeight = 18;
        var listView = new ListView(files, itemHeight, makeItem, bindItem);
        listView.name = "listview_FlagLibrary";
        listView.selectionType = SelectionType.Single;
        listView.showAlternatingRowBackgrounds = AlternatingRowBackground.All;
        listView.style.flexGrow = 1.0f;
        listView.style.fontSize = 14;
        listView.style.unityFont = AssetDatabase.LoadAssetAtPath<Font>("Assets/Resources/Fonts/SourceCodePro-Regular.ttf");
        //listView.onItemChosen += obj => onLoadClicked();
        root.Q<VisualElement>("container_FlagListView").Add(listView);
        listView.PlaceBehind(root.Q<VisualElement>("container_FooterButtonsRight"));
    }

    private Label createFlagLibraryLabel() {
        Label newLabel = new Label();
        newLabel.style.unityFont = AssetDatabase.LoadAssetAtPath<Font>("Assets/Resources/Fonts/SourceCodePro-Regular.ttf");
        return newLabel;
    }
}