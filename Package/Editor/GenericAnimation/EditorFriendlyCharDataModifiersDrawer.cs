using System.Collections;
using System.Collections.Generic;
using TMPEffects.Modifiers;
using TMPEffects.TMPAnimations;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;

[CustomPropertyDrawer(typeof(EditorFriendlyCharDataModifiers))]
public class EditorFriendlyCharDataModifiersDrawer : PropertyDrawer
{
    private SerializedProperty positionProp;
    private SerializedProperty scaleProp;
    private SerializedProperty rotationsProp;
    private SerializedProperty blPositionProp, tlPositionProp, trPositionProp, brPositionProp;
    private SerializedProperty blColorProp, tlColorProp, trColorProp, brColorProp;
    private SerializedProperty blUV0Prop, tlUV0Prop, trUV0Prop, brUV0Prop;

    private Color32 backgroundColor;

    private const float TypedVectorFoldoutWidth = 20f;
    
    private static readonly GUIContent blColorGUI = new GUIContent("BL Color", "The color modifier to apply to the bottom left vertex.");
    private static readonly GUIContent tlColorGUI = new GUIContent("TL Color", "The color modifier to apply to the top left vertex.");
    private static readonly GUIContent trColorGUI = new GUIContent("TR Color", "The color modifier to apply to the top right vertex.");
    private static readonly GUIContent brColorGUI = new GUIContent("BR Color", "The color modifier to apply to the bottom right vertex.");
    
    private static readonly GUIContent blPositionGUI = new GUIContent("BL Position", "The position modifier to apply to the bottom left vertex.");
    private static readonly GUIContent tlPositionGUI = new GUIContent("TL Position", "The position modifier to apply to the top left vertex.");
    private static readonly GUIContent trPositionGUI = new GUIContent("TR Position", "The position modifier to apply to the top right vertex.");
    private static readonly GUIContent brPositionGUI = new GUIContent("BR Position", "The position modifier to apply to the bottom right vertex.");
    
    private static readonly GUIContent positionGUI = new GUIContent("Position", "The position modifier to apply.");
    private static readonly GUIContent scaleGUI = new GUIContent("Scale", "The scale modifier to apply.");
    private static readonly GUIContent rotationsGUI = new GUIContent("Rotations", "The rotation modifiers to apply.");

    private void Init(SerializedProperty property)
    {
        positionProp = property.FindPropertyRelative("Position");
        scaleProp = property.FindPropertyRelative("Scale");
        rotationsProp = property.FindPropertyRelative("Rotations");

        blPositionProp = property.FindPropertyRelative("BL_Position");
        tlPositionProp = property.FindPropertyRelative("TL_Position");
        trPositionProp = property.FindPropertyRelative("TR_Position");
        brPositionProp = property.FindPropertyRelative("BR_Position");

        blColorProp = property.FindPropertyRelative("BL_Color");
        tlColorProp = property.FindPropertyRelative("TL_Color");
        trColorProp = property.FindPropertyRelative("TR_Color");
        brColorProp = property.FindPropertyRelative("BR_Color");

        blPositionProp = property.FindPropertyRelative("BL_Position");
        tlPositionProp = property.FindPropertyRelative("TL_Position");
        trPositionProp = property.FindPropertyRelative("TR_Position");
        brPositionProp = property.FindPropertyRelative("BR_Position");

        blUV0Prop = property.FindPropertyRelative("BL_UV0");
        tlUV0Prop = property.FindPropertyRelative("TL_UV0");
        trUV0Prop = property.FindPropertyRelative("TR_UV0");
        brUV0Prop = property.FindPropertyRelative("BR_UV0");

        backgroundColor = EditorGUIUtility.isProSkin
            ? new Color32(56, 56, 56, 255)
            : new Color32(194, 194, 194, 255);
    }

    private Rect DrawCharacterModifier(Rect rect)
    {
        var bgRect = new Rect(rect.x, rect.y, rect.width,
            (EditorGUIUtility.singleLineHeight * 2) + EditorGUI.GetPropertyHeight(rotationsProp, true));
        EditorGUI.DrawRect(bgRect, backgroundColor);

        EditorGUI.PropertyField(rect, positionProp, positionGUI);
        rect.y += EditorGUIUtility.singleLineHeight;

        rect.height = EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(rect, scaleProp, scaleGUI, true);
        rect.y += EditorGUIUtility.singleLineHeight;

        rect.height = EditorGUI.GetPropertyHeight(rotationsProp, true);
        EditorGUI.PropertyField(rect, rotationsProp, rotationsGUI, true);
        rect.y += rect.height;

        rect.height = EditorGUIUtility.singleLineHeight;
        rect.y += EditorGUIUtility.singleLineHeight;
        return rect;
    }

    private Rect DrawVertexPositionModifiers(Rect rect)
    {
        var bgRect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight * 4);
        EditorGUI.DrawRect(bgRect, backgroundColor);

        EditorGUI.PropertyField(rect, blPositionProp, blPositionGUI);
        rect.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(rect, tlPositionProp, tlPositionGUI);
        rect.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(rect, trPositionProp, trPositionGUI);
        rect.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(rect, brPositionProp, brPositionGUI);
        rect.y += EditorGUIUtility.singleLineHeight;

        rect.y += EditorGUIUtility.singleLineHeight;
        return rect;
    }

    private Rect DrawVertexColorModifiers(Rect rect)
    {
        var bgRect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight * 4);
        EditorGUI.DrawRect(bgRect, backgroundColor);

        EditorGUI.PropertyField(rect, blColorProp, blColorGUI);
        rect.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(rect, tlColorProp, tlColorGUI);
        rect.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(rect, trColorProp, trColorGUI);
        rect.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(rect, brColorProp, brColorGUI);
        rect.y += EditorGUIUtility.singleLineHeight;

        rect.y += EditorGUIUtility.singleLineHeight;
        return rect;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        Init(property);

        var rect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

        scaleProp.isExpanded =
            EditorGUI.Foldout(rect, scaleProp.isExpanded, new GUIContent("Character Modifiers"), true);
        rect.y += EditorGUIUtility.singleLineHeight;

        if (scaleProp.isExpanded)
            rect = DrawCharacterModifier(rect);

        blPositionProp.isExpanded =
            EditorGUI.Foldout(rect, blPositionProp.isExpanded, new GUIContent("Vertex Position Modifiers"), true);
        rect.y += EditorGUIUtility.singleLineHeight;

        if (blPositionProp.isExpanded)
            rect = DrawVertexPositionModifiers(rect);

        blColorProp.isExpanded =
            EditorGUI.Foldout(rect, blColorProp.isExpanded, new GUIContent("Vertex Color Modifiers"), true);
        rect.y += EditorGUIUtility.singleLineHeight;

        if (blColorProp.isExpanded)
            rect = DrawVertexColorModifiers(rect);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        Init(property);
        float height = 0f;

        height += EditorGUIUtility.singleLineHeight;
        if (scaleProp.isExpanded)
        {
            height += EditorGUIUtility.singleLineHeight * 3;
            height += EditorGUI.GetPropertyHeight(rotationsProp, true);
        }

        height += EditorGUIUtility.singleLineHeight;
        if (blPositionProp.isExpanded) height += EditorGUIUtility.singleLineHeight * 5;

        height += EditorGUIUtility.singleLineHeight;
        if (blColorProp.isExpanded) height += EditorGUIUtility.singleLineHeight * 5;

        return height;
    }
}