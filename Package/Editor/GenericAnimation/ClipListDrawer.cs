using System.Collections;
using System.Collections.Generic;
using TMPEffects.TMPAnimations;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(GenericAnimation.ClipList))]
public class ClipListDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.PropertyField(position, property.FindPropertyRelative("Clips"));
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var prop = property.FindPropertyRelative("Clips");
        if (prop.isExpanded)
        {
            return EditorGUI.GetPropertyHeight(property.FindPropertyRelative("Clips"), true);
        }
        else
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}