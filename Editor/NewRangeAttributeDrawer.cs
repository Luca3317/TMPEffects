using UnityEngine;
using UnityEditor;
using System;

[CustomPropertyDrawer(typeof(NewRangeAttribute))]
public class NewRangeAttributeDrawer : PropertyDrawer
{
    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // First get the attribute since it contains the range for the slider
        NewRangeAttribute range = attribute as NewRangeAttribute;

        // Now draw the property as a Slider or an IntSlider based on whether it's a float or integer.
        if (property.propertyType == SerializedPropertyType.Float)
            EditorGUI.Slider(position, property, range.min, range.max, label);
        else if (property.propertyType == SerializedPropertyType.Integer)
            EditorGUI.IntSlider(position, property, Convert.ToInt32(range.min), Convert.ToInt32(range.max), label);

        else if (property.propertyType == SerializedPropertyType.Vector3)
        {
            Vector3 value = EditorGUI.Vector3Field(position, label, property.vector3Value);
            value = new Vector3(Mathf.Clamp(value.x, range.min, range.max), Mathf.Clamp(value.y, range.min, range.max), Mathf.Clamp(value.z, range.min, range.max));
            property.vector3Value = value;
        }
        else if (property.propertyType == SerializedPropertyType.Vector3Int)
        {
            Vector3Int value = EditorGUI.Vector3IntField(position, label, property.vector3IntValue);
            value = new Vector3Int((int)Mathf.Clamp(value.x, range.min, range.max), (int)Mathf.Clamp(value.y, range.min, range.max), (int)Mathf.Clamp(value.z, range.min, range.max));
            property.vector3IntValue = value;
        }
        else
            EditorGUI.LabelField(position, label.text, "Use Range with float or int.");
    }
}