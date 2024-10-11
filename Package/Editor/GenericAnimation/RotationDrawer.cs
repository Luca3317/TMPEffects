using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Rotation))]
public class RotationDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var rect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

        property.isExpanded = EditorGUI.Foldout(rect, property.isExpanded, label);
        rect.y += EditorGUIUtility.singleLineHeight;

        if (!property.isExpanded) return;

        EditorGUI.PropertyField(rect, property.FindPropertyRelative("pivot"));
        rect.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(rect, property.FindPropertyRelative("eulerAngles"));

        if (property.serializedObject.hasModifiedProperties)
            property.serializedObject.ApplyModifiedProperties();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (property.isExpanded) return EditorGUIUtility.singleLineHeight * 3;
        return EditorGUIUtility.singleLineHeight;
    }
}