using TMPEffects.TMPAnimations;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;

[CustomPropertyDrawer(typeof(AnimationStep), false)]
public class AnimationStepDrawer : PropertyDrawer
{
    private bool entry = false;

    private SerializedProperty entryCurve;
    private SerializedProperty exitCurve;
    private SerializedProperty entryDuration;
    private SerializedProperty exitDuration;

    private SerializedProperty loops;
    private SerializedProperty repetitions;
    private SerializedProperty startTime;
    private SerializedProperty duration;
    private SerializedProperty modifiers;
    private SerializedProperty initModifiers;
    private SerializedProperty useInitModifiers;
    private SerializedProperty wave;
    private SerializedProperty waveOffsetType;
    private SerializedProperty useWave;


    private Color backgroundColor;

    protected void Init(SerializedProperty property)
    {
        entryCurve = property.FindPropertyRelative("entryCurve");
        exitCurve = property.FindPropertyRelative("exitCurve");
        entryDuration = property.FindPropertyRelative("entryDuration");
        exitDuration = property.FindPropertyRelative("exitDuration");
        loops = property.FindPropertyRelative("loops");
        repetitions = property.FindPropertyRelative("repetitions");
        startTime = property.FindPropertyRelative("startTime");
        duration = property.FindPropertyRelative("duration");
        modifiers = property.FindPropertyRelative("modifiers");
        initModifiers = property.FindPropertyRelative("initModifiers");
        useInitModifiers = property.FindPropertyRelative("useInitialModifiers");
        useWave = property.FindPropertyRelative("useWave");
        waveOffsetType = property.FindPropertyRelative("waveOffsetType");
        wave = property.FindPropertyRelative("wave");

        backgroundColor = EditorGUIUtility.isProSkin
            ? new Color32(56, 56, 56, 255)
            : new Color32(194, 194, 194, 255);
    }

    protected float GetCommonHeight(SerializedProperty property)
    {
        float height = EditorGUIUtility.singleLineHeight * 3; // Blending + in out headers

        if (entryCurve.isExpanded) height += EditorGUIUtility.singleLineHeight * 2;
        if (exitCurve.isExpanded) height += EditorGUIUtility.singleLineHeight * 2;

        height += EditorGUIUtility.singleLineHeight * 3; // Space + waves

        if (useWave.boolValue) height += EditorGUI.GetPropertyHeight(wave, true) + EditorGUIUtility.singleLineHeight;

        height += EditorGUIUtility.singleLineHeight * 2; // Space + initial

        if (useInitModifiers.boolValue) height += EditorGUI.GetPropertyHeight(initModifiers, true);

        height += EditorGUIUtility.singleLineHeight * 2; // Space + modifiers

        height += EditorGUI.GetPropertyHeight(modifiers, true);

        return height;
    }

    protected void DrawCommon(Rect rect, SerializedProperty property, GUIContent label)
    {
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
                if (property.FindPropertyRelative("lastMovedEntry") != null)
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
                if (property.FindPropertyRelative("lastMovedEntry") != null)
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


        GUIContent content = new GUIContent();
        content.text = "Initial Modifiers";
        content.tooltip =
            "Whether to use initial modifiers. If so, the lerp will be between InitialModifiers and Modifiers. Otherwise, the lerp will be between the CharData and Modifiers.";

        // useInitModifiers.boolValue = EditorGUI.Toggle(rect, content, useInitModifiers.boolValue, EditorStyles.boldLabel);
        Vector2 labelSize = EditorStyles.boldLabel.CalcSize(content);
        EditorGUI.BeginDisabledGroup(!useInitModifiers.boolValue);
        EditorGUI.LabelField(rect, content, EditorStyles.boldLabel);
        EditorGUI.EndDisabledGroup();
        var toggleRect = new Rect(rect.x + labelSize.x + 10, rect.y, rect.width - labelSize.x - 10, rect.height);
        useInitModifiers.boolValue = EditorGUI.Toggle(toggleRect, useInitModifiers.boolValue);

        // EditorGUI.PropertyField(rect, useInitModifiers, content);
        rect.y += EditorGUIUtility.singleLineHeight;
        if (useInitModifiers.boolValue)
        {
            EditorGUI.PropertyField(rect, initModifiers, true);
            rect.y += EditorGUI.GetPropertyHeight(initModifiers, true);
        }

        rect.y += EditorGUIUtility.singleLineHeight;

        EditorGUI.LabelField(rect, "Modifier", EditorStyles.boldLabel);
        rect.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(rect, modifiers, true);
        rect.y += EditorGUI.GetPropertyHeight(modifiers, true);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        Init(property);
        EditorGUI.BeginProperty(position, label, property);
        EditorGUI.indentLevel++;

        var rect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        property.isExpanded = EditorGUI.Foldout(rect, property.isExpanded, label);
        rect.y += EditorGUIUtility.singleLineHeight;
        if (!property.isExpanded) return;

        EditorGUI.PropertyField(rect, property.FindPropertyRelative("name"));
        rect.y += EditorGUIUtility.singleLineHeight * 2f;

        EditorGUI.PropertyField(rect, loops);
        rect.y += EditorGUIUtility.singleLineHeight;

        if (loops.boolValue)
        {
            EditorGUI.PropertyField(rect, repetitions, new GUIContent("Repetitions (0 = forever)"));
            rect.y += EditorGUIUtility.singleLineHeight;
        }

        EditorGUI.PropertyField(rect, startTime);
        rect.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(rect, duration);
        rect.y += EditorGUIUtility.singleLineHeight * 2;

        DrawCommon(rect, property, label);

        EditorGUI.indentLevel--;
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        Init(property);

        if (property.isExpanded)
            return GetCommonHeight(property) + EditorGUIUtility.singleLineHeight * 7;

        return EditorGUIUtility.singleLineHeight;

        float totalHeight = EditorGUIUtility.singleLineHeight; // foldout
        if (!property.isExpanded) return totalHeight;

        totalHeight += EditorGUIUtility.singleLineHeight * 2f; // For the "name" property

        if (property.serializedObject.targetObject is not PlayableAsset)
        {
            totalHeight += EditorGUIUtility.singleLineHeight * 2f; // startTime, duration
        }

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

        totalHeight += EditorGUIUtility.singleLineHeight * 2; // loops and useWave

        if (loops.boolValue)
        {
            totalHeight += EditorGUIUtility.singleLineHeight; // repetitions
        }

        if (useWave.boolValue)
        {
            totalHeight += EditorGUIUtility.singleLineHeight;
            totalHeight += EditorGUI.GetPropertyHeight(wave, true);
        }

        // totalHeight += EditorGUI.GetPropertyHeight(modifiers, true);

        totalHeight += EditorGUI.GetPropertyHeight(modifiers, true);

        return totalHeight;
    }
}