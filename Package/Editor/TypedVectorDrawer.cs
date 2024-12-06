using TMPEffects.Parameters;
using UnityEditor;
using UnityEngine;
using GUIContent = UnityEngine.GUIContent;

[CustomPropertyDrawer(typeof(TMPParameterTypes.TypedVector3))]
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

[CustomPropertyDrawer(typeof(TMPParameterTypes.TypedVector2))]
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

        int indent = EditorGUI.indentLevel;
        for (int i = 0; i < indent; i++) EditorGUI.indentLevel--;

        EditorGUI.PropertyField(position, property.FindPropertyRelative("vector"), GUIContent.none);

        position.x -= toggleWidth + 5;
        position.width = toggleWidth;

        for (int i = 0; i < indent; i++) EditorGUI.indentLevel++;

        property.FindPropertyRelative("type").enumValueIndex = (int)(TMPParameterTypes.VectorType)
            EditorGUI.EnumPopup(position, GUIContent.none,
                (TMPParameterTypes.VectorType)property.FindPropertyRelative("type").enumValueIndex);
    }
}