using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(CharDataModifiers.Rotation))]
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

        // var rotProp = property.FindPropertyRelative("rotation");
        // var quat = rotProp.quaternionValue;
        // var vector = EditorGUI.Vector3Field(rect, "Rotation", new Vector3(quat.x, quat.y, quat.z));
        // if (vector != new Vector3(quat.x, quat.y, quat.z))
        // {
        //     rotProp.quaternionValue = new Quaternion(vector.x, vector.y, vector.z, quat.w);
        // }
        
        
        // var rotProp = property.FindPropertyRelative("rotation");
        // var quat = rotProp.quaternionValue;
        // var vector = EditorGUI.Vector3Field(rect, "Rotation", quat.eulerAngles);
        // if (vector != quat.eulerAngles)
        // {
        //     rotProp.quaternionValue = Quaternion.Euler(vector);
        // }
        
        
        EditorGUI.PropertyField(rect, property.FindPropertyRelative("rotation"));
        
        
        if (property.serializedObject.hasModifiedProperties)
            property.serializedObject.ApplyModifiedProperties();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (property.isExpanded) return EditorGUIUtility.singleLineHeight * 3;
        return EditorGUIUtility.singleLineHeight;
    }
}