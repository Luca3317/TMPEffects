using TMPEffects.TMPAnimations;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[CustomPropertyDrawer(typeof(TimelineAnimationStep), false)]
public class AnimationStepDrawer : PropertyDrawer
{
    private bool entry = false;

    private SerializedProperty entryCurve;
    private SerializedProperty exitCurve;
    private SerializedProperty entryDuration;
    private SerializedProperty exitDuration;

    private SerializedProperty modifiers;
    private SerializedProperty wave;
    private SerializedProperty waveOffsetType;
    private SerializedProperty useWave;

    private Color backgroundColor;

    private void Init(SerializedProperty property)
    {
        entryCurve = property.FindPropertyRelative("entryCurve");
        exitCurve = property.FindPropertyRelative("exitCurve");
        entryDuration = property.FindPropertyRelative("entryDuration");
        exitDuration = property.FindPropertyRelative("exitDuration");
        modifiers = property.FindPropertyRelative("editorModifiers");
        useWave = property.FindPropertyRelative("useWave");
        waveOffsetType = property.FindPropertyRelative("waveOffsetType");
        wave = property.FindPropertyRelative("wave");
        backgroundColor = EditorGUIUtility.isProSkin
            ? new Color32(56, 56, 56, 255)
            : new Color32(194, 194, 194, 255);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        Init(property);

        EditorGUI.BeginProperty(position, label, property);

        var rect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

        EditorGUI.BeginChangeCheck();

        EditorGUI.LabelField(rect, "Blending", EditorStyles.boldLabel);
        rect.y += EditorGUIUtility.singleLineHeight;

        entryCurve.isExpanded = EditorGUI.Foldout(rect, entryCurve.isExpanded, "Entry");
        rect.y += EditorGUIUtility.singleLineHeight;

        if (entryCurve.isExpanded)
        {
            EditorGUI.indentLevel++;
            var bgRect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight * 2);
            EditorGUI.DrawRect(bgRect, backgroundColor);

            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(rect, entryDuration);
            if (EditorGUI.EndChangeCheck())
            {
                property.FindPropertyRelative("lastMovedEntry").intValue = 0;
            }

            rect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(rect, entryCurve);
            rect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.indentLevel--;
        }

        exitCurve.isExpanded = EditorGUI.Foldout(rect, exitCurve.isExpanded, "Exit");
        rect.y += EditorGUIUtility.singleLineHeight;

        if (exitCurve.isExpanded)
        {
            EditorGUI.indentLevel++;
            var bgRect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight * 2);
            EditorGUI.DrawRect(bgRect, backgroundColor);

            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(rect, exitDuration);
            if (EditorGUI.EndChangeCheck())
            {
                property.FindPropertyRelative("lastMovedEntry").intValue = 1;
            }

            rect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(rect, exitCurve);
            rect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.indentLevel--;
        }

        rect.y += EditorGUIUtility.singleLineHeight;


        EditorGUI.LabelField(rect, "Waves", EditorStyles.boldLabel);
        rect.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(rect, useWave);
        rect.y += EditorGUIUtility.singleLineHeight;

        if (useWave.boolValue)
        {
            EditorGUI.PropertyField(rect, waveOffsetType);
            rect.y += EditorGUIUtility.singleLineHeight;

            if (wave.isExpanded)
            {
                var bgRect = new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight, rect.width,
                    EditorGUI.GetPropertyHeight(wave, true) - EditorGUIUtility.singleLineHeight);
                EditorGUI.DrawRect(bgRect, backgroundColor);
            }

            // EditorGUI.indentLevel++;
            EditorGUI.PropertyField(rect, wave, true);
            rect.y += EditorGUI.GetPropertyHeight(wave, true);
            // EditorGUI.indentLevel--;
        }

        rect.y += EditorGUIUtility.singleLineHeight;

        EditorGUI.LabelField(rect, "Modifier", EditorStyles.boldLabel);
        rect.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(rect, modifiers, true);
        rect.y += EditorGUI.GetPropertyHeight(modifiers, true);

        if (EditorGUI.EndChangeCheck())
        {
            property.serializedObject.ApplyModifiedProperties();
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        Init(property);
        float totalHeight = 0f; /*EditorGUIUtility.singleLineHeight;*/ // foldout
        if (!property.isExpanded) return totalHeight;

        totalHeight += EditorGUIUtility.singleLineHeight; // entry foldout
        if (entryCurve.isExpanded)
        {
            totalHeight += EditorGUIUtility.singleLineHeight * 2f; // entryDuration and entryCurve
        }

        totalHeight += EditorGUIUtility.singleLineHeight; // exit foldout
        if (exitCurve.isExpanded)
        {
            totalHeight += EditorGUIUtility.singleLineHeight * 2f; // exitDuration and exitCurve
        }

        totalHeight += EditorGUIUtility.singleLineHeight; // usewave

        if (useWave.boolValue)
        {
            totalHeight += EditorGUIUtility.singleLineHeight;
            totalHeight += EditorGUI.GetPropertyHeight(wave, true);
        }

        totalHeight += EditorGUI.GetPropertyHeight(modifiers, true);

        totalHeight += EditorGUIUtility.singleLineHeight * 6; // Headers and space post header

        return totalHeight;
    }
}