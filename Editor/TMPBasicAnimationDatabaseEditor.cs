using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TMPBasicAnimationDatabase))]
public class TMPBasicAnimationDatabaseEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUILayout.PropertyField(serializedObject.FindProperty("animations"));
        if (serializedObject.hasModifiedProperties) serializedObject.ApplyModifiedProperties();
    }
}
