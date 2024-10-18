using TMPEffects.Parameters;
using UnityEditor;
using UnityEngine;
using GUIContent = UnityEngine.GUIContent;

[CustomPropertyDrawer(typeof(ParameterTypes.TypedVector3))]
public class TypedVector3Drawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        TypedVectorDrawerUtility.Draw(position, property, label, 100);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight;
    }
}

[CustomPropertyDrawer(typeof(ParameterTypes.TypedVector2))]
public class TypedVector2Drawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        TypedVectorDrawerUtility.Draw(position, property, label, 100);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight;
    }
}

internal static class TypedVectorDrawerUtility
{
    public static void Draw(Rect position, SerializedProperty property, GUIContent label, float toggleWidth)
    {
        position = EditorGUI.PrefixLabel(position, label);
        EditorGUI.PropertyField(position, property.FindPropertyRelative("vector"), GUIContent.none);
        
        position.x -= toggleWidth;
        position.width = toggleWidth;
        
        property.FindPropertyRelative("type").enumValueIndex = (int)(ParameterTypes.VectorType)
            EditorGUI.EnumPopup(position, GUIContent.none,(ParameterTypes.VectorType)property.FindPropertyRelative("type").enumValueIndex);
    }
}