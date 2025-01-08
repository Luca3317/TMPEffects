using System;
using TMPEffects.Editor;
using TMPEffects.Timeline;
using TMPEffects.TMPAnimations;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[CustomPropertyDrawer(typeof(TimelineAnimationStep), false)]
public class TimelineAnimationStepDrawer : AnimationStepDrawer
{
    private SerializedProperty entryCurve;
    private SerializedProperty exitCurve;
    private SerializedProperty entryDuration;
    private SerializedProperty exitDuration;

    private SerializedProperty modifiers;
    private SerializedProperty initModifiers;
    private SerializedProperty useInitModifiers;
    private SerializedProperty wave;
    private SerializedProperty waveOffsetType;
    private SerializedProperty useWave;

    private Color backgroundColor;
    
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        Init(property.FindPropertyRelative("Step"));

        EditorGUI.BeginProperty(position, label, property);

        var rect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

        var drawCurves = EditorPrefs.GetBool(TMPEffectsTimelineEditorPrefsKeys.DRAW_CURVES_EDITORPREFS_KEY);
        var set = EditorGUI.Toggle(rect, new GUIContent("Draw blending curves"), drawCurves);
        if (drawCurves != set)
            EditorPrefs.SetBool(TMPEffectsTimelineEditorPrefsKeys.DRAW_CURVES_EDITORPREFS_KEY, set);
        
        rect.y += EditorGUIUtility.singleLineHeight * 2;
        
        DrawCommon(rect, property.FindPropertyRelative("Step"), label);

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        Init(property.FindPropertyRelative("Step"));
        return GetCommonHeight(property.FindPropertyRelative("Step")) + (EditorGUIUtility.singleLineHeight  *2);
    }
}