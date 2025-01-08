using System.Collections;
using System.Collections.Generic;
using TMPEffects.CharacterData;
using TMPEffects.Modifiers;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ColorOverride))]
public class ColorOverrideDrawer : PropertyDrawer
{
    private SerializedProperty flagProp;
    private SerializedProperty colorProp;

    private void Init(SerializedProperty property)
    {
        flagProp = property.FindPropertyRelative("Override");
        colorProp = property.FindPropertyRelative("Color");
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        Init(property);
        
        position = EditorGUI.PrefixLabel(position, label);

        EditorGUI.indentLevel--;
        
        var flags = (ColorOverride.OverrideMode)flagProp.enumValueFlag;
        if (flags.HasFlag(ColorOverride.OverrideMode.Color))
        {
            colorProp.colorValue =
                EditorGUI.ColorField(position, GUIContent.none, colorProp.colorValue, true,
                    flags.HasFlag(ColorOverride.OverrideMode.Alpha), false);
        }
        else if (flags.HasFlag(ColorOverride.OverrideMode.Alpha))
        {
            float alpha = EditorGUI.Slider(position, colorProp.colorValue.a, 0f, 1f);
            
            if (colorProp.colorValue.a != alpha)
            {
                colorProp.colorValue = new Color32((byte)colorProp.colorValue.r, (byte)colorProp.colorValue.g, (byte)colorProp.colorValue.b, (byte)
                    (alpha * 255));
            }
        }

        position.x -= 105;
        position.width = 100;
        
        EditorGUI.indentLevel++;
        
        flagProp.enumValueFlag = (int)(ColorOverride.OverrideMode)
            EditorGUI.EnumFlagsField(position, GUIContent.none,(ColorOverride.OverrideMode)flagProp.enumValueFlag);
    }
    
    // public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    // {
    //     EditorGUI.BeginProperty(position, label, property);
    //     Init(property);
    //
    //     
    //     position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
    //     var rect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
    //
    //     var flagsRect = new Rect(rect.x - 75, rect.y, 100, rect.height);
    //     var flags = (ColorOverride.OverrideMode)EditorGUI.EnumFlagsField(flagsRect,
    //         (ColorOverride.OverrideMode)flagProp.enumValueFlag);
    //     flagProp.enumValueFlag = (int)flags;
    //
    //
    //     var colorRect = new Rect(rect.x, rect.y, rect.width, rect.height);
    //
    //     if (flags.HasFlag(ColorOverride.OverrideMode.Color))
    //     {
    //         colorProp.colorValue =
    //             EditorGUI.ColorField(colorRect, GUIContent.none, colorProp.colorValue, true,
    //                 flags.HasFlag(ColorOverride.OverrideMode.Alpha), false);
    //     }
    //     else if (flags.HasFlag(ColorOverride.OverrideMode.Alpha))
    //     {
    //         float alpha = EditorGUI.Slider(colorRect, colorProp.colorValue.a, 0f, 1f);
    //         
    //         if (colorProp.colorValue.a != alpha)
    //         {
    //             colorProp.colorValue = new Color32((byte)colorProp.colorValue.r, (byte)colorProp.colorValue.g, (byte)colorProp.colorValue.b, (byte)
    //                 (alpha * 255));
    //         }
    //     }
    //
    //     EditorGUI.EndProperty();
    // }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight;
    }
}