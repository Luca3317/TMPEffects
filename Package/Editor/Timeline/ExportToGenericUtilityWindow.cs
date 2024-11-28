using System;
using System.IO;
using UnityEditor;
using UnityEditor.Timeline.Actions;
using UnityEngine;

internal class ExportToGenericUtilityWindow : EditorWindow
{
    private static string directoryPath = "";
    private string assetName = "";
    private GUIStyle fileBrowserButtonStyle;

    private ActionContext context;

    public static void ShowWindow(ActionContext context)
    {
        ExportToGenericUtilityWindow window = CreateInstance<ExportToGenericUtilityWindow>();
        window.titleContent = new GUIContent("Export to GenericAnimation");
        window.minSize = new Vector2(300, 280);
        window.maxSize = new Vector2(800, window.minSize.y);
        window.context = context;
        window.ShowModalUtility();
    }

    private void OnEnable()
    {
        if (string.IsNullOrWhiteSpace(directoryPath) || !Directory.Exists(Path.GetFullPath(directoryPath)))
        {
            directoryPath = Application.dataPath;
        }
    }

    void OnGUI()
    {
        bool mayExport = true;

        GUILayout.Space(15);
        GUILayout.Label("You are exporting to a GenericAnimation asset.", EditorStyles.boldLabel);

        GUILayout.Space(10);
        GUILayout.Label("Where do you want to export to?", EditorStyles.boldLabel);

        fileBrowserButtonStyle = new GUIStyle(GUI.skin.button);
        fileBrowserButtonStyle.normal.background = EditorGUIUtility.IconContent("d_Folder Icon").image as Texture2D;
        fileBrowserButtonStyle.fixedHeight = EditorGUIUtility.singleLineHeight * 1.5f;
        fileBrowserButtonStyle.fixedWidth = EditorGUIUtility.singleLineHeight * 2;

        GUILayout.Space(10);

        var rect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight);
        var labelRect = new Rect(rect.x, rect.y, EditorGUIUtility.labelWidth, rect.height);
        var buttonRect = new Rect(rect.x + labelRect.width,
            rect.y - (fileBrowserButtonStyle.fixedHeight - rect.height) / 2f, fileBrowserButtonStyle.fixedWidth,
            fileBrowserButtonStyle.fixedHeight);
        var textRect = new Rect(rect.x + labelRect.width + 40, rect.y, rect.width - labelRect.width - 40, rect.height);
        EditorGUI.LabelField(labelRect, "Directory");
        if (GUI.Button(buttonRect, "", fileBrowserButtonStyle))
        {
            string path = EditorUtility.OpenFolderPanel("Select File", "", "");
            if (!string.IsNullOrEmpty(path))
            {
                directoryPath = path;
            }
        }

        directoryPath = EditorGUI.TextField(textRect, directoryPath);

        GUILayout.Space(5);
        assetName = EditorGUILayout.TextField("Asset name", assetName);


        if (!string.IsNullOrEmpty(directoryPath))
        {
            string tempPath = Path.GetFullPath(directoryPath);
            if (!Directory.Exists(tempPath))
            {
                mayExport = false;
                EditorGUILayout.HelpBox("Not a valid directory: " + directoryPath, MessageType.Error);
            }
            // else if (!tempPath.StartsWith(Application.dataPath, StringComparison.OrdinalIgnoreCase))
            // {
            //     EditorGUILayout.HelpBox("Directory not contained in this project: " + Application.dataPath + " :" + directoryPath + "/" + assetName, MessageType.Warning);
            // }

            else if (File.Exists(Path.Combine(tempPath, assetName + ".asset")))
            {
                mayExport = false;
                EditorGUILayout.HelpBox(
                    "A file with the specified name and extension already exists: " +
                    Path.Combine(tempPath, assetName + ".asset").ToString(), MessageType.Error);
            }

            else if (string.IsNullOrWhiteSpace(assetName))
            {
                mayExport = false;
                EditorGUILayout.HelpBox("Specify a name for the asset to export to", MessageType.Info);
            }

            else
                EditorGUILayout.HelpBox(
                    "Selected location: " + Path.Combine(Path.GetFullPath(directoryPath), assetName + ".asset"),
                    MessageType.Info);
        }
        else
        {
            mayExport = false;
            EditorGUILayout.HelpBox("Select a directory to export to", MessageType.Info);
        }

        var customButtonStyle = new GUIStyle(GUI.skin.button);
        customButtonStyle.fontSize = 14;
        customButtonStyle.fontStyle = FontStyle.Bold;
        customButtonStyle.fixedHeight = 40;

        GUILayout.Space(10);
        GUILayout.Label("What do you want to export?", EditorStyles.boldLabel);

        exportSelection = (ExportSelection)EditorGUILayout.EnumPopup(exportSelection);

        GUILayout.Space(25);
        EditorGUI.BeginDisabledGroup(!mayExport);
        if (GUILayout.Button("Export", customButtonStyle))
        {
            TimelineUtility.ExportAsGeneric(context, directoryPath, assetName + ".asset", (int)exportSelection);
            Close();
        }

