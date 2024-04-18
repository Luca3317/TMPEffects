using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPEffects.Databases.CommandDatabase;

namespace TMPEffects.Editor
{
    [CustomEditor(typeof(TMPCommandDatabase))]
    public class TMPCommandDatabaseEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("commands"));
            if (serializedObject.hasModifiedProperties) serializedObject.ApplyModifiedProperties();
        }
    }
}

