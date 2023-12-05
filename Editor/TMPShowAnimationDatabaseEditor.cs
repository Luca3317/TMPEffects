using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TMPShowAnimationDatabase))]
public class TMPShowAnimationDatabaseEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUILayout.PropertyField(serializedObject.FindProperty("showAnimations"));
        if (serializedObject.hasModifiedProperties) serializedObject.ApplyModifiedProperties();
    }
}