        EditorGUI.EndDisabledGroup();
    }

    public ExportSelection exportSelection;

    public enum ExportSelection
    {
        AllTracks = 2,
        SelectedTracksOnly = 0
    }
}


internal class ExportToTMPAnimationScriptUtilityWindow : EditorWindow
{
    private static string directoryPath = "";
    private string scriptName = "";
    private GUIStyle fileBrowserButtonStyle;

    private ActionContext context;

    public static void ShowWindow(ActionContext context)
    {
        ExportToTMPAnimationScriptUtilityWindow window = CreateInstance<ExportToTMPAnimationScriptUtilityWindow>();
        window.titleContent = new GUIContent("Export to GenericAnimation");
        window.minSize = new Vector2(300, 280);
        window.maxSize = new Vector2(800, window.minSize.y);
        window.context = context;
        window.ShowModalUtility();
    }

    private void OnEnable()
    {
        if (string.IsNullOrWhiteSpace(directoryPath) || !Directory.Exists(Path.GetFullPath(directoryPath)))
        {
            directoryPath = Application.dataPath;
        }
    }

    void OnGUI()
    {
        bool mayExport = true;

        GUILayout.Space(15);
        GUILayout.Label("You are exporting to a TMPAnimation script.", EditorStyles.boldLabel);

        GUILayout.Space(10);
        GUILayout.Label("Where do you want to export to?", EditorStyles.boldLabel);

        fileBrowserButtonStyle = new GUIStyle(GUI.skin.button);
        fileBrowserButtonStyle.normal.background = EditorGUIUtility.IconContent("d_Folder Icon").image as Texture2D;
        fileBrowserButtonStyle.fixedHeight = EditorGUIUtility.singleLineHeight * 1.5f;
        fileBrowserButtonStyle.fixedWidth = EditorGUIUtility.singleLineHeight * 2;

        GUILayout.Space(10);

        var rect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight);
        var labelRect = new Rect(rect.x, rect.y, EditorGUIUtility.labelWidth, rect.height);
        var buttonRect = new Rect(rect.x + labelRect.width,
            rect.y - (fileBrowserButtonStyle.fixedHeight - rect.height) / 2f, fileBrowserButtonStyle.fixedWidth,
            fileBrowserButtonStyle.fixedHeight);
        var textRect = new Rect(rect.x + labelRect.width + 40, rect.y, rect.width - labelRect.width - 40, rect.height);
        EditorGUI.LabelField(labelRect, "Directory");
        if (GUI.Button(buttonRect, "", fileBrowserButtonStyle))
        {
            string path = EditorUtility.OpenFolderPanel("Select File", "", "");
            if (!string.IsNullOrEmpty(path))
            {
                directoryPath = path;
            }
        }

        directoryPath = EditorGUI.TextField(textRect, directoryPath);

        GUILayout.Space(5);
        scriptName = EditorGUILayout.TextField("Script name", scriptName);


        if (!string.IsNullOrEmpty(directoryPath))
        {
            string tempPath = Path.GetFullPath(directoryPath);
            if (!Directory.Exists(tempPath))
            {
                mayExport = false;
                EditorGUILayout.HelpBox("Not a valid directory: " + directoryPath, MessageType.Error);
            }
            // else if (!tempPath.StartsWith(Application.dataPath, StringComparison.OrdinalIgnoreCase))
            // {
            //     EditorGUILayout.HelpBox("Directory not contained in this project: " + Application.dataPath + " :" + directoryPath + "/" + assetName, MessageType.Warning);
            // }

            else if (File.Exists(Path.Combine(tempPath, scriptName + ".cs")))
            {
                mayExport = false;
                EditorGUILayout.HelpBox(
                    "A file with the specified name and extension already exists: " +
                    Path.Combine(tempPath, scriptName + ".cs").ToString(), MessageType.Error);
            }

            else if (string.IsNullOrWhiteSpace(scriptName))
            {
                mayExport = false;
                EditorGUILayout.HelpBox("Specify a name for the script to export to", MessageType.Info);
            }

            else
                EditorGUILayout.HelpBox(
                    "Selected location: " + Path.Combine(Path.GetFullPath(directoryPath), scriptName + ".cs"),
                    MessageType.Info);
        }
        else
        {
            mayExport = false;
            EditorGUILayout.HelpBox("Select a directory to export to", MessageType.Info);
        }

        var customButtonStyle = new GUIStyle(GUI.skin.button);
        customButtonStyle.fontSize = 14;
        customButtonStyle.fontStyle = FontStyle.Bold;
        customButtonStyle.fixedHeight = 40;

        GUILayout.Space(10);
        GUILayout.Label("What do you want to export?", EditorStyles.boldLabel);

        exportSelection = (ExportSelection)EditorGUILayout.EnumPopup(exportSelection);

        GUILayout.Space(25);
        EditorGUI.BeginDisabledGroup(!mayExport);
        if (GUILayout.Button("Export", customButtonStyle))
        {
            TimelineUtility.ExportAsScript(context, directoryPath, scriptName, (int)exportSelection);
            Close();
        }

        EditorGUI.EndDisabledGroup();
    }

    public ExportSelection exportSelection;

    public enum ExportSelection
    {
        AllTracks = 2,
        SelectedTracksOnly = 0
    }
}