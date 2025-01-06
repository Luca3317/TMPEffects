using TMPEffects.TMPAnimations;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;

[CustomPropertyDrawer(typeof(AnimationStep), false)]
public class AnimationStepDrawer : PropertyDrawer
{
    private SerializedProperty entryCurve;
    private SerializedProperty exitCurve;
    private SerializedProperty entryDuration;
    private SerializedProperty exitDuration;

    private SerializedProperty startTime;
    private SerializedProperty duration;
    private SerializedProperty modifiers;
    private SerializedProperty initModifiers;
    private SerializedProperty useInitModifiers;
    private SerializedProperty wave;
    private SerializedProperty waveOffsetType;
    private SerializedProperty useWave;

    private SerializedProperty preExtrapolation;
    private SerializedProperty postExtrapolation;
    
    private static readonly GUIContent waveGUI = new GUIContent("Wave", "Whether to use a wave.");
    private static readonly GUIContent initialModsGUI = new GUIContent("Initial Modifiers", "Whether to use initial modifiers. If so, the lerp will be between InitialModifiers and Modifiers. Otherwise, the lerp will be between the CharData and Modifiers.");
    private static readonly GUIContent modsGUI = new GUIContent("Modifiers");
    private static readonly GUIContent extraplationGUI = new GUIContent("Extrapolation");
    
    private Color backgroundColor; 

    protected void Init(SerializedProperty property)
    {
        entryCurve = property.FindPropertyRelative("entryCurve");
        exitCurve = property.FindPropertyRelative("exitCurve");
        entryDuration = property.FindPropertyRelative("entryDuration");
        exitDuration = property.FindPropertyRelative("exitDuration");
        startTime = property.FindPropertyRelative("startTime");
        duration = property.FindPropertyRelative("duration");
        modifiers = property.FindPropertyRelative("modifiers");
        initModifiers = property.FindPropertyRelative("initModifiers");
        useInitModifiers = property.FindPropertyRelative("useInitialModifiers");
        useWave = property.FindPropertyRelative("useWave");
        waveOffsetType = property.FindPropertyRelative("waveOffset");
        wave = property.FindPropertyRelative("wave");
        preExtrapolation = property.FindPropertyRelative("preExtrapolation");
        postExtrapolation = property.FindPropertyRelative("postExtrapolation");

        backgroundColor = EditorGUIUtility.isProSkin
            ? new Color32(56, 56, 56, 255)
            : new Color32(194, 194, 194, 255);
    }

    protected float GetCommonHeight(SerializedProperty property)
    {
        float height = EditorGUIUtility.singleLineHeight * 5; // Blending + in out headers

        if (entryCurve.isExpanded)
        {
            height += EditorGUIUtility.singleLineHeight + EditorGUI.GetPropertyHeight(entryCurve);
        }

        if (exitCurve.isExpanded)
        {
            height += EditorGUIUtility.singleLineHeight + EditorGUI.GetPropertyHeight(exitCurve);
        }

        height += EditorGUIUtility.singleLineHeight * 3; // Space + waves

        if (useWave.boolValue) height += EditorGUI.GetPropertyHeight(wave, true) + EditorGUI.GetPropertyHeight(waveOffsetType, true);

        height += EditorGUIUtility.singleLineHeight * 2; // Space + initial

        if (useInitModifiers.boolValue) height += EditorGUI.GetPropertyHeight(initModifiers, true);

        height += EditorGUIUtility.singleLineHeight * 2; // Space + modifiers

        height += EditorGUI.GetPropertyHeight(modifiers, true);

        return height;
    }

    // Clamp entry / exit blend duration when either, or clip duration changed
    protected void OnClipDurationChanged(int control)
    {
        entryDuration.floatValue = Mathf.Max(0, entryDuration.floatValue);
        exitDuration.floatValue = Mathf.Max(0, exitDuration.floatValue);
        duration.floatValue = Mathf.Max(0, duration.floatValue);
        
        // if step duration changed
        if (control == 0)
        {
            entryDuration.floatValue = Mathf.Min(entryDuration.floatValue, duration.floatValue);
            exitDuration.floatValue = Mathf.Min(exitDuration.floatValue, duration.floatValue - entryDuration.floatValue);
        }
        // if entry duration changed
        else if (control == 1)
        {
            entryDuration.floatValue = Mathf.Min(entryDuration.floatValue, duration.floatValue);
            exitDuration.floatValue = Mathf.Min(exitDuration.floatValue, duration.floatValue - entryDuration.floatValue);
        }
        // if exit duration changed
        else if (control == 2)
        {
            exitDuration.floatValue = Mathf.Min(exitDuration.floatValue, duration.floatValue);
            entryDuration.floatValue = Mathf.Min(entryDuration.floatValue, duration.floatValue - exitDuration.floatValue);
        }
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
            var bgRect = new Rect(rect.x, rect.y, rect.width,
                EditorGUIUtility.singleLineHeight + EditorGUI.GetPropertyHeight(entryCurve));
            EditorGUI.DrawRect(bgRect, backgroundColor);

            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(rect, entryDuration);
            if (EditorGUI.EndChangeCheck())
            {
                if (property.FindPropertyRelative("lastMovedEntry") != null)
                    property.FindPropertyRelative("lastMovedEntry").intValue = 0;
                
                OnClipDurationChanged(1);
            }

            rect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(rect, entryCurve);
            rect.y += EditorGUI.GetPropertyHeight(entryCurve);
            EditorGUI.indentLevel--;
        }

