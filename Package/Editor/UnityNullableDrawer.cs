using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(UnityNullable<>), false)]
public class UnityNullableDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        // Calculate rects
        var hasValueRect = new Rect(position.x, position.y, 30, position.height);
        var valueRect = new Rect(position.x + 35, position.y, position.width - 35, position.height);

        // Draw fields - pass GUIContent.none to each so they are drawn without labels
        EditorGUI.PropertyField(hasValueRect, property.FindPropertyRelative("hasValue"), GUIContent.none);

        if (property.FindPropertyRelative("hasValue").boolValue)
        {
            EditorGUI.PropertyField(valueRect, property.FindPropertyRelative("value"), GUIContent.none);
        }
        else
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.PropertyField(valueRect, property.FindPropertyRelative("value"), GUIContent.none);
            EditorGUI.EndDisabledGroup();
        }

        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight;
    }
}