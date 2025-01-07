using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TMPEffects.Parameters;
using TMPEffects.TMPAnimations;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(OffsetTypePowerEnum))]
public class OffsetTypeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        
        var enumProp = property.FindPropertyRelative("enumValue");
        var customProp = property.FindPropertyRelative("customValue");
        var useCustomProp = property.FindPropertyRelative("useCustom");

        var rect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

        var width = EditorStyles.label.CalcSize(label);
        var foldoutRect = new Rect(rect.x, rect.y, 20, EditorGUIUtility.singleLineHeight);
        var ctrlRect = EditorGUI.PrefixLabel(rect, GUIUtility.GetControlID(FocusType.Passive, rect), label);


        List<string> options = new List<string>(enumProp.enumDisplayNames);
        options.Add("Custom");
        
        int selectedIndex = -1;
        if (!useCustomProp.boolValue)
            selectedIndex = enumProp.enumValueIndex;
        else selectedIndex = options.Count - 1;

        
        int index;
        int indent = EditorGUI.indentLevel;
        for (int i = 0; i < indent; i++) EditorGUI.indentLevel--;

        // Custom
        if (selectedIndex == options.Count - 1)
        {
            var ctrlRectWidth = ctrlRect.width;
            ctrlRect.width *= 0.25f;
            var ctrlRect2 = new Rect(ctrlRect.x + ctrlRect.width, ctrlRect.y, ctrlRectWidth * 0.75f, ctrlRect.height);
            index = EditorGUI.Popup(ctrlRect, selectedIndex, options.ToArray());
            EditorGUI.PropertyField(ctrlRect2, customProp, GUIContent.none);
            // customProp.objectReferenceValue = EditorGUI.ObjectField(ctrlRect2, customProp.objectReferenceValue,
            //     type, !EditorUtility.IsPersistent(property.serializedObject.targetObject));
        }
        // Normal
        else
        {
            index = EditorGUI.Popup(ctrlRect, selectedIndex, options.ToArray());
            if (index < options.Count - 1)
                enumProp.enumValueIndex = index;
        }
        
        useCustomProp.boolValue = index == options.Count - 1;
        
        for (int i = 0; i < indent; i++) EditorGUI.indentLevel++;

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight;
    }
}