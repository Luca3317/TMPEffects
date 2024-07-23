using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(AnimationStack<>.AnimPrefixTuple))]
public class AnimPrefixTupleDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var animProp = property.FindPropertyRelative("animation");
        var prefixProp = property.FindPropertyRelative("prefix");

        Rect rect0 = new Rect(position.position, new Vector2(position.width / 2f, position.height));
        Rect rect1 = new Rect(new Vector2(position.x + position.width / 2f + 5, position.y), new Vector2(position.width / 2f - 5, position.height));

        var prev = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth = 40;
        EditorGUI.PropertyField(rect0, prefixProp);
        EditorGUIUtility.labelWidth = 60;
        EditorGUI.PropertyField(rect1, animProp);
        EditorGUIUtility.labelWidth = prev;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight;
    }
}