        exitCurve.isExpanded = EditorGUI.Foldout(rect, exitCurve.isExpanded, "Exit");
        rect.y += EditorGUIUtility.singleLineHeight;

        if (exitCurve.isExpanded)
        {
            EditorGUI.indentLevel++;
            var bgRect = new Rect(rect.x, rect.y, rect.width,
                EditorGUIUtility.singleLineHeight + EditorGUI.GetPropertyHeight(exitCurve));
            EditorGUI.DrawRect(bgRect, backgroundColor);

            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(rect, exitDuration);
            if (EditorGUI.EndChangeCheck())
            {
                if (property.FindPropertyRelative("lastMovedEntry") != null)
                    property.FindPropertyRelative("lastMovedEntry").intValue = 1;
                
                OnClipDurationChanged(2);
            }

            rect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(rect, exitCurve);
            rect.y += EditorGUI.GetPropertyHeight(exitCurve);
            EditorGUI.indentLevel--;
        }

        rect.y += EditorGUIUtility.singleLineHeight;

        Vector2 labelSize = EditorStyles.boldLabel.CalcSize(waveGUI);
        EditorGUI.BeginDisabledGroup(!useWave.boolValue);
        EditorGUI.LabelField(rect, waveGUI, EditorStyles.boldLabel);
        EditorGUI.EndDisabledGroup();
        var toggleRect = new Rect(rect.x + labelSize.x + 10, rect.y, rect.width - labelSize.x - 10, rect.height);
        useWave.boolValue = EditorGUI.Toggle(toggleRect, useWave.boolValue);
        rect.y += EditorGUIUtility.singleLineHeight;

        if (useWave.boolValue)
        {
            EditorGUI.PropertyField(rect, waveOffsetType, true);
            rect.y += EditorGUI.GetPropertyHeight(waveOffsetType);

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

        labelSize = EditorStyles.boldLabel.CalcSize(initialModsGUI);
        EditorGUI.BeginDisabledGroup(!useInitModifiers.boolValue);
        EditorGUI.LabelField(rect, initialModsGUI, EditorStyles.boldLabel);
        EditorGUI.EndDisabledGroup();
        toggleRect = new Rect(rect.x + labelSize.x + 10, rect.y, rect.width - labelSize.x - 10, rect.height);
        useInitModifiers.boolValue = EditorGUI.Toggle(toggleRect, useInitModifiers.boolValue);

        rect.y += EditorGUIUtility.singleLineHeight;
        if (useInitModifiers.boolValue)
        {
            EditorGUI.PropertyField(rect, initModifiers, true);
            rect.y += EditorGUI.GetPropertyHeight(initModifiers, true);
        }

        rect.y += EditorGUIUtility.singleLineHeight;

        EditorGUI.LabelField(rect, modsGUI, EditorStyles.boldLabel);
        rect.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(rect, modifiers, true);
        rect.y += EditorGUI.GetPropertyHeight(modifiers, true);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType == SerializedPropertyType.ManagedReference)
        {
            if (property.managedReferenceValue == null)
            {
                property.managedReferenceValue = new AnimationStep();
            }
        }

        Init(property);
        EditorGUI.BeginProperty(position, label, property);
        EditorGUI.indentLevel++;

        var rect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        property.isExpanded = EditorGUI.Foldout(rect, property.isExpanded, label);
        rect.y += EditorGUIUtility.singleLineHeight;
        if (!property.isExpanded)
        {
            EditorGUI.EndProperty();
            EditorGUI.indentLevel--;
            return;
        }

        EditorGUI.PropertyField(rect, property.FindPropertyRelative("name"));
        rect.y += EditorGUIUtility.singleLineHeight * 2f;

        EditorGUI.LabelField(rect, extraplationGUI, EditorStyles.boldLabel);
        rect.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(rect, preExtrapolation);
        rect.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(rect, postExtrapolation);
        rect.y += EditorGUIUtility.singleLineHeight * 2;

        DrawCommon(rect, property, label);

        EditorGUI.indentLevel--;
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (property.propertyType == SerializedPropertyType.ManagedReference)
        {
            if (property.managedReferenceValue == null)
            {
                property.managedReferenceValue = new AnimationStep();
            }
        }

        Init(property);

        if (property.isExpanded)
            return GetCommonHeight(property) + (EditorGUIUtility.singleLineHeight * 6);

        return EditorGUIUtility.singleLineHeight;
    }
}