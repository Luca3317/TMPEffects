using System.Collections;
using System.Collections.Generic;
using TMPEffects.Modifiers;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Rotation))]
public class RotationDrawer : PropertyDrawer
{
    private static readonly GUIContent pivotGUI = new GUIContent("Pivot", "The pivot of the rotation.");
    private static readonly GUIContent eulerGUI = new GUIContent("Euler", "The rotation as euler angles.");

    private SerializedProperty pivotProperty;
    private SerializedProperty eulerAnglesProperty;

    private void Init(SerializedProperty property)
    {
        pivotProperty ??= property.FindPropertyRelative("pivot");
        eulerAnglesProperty ??= property.FindPropertyRelative("eulerAngles");
    }
    
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        Init(property);
        var rect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

        property.isExpanded = EditorGUI.Foldout(rect, property.isExpanded, label);
        rect.y += EditorGUIUtility.singleLineHeight;

        if (!property.isExpanded) return;

        EditorGUI.PropertyField(rect, pivotProperty, pivotGUI);
        rect.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(rect, eulerAnglesProperty, eulerGUI);

        if (property.serializedObject.hasModifiedProperties)
            property.serializedObject.ApplyModifiedProperties();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (property.isExpanded) return EditorGUIUtility.singleLineHeight * 3;
        return EditorGUIUtility.singleLineHeight;
    }
}