using TMPEffects.Databases;
using UnityEditor;

[CustomEditor(typeof(TMPHideAnimationDatabase))]
public class TMPHideAnimationDatabaseEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUILayout.PropertyField(serializedObject.FindProperty("hideAnimations"));
        if (serializedObject.hasModifiedProperties) serializedObject.ApplyModifiedProperties();
    }
}