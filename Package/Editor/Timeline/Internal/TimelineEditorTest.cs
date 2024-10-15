using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[EditorWindowTitle(title = "TMPEffects Animation Creator", useTypeNameAsIconName = true)]
internal class TimelineEditorTest : TimelineWindow
{
    [MenuItem ("Window/My Window")]
    public static void ShowWindow () {
        EditorWindow.GetWindow(typeof(TimelineEditorTest));
    }

    public void OnGUI()
    {
        GUILayout.Button("WOW");
    }
}
