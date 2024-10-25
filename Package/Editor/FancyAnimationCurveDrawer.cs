using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using AnimationUtility = TMPEffects.TMPAnimations.AnimationUtility;

[CustomPropertyDrawer(typeof(AnimationUtility.FancyAnimationCurve))]
public class FancyAnimationCurveDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var rect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

        var width = EditorStyles.label.CalcSize(label);
        var ctrlRect = EditorGUI.PrefixLabel(rect, GUIUtility.GetControlID(FocusType.Passive, rect), label);
        var foldoutRect = new Rect(rect.x + width.x + 15, rect.y, 20, EditorGUIUtility.singleLineHeight);

        var curveProp = property.FindPropertyRelative("curve");

        curveProp.isExpanded = EditorGUI.Foldout(foldoutRect, curveProp.isExpanded, new GUIContent(""));
        
        // TODO Strange ass workaround for offset created by EditorGUI.PrefixLabel
        EditorGUI.indentLevel--;
        EditorGUI.indentLevel--;
        EditorGUI.PropertyField(ctrlRect, curveProp, new GUIContent(""));
        EditorGUI.indentLevel++; 
        EditorGUI.indentLevel++;

        if (curveProp.isExpanded)
        {
            EditorGUI.indentLevel++; 
            rect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("offsetType"));
            rect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("wrapMode"));
            rect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("uniformity"));
            EditorGUI.indentLevel--;
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var curveProp = property.FindPropertyRelative("curve");
        if (curveProp.isExpanded)
        {
            return EditorGUIUtility.singleLineHeight * 4;
        }
        
        return EditorGUIUtility.singleLineHeight;
    }
}