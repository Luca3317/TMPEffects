using System.Collections;
using System.Collections.Generic;
using TMPEffects.CharacterData;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.Text;

[CustomPropertyDrawer(typeof(TMPMeshModifiers))]
public class TMPMeshModifierDrawer : PropertyDrawer
{
    private Color32 backgroundColor;
    
    private void Init(SerializedProperty property)
    {
        backgroundColor = EditorGUIUtility.isProSkin
            ? new Color32(56, 56, 56, 255)
            : new Color32(194, 194, 194, 255);
    }

    private void DrawRawToggle(float y, SerializedProperty rawProp, Rect toggleRect, Rect labelRect)
    {
        toggleRect.y = y;
        labelRect.y = y;
        EditorGUI.LabelField(labelRect, "Scaled");
        rawProp.boolValue =
            EditorGUI.Toggle(toggleRect, rawProp.boolValue);
    }
    
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        Init(property);
        
        var rect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

        property.isExpanded = EditorGUI.Foldout(rect, property.isExpanded, property.displayName);
        EditorGUI.indentLevel++;

        if (!property.isExpanded) return;

        rect.y += EditorGUIUtility.singleLineHeight;
        property.FindPropertyRelative("positionDelta").isExpanded =
            EditorGUI.Foldout(rect, property.FindPropertyRelative("positionDelta").isExpanded, "Character Deltas");
        rect.y += EditorGUIUtility.singleLineHeight;
        
        
        var ctrlRect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight);
        var togglerect = new Rect(ctrlRect.x + EditorGUIUtility.labelWidth - 20, rect.y, ctrlRect.width, EditorGUIUtility.singleLineHeight);
        var labelRect = new Rect(togglerect.x - 45, togglerect.y, togglerect.width, EditorGUIUtility.singleLineHeight);

        if (property.FindPropertyRelative("positionDelta").isExpanded)
        {
            var bgRect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight * 3);
            EditorGUI.DrawRect(bgRect, backgroundColor);
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("positionDelta"));
            DrawRawToggle(rect.y, property.FindPropertyRelative("positionDeltaIsRaw"), togglerect, labelRect);
            rect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("rotationDelta"));
            rect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("scaleDelta"));
            rect.y += EditorGUIUtility.singleLineHeight;
        }

        property.FindPropertyRelative("bl_Delta").isExpanded =
            EditorGUI.Foldout(rect, property.FindPropertyRelative("bl_Delta").isExpanded, "Vertex Deltas");
        rect.y += EditorGUIUtility.singleLineHeight;

        if (property.FindPropertyRelative("bl_Delta").isExpanded)
        {
            var bgRect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight * 4);
            EditorGUI.DrawRect(bgRect, backgroundColor);
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("bl_Delta"));
            DrawRawToggle(rect.y, property.FindPropertyRelative("bl_DeltaIsRaw"), togglerect, labelRect);
            rect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("tl_Delta"));
            DrawRawToggle(rect.y, property.FindPropertyRelative("tl_DeltaIsRaw"), togglerect, labelRect);
            rect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("tr_Delta"));
            DrawRawToggle(rect.y, property.FindPropertyRelative("tr_DeltaIsRaw"), togglerect, labelRect);
            rect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("br_Delta"));
            DrawRawToggle(rect.y, property.FindPropertyRelative("br_DeltaIsRaw"), togglerect, labelRect);
            rect.y += EditorGUIUtility.singleLineHeight;
        }

        property.FindPropertyRelative("bl_Color").isExpanded =
            EditorGUI.Foldout(rect, property.FindPropertyRelative("bl_Color").isExpanded, "Vertex Colors");
        rect.y += EditorGUIUtility.singleLineHeight;

        if (property.FindPropertyRelative("bl_Color").isExpanded)
        {
            var bgRect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight * 4);
            EditorGUI.DrawRect(bgRect, backgroundColor);
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("bl_Color"));
            rect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("tl_Color"));
            rect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("tr_Color"));
            rect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("br_Color"));
            rect.y += EditorGUIUtility.singleLineHeight;
        }

        property.FindPropertyRelative("bl_UV0").isExpanded =
            EditorGUI.Foldout(rect, property.FindPropertyRelative("bl_UV0").isExpanded, "Vertex UVs");
        rect.y += EditorGUIUtility.singleLineHeight;

        if (property.FindPropertyRelative("bl_UV0").isExpanded)
        {
            togglerect.y = rect.y;
            labelRect.y = rect.y;
            
            var bgRect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight * 4);
            EditorGUI.DrawRect(bgRect, backgroundColor);
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("bl_UV0"));
            rect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("tl_UV0"));
            rect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("tr_UV0"));
            rect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("br_UV0"));
            rect.y += EditorGUIUtility.singleLineHeight;
        }
        
        EditorGUI.indentLevel--;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = EditorGUIUtility.singleLineHeight;
        
        if (!property.isExpanded) return height;

        height += EditorGUIUtility.singleLineHeight;
        
        if (property.FindPropertyRelative("positionDelta").isExpanded)
            height += EditorGUIUtility.singleLineHeight * 3;

        height += EditorGUIUtility.singleLineHeight;

        if (property.FindPropertyRelative("bl_Delta").isExpanded)
            height += EditorGUIUtility.singleLineHeight * 4;

        height += EditorGUIUtility.singleLineHeight;

        if (property.FindPropertyRelative("bl_Color").isExpanded)
            height += EditorGUIUtility.singleLineHeight * 4;

        height += EditorGUIUtility.singleLineHeight;

        if (property.FindPropertyRelative("bl_UV0").isExpanded)
            height += EditorGUIUtility.singleLineHeight * 4;

        return height;
    }
}