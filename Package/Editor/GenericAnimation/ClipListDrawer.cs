using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPEffects.TMPAnimations;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomPropertyDrawer(typeof(GenericAnimation.TrackList))]
public class TrackListDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.PropertyField(position, property.FindPropertyRelative("Tracks"));
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var prop = property.FindPropertyRelative("Tracks");
        if (prop.isExpanded)
        {
            return EditorGUI.GetPropertyHeight(property.FindPropertyRelative("Tracks"), true);
        }
        else
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}


[CustomPropertyDrawer(typeof(GenericAnimation.Track))]
public class TrackDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.PropertyField(position, property.FindPropertyRelative("Clips"));
        var list = ReorderableList.GetReorderableListFromSerializedProperty(property.FindPropertyRelative("Clips"));
        list.drawElementCallback = (rect, index, isActive, isFocused) =>
            DrawElementCallback(property, rect, index, isActive, isFocused);
        list.onAddCallback = (rList) =>
            AddCallback(property, rList);
    }

    private void DrawElementCallback(SerializedProperty prop, Rect rect, int index, bool isactive, bool isfocused)
    {
        var itemProp = prop.FindPropertyRelative("Clips").GetArrayElementAtIndex(index);
        var nameProp = itemProp.FindPropertyRelative("name");
        var animateProp = itemProp.FindPropertyRelative("animate");

        var toggleRect = new Rect(rect.x + 20, rect.y, 15f, EditorGUIUtility.singleLineHeight);
        animateProp.boolValue = EditorGUI.ToggleLeft(toggleRect, GUIContent.none, animateProp.boolValue);
        toggleRect.x += 17.5f;
        toggleRect.width = rect.width - 17.5f;
        EditorGUI.LabelField(toggleRect,
            string.IsNullOrWhiteSpace(nameProp.stringValue) ? "Element " + index : nameProp.stringValue);

        float fromWidth = 75;
        float gap = 10;
        var fromRect = new Rect(rect.x + rect.width - fromWidth * 2 - gap * 1.5f, rect.y, fromWidth,
            EditorGUIUtility.singleLineHeight);
        var toRect = new Rect(rect.x + rect.width - fromWidth - gap, rect.y, fromWidth,
            EditorGUIUtility.singleLineHeight);

        var startTimeProp = itemProp.FindPropertyRelative("startTime");
        var durationProp = itemProp.FindPropertyRelative("duration");

        EditorGUIUtility.labelWidth = 30;
        float newTime = Mathf.Max(0, EditorGUI.FloatField(fromRect, new GUIContent("from:"), startTimeProp.floatValue));
        EditorGUIUtility.labelWidth = 15;
        float newEndTime = Mathf.Max(newTime, EditorGUI.FloatField(toRect, new GUIContent("to:"),
            startTimeProp.floatValue + durationProp.floatValue));

        if (newEndTime < newTime) newEndTime = newTime;

        if (newTime != startTimeProp.floatValue)
        {
            startTimeProp.floatValue = newTime;
            OnChangedStartOrDuration(prop, index);
        }
        else if (newEndTime != startTimeProp.floatValue + durationProp.floatValue)
        {
            durationProp.floatValue = newEndTime - startTimeProp.floatValue;
            OnChangedStartOrDuration(prop, index);
        }

        EditorGUIUtility.labelWidth = 0;
        EditorGUI.PropertyField(rect, itemProp, GUIContent.none);
    }

    private void AddCallback(SerializedProperty prop, ReorderableList list)
    {
        var animStepsProp = prop.FindPropertyRelative("Clips");
        animStepsProp.arraySize++;
        var newElement = animStepsProp.GetArrayElementAtIndex(animStepsProp.arraySize - 1);
        newElement.managedReferenceValue = new AnimationStep();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var prop = property.FindPropertyRelative("Clips");
        if (prop.isExpanded)
        {
            return EditorGUI.GetPropertyHeight(property.FindPropertyRelative("Clips"), true);
        }
        else
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
    
    // Update all other durations when one was changed in the inspector
    // ChangedIndex: the index that was changed,
    // ignoreIndex: further indices that should remain unchanged
    private void OnChangedStartOrDuration(SerializedProperty prop, int changedIndex, params int[] ignoreIndex)
    {
        var steps = prop.FindPropertyRelative("Clips");
        var changedProp = steps.GetArrayElementAtIndex(changedIndex);
        float changedStartTime = changedProp.FindPropertyRelative("startTime").floatValue;
        float changedDuration = changedProp.FindPropertyRelative("duration").floatValue;
        float changedEndTime = changedStartTime + changedDuration;

        // Update all start times and durations to accomodate for changed time
        for (int i = 0; i < steps.arraySize; i++)
        {
            if (i == changedIndex || ignoreIndex.Contains(i)) continue;

            var step = steps.GetArrayElementAtIndex(i);

            float startTime = step.FindPropertyRelative("startTime").floatValue;
            if (startTime > changedEndTime) continue; // If starts after changed ends, cont

            float duration = step.FindPropertyRelative("duration").floatValue;
            float endTime = startTime + duration;
            if (endTime < changedStartTime) continue; // If ends before changed starts, cont
            // else, effected

            // if starts while changed already running
            if (startTime >= changedStartTime)
            {
                // if ends after changed, clamp starttime
                if (endTime >= changedEndTime)
                {
                    step.FindPropertyRelative("startTime").floatValue = changedEndTime;
                    OnChangedStartOrDuration(prop, i, ignoreIndex.Concat(new int[] { changedIndex }).ToArray());
                }
                // else meaning fully within the changed step
                else
                {
                    step.FindPropertyRelative("startTime").floatValue = changedStartTime;
                    step.FindPropertyRelative("duration").floatValue = 0f;
                    OnChangedStartOrDuration(prop, i, ignoreIndex.Concat(new int[] { changedIndex }).ToArray());
                }
            }

            // if changed starts while this is already running, clamp the duration
            else if (changedStartTime >= startTime)
            {
                step.FindPropertyRelative("duration").floatValue = changedStartTime - startTime;
                OnChangedStartOrDuration(prop, i, ignoreIndex.Concat(new int[] { changedIndex }).ToArray());
            }
        }
    }

}