using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AnimationStackObject))]
public class AnimationStackObjectEditor : Editor
{
    SerializedProperty animsProp;

    void OnEnable()
    {
        animsProp = serializedObject.FindProperty("stack").FindPropertyRelative("animations");
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.PropertyField(animsProp);

        if (serializedObject.hasModifiedProperties)
        {
            serializedObject.ApplyModifiedProperties();
        }
    }
}
