using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(PowerEnum<,>), true)]
public class PowerEnumDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var enumProp = property.FindPropertyRelative("enumValue");
        var customProp = property.FindPropertyRelative("customValue");
        var useCustomProp = property.FindPropertyRelative("useCustom");

        List<string> options = new List<string>(enumProp.enumDisplayNames);
        options.Add("Custom");

        int selectedIndex = -1;
        if (!useCustomProp.boolValue)
            selectedIndex = enumProp.enumValueIndex;
        else selectedIndex = options.Count - 1;

        var rect = new Rect(position.x, position.y, position.width, position.height);
        var ctrlRect = EditorGUI.PrefixLabel(rect, label);

        int index;

        // Custom
        if (selectedIndex == options.Count - 1)
        {
            var width = ctrlRect.width;
            ctrlRect.width *= 0.25f;
            var ctrlRect2 = new Rect(ctrlRect.x + ctrlRect.width, ctrlRect.y, width * 0.75f, ctrlRect.height);
            index = EditorGUI.Popup(ctrlRect, selectedIndex, options.ToArray());
            EditorGUI.PropertyField(ctrlRect2, customProp, GUIContent.none);
        } 
        // Normal
        else
        {
            index = EditorGUI.Popup(ctrlRect, selectedIndex, options.ToArray());
            if (index < options.Count - 1)
                enumProp.enumValueIndex = index;
        }

        useCustomProp.boolValue = index == options.Count - 1;

        EditorGUI.EndProperty();
    }
}