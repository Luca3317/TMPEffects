using System.Collections;
using System.Collections.Generic;
using TMPEffects.Timeline;
using TMPEffects.TMPAnimations;
using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine;

namespace TMPEffects.Editor
{
    [CustomEditor(typeof(TMPAnimationClip))]
    public class TMPAnimationClipInspectorEditor : UnityEditor.Editor
    {
        private SerializedProperty animation;
        private SerializedProperty entryCurve;
        private SerializedProperty entryDuration;
        private SerializedProperty exitCurve;
        private SerializedProperty exitDuration;
        private SerializedProperty lastMovedEntry;

        private void Init()
        {
            animation ??= serializedObject.FindProperty("animation");
            entryCurve ??= serializedObject.FindProperty("entryCurve");
            entryDuration ??= serializedObject.FindProperty("entryDuration");
            exitCurve ??= serializedObject.FindProperty("exitCurve");
            exitDuration ??= serializedObject.FindProperty("exitDuration");
            lastMovedEntry ??= serializedObject.FindProperty("lastMovedEntry");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            Init();
            
            var drawCurves = EditorPrefs.GetBool(TMPEffectsTimelineEditorPrefsKeys.DRAW_CURVES_EDITORPREFS_KEY);
            var set = EditorGUILayout.Toggle(new GUIContent("Draw blending curves"), drawCurves);
            if (drawCurves != set)
                EditorPrefs.SetBool(TMPEffectsTimelineEditorPrefsKeys.DRAW_CURVES_EDITORPREFS_KEY, set);

            animation.objectReferenceValue =
                EditorGUILayout.ObjectField("Animation Asset", animation.objectReferenceValue, typeof(ITMPAnimation), true);

            entryCurve.isExpanded = EditorGUILayout.Foldout(entryCurve.isExpanded, "Entry");
            if (entryCurve.isExpanded)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(entryCurve);
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(entryDuration);
                if (EditorGUI.EndChangeCheck())
                {
                    lastMovedEntry.intValue = 0;
                }
                EditorGUI.indentLevel--;
            }

            exitCurve.isExpanded = EditorGUILayout.Foldout(exitCurve.isExpanded, "Exit");
            if (exitCurve.isExpanded)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(exitCurve);
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(exitDuration);
                if (EditorGUI.EndChangeCheck())
                {
                    lastMovedEntry.intValue = 1;
                }
                EditorGUI.indentLevel--;
            }

            if (serializedObject.hasModifiedProperties)
                serializedObject.ApplyModifiedProperties();
        }
    }
}