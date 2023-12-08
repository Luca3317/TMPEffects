using TMPEffects.Databases;
using UnityEditor;

[CustomEditor(typeof(TMPBasicAnimationDatabase))]
public class TMPBasicAnimationDatabaseEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUILayout.PropertyField(serializedObject.FindProperty("animations"));
        if (serializedObject.hasModifiedProperties) serializedObject.ApplyModifiedProperties();
    }
}
