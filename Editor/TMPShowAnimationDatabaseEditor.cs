using TMPEffects.Databases;
using UnityEditor;

[CustomEditor(typeof(TMPShowAnimationDatabase))]
public class TMPShowAnimationDatabaseEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUILayout.PropertyField(serializedObject.FindProperty("showAnimations"));
        if (serializedObject.hasModifiedProperties) serializedObject.ApplyModifiedProperties();
    }
}