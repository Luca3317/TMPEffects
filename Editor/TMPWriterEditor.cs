using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TMPWriter))]
public class TMPWriterEditor : Editor
{
    TMPWriter writer;
    bool changed;
    float progress
    {
        get => _progress;
        set
        {
            _progress = value;
            changed = true;
        }
    }
    float _progress;
    bool clicked;
    bool wasWriting;
    bool useDefaultDatabase;
    bool wasEnabled;
    TMPCommandDatabase defaultDatabase;

    // Serialized properties
    SerializedProperty databaseProp;
    SerializedProperty speedProp;
    SerializedProperty startOnPlayProp;
    SerializedProperty eventsEnabledProp;
    SerializedProperty commandsEnabledProp;
    SerializedProperty onTextEventProp;
    SerializedProperty onShowCharacterProp;
    SerializedProperty onStartWriterProp;
    SerializedProperty onStopWriterProp;
    SerializedProperty onResetWriterProp;
    SerializedProperty onFinishWriterProp;

    // Styles
    GUIStyle buttonStyle;
    GUIStyle buttonLeft;
    GUIStyle buttonRight;
    GUIStyle buttonMid;
    GUIStyle buttonLeft2;
    GUIStyle buttonRight2;
    GUIStyle buttonMid2;
    GUIStyle playLabelStyle;
    GUIStyle playToggleStyle;
    GUIStyle progressBarControllerStyle;
    GUIStyle warningStyle;

    // GUIContent
    GUIContent playLabel;
    GUIContent eventToggleLabel;
    GUIContent commandToggleLabel;
    GUIContent useDefaultDatabaseLabel;
    GUIContent eventWarningContent;

    // Textures
    Texture playButtonTexture;
    Texture pauseButtonTexture;
    Texture resetButtonTexture;
    Texture stopButtonTexture;
    Texture skipButtonTexture;

    // Widths
    float width;
    const float playLabelWidth = 115;
    const float eventToggleWidth = 120;
    const float commandToggleWidth = 130;
    const float playButtonWidth = 50;
    const float buttonWidth = 33;

    // Heights
    float playLabelHeight;
    float eventToggleHeight;
    float commandToggleHeight;
    float playButtonHeight;
    float buttonHeight;
    float progressBarThickness;
    float headerHeight;
    float playerHeight;
    float dividerHeight;

    // Wrappings
    bool wrapHeader;
    bool wrapPlayer;

    // Rects
    Rect headerRect;
    Rect dividerRect;
    Rect playerRect;

    Rect playLabelRect;
    Rect eventToggleRect;
    Rect commandToggleRect;

    Rect playButtonRect;
    Rect resetButtonRect;
    Rect stopButtonRect;
    Rect skipButtonRect;
    Rect progressBarRect;
    Rect progressBarControllerRect;
    Rect eventWarningRect;

    // Constant offsets
    const float xOffset = 10;
    const float yOffset = 10;
    const float headerPlayerOffset = 5;
    const float buttonOffset = 5;
    const float progressBarXOffset = 10;
    const float progressBarYOffset = 10;

    private void OnEnable()
    {
        // Cache properties
        databaseProp = serializedObject.FindProperty("database");
        speedProp = serializedObject.FindProperty("speed");
        startOnPlayProp = serializedObject.FindProperty("writeOnStart");
        eventsEnabledProp = serializedObject.FindProperty("eventsEnabled");
        commandsEnabledProp = serializedObject.FindProperty("commandsEnabled");
        onTextEventProp = serializedObject.FindProperty("OnTextEvent");
        onShowCharacterProp = serializedObject.FindProperty("OnShowCharacter");
        onStartWriterProp = serializedObject.FindProperty("OnStartWriter");
        onStopWriterProp = serializedObject.FindProperty("OnStopWriter");
        onResetWriterProp = serializedObject.FindProperty("OnResetWriter");
        onFinishWriterProp = serializedObject.FindProperty("OnFinishWriter");

        // Load Textures
        playButtonTexture = (Texture)Resources.Load("playButton");
        pauseButtonTexture = (Texture)Resources.Load("pauseButton");
        resetButtonTexture = (Texture)Resources.Load("resetButton");
        stopButtonTexture = (Texture)Resources.Load("stopButton");
        skipButtonTexture = (Texture)Resources.Load("skipButton");

        // Other Initialization work
        writer = target as TMPWriter;
        progress = 0f;
        clicked = false;
        wasWriting = false;
        wasEnabled = writer.enabled;
        writer.OnShowCharacter.AddListener(UpdateProgress);
        writer.OnResetWriter.AddListener(UpdateProgress);
        defaultDatabase = (TMPCommandDatabase)Resources.Load("DefaultCommandDatabase");
        useDefaultDatabase = defaultDatabase == databaseProp.objectReferenceValue || databaseProp.objectReferenceValue == null;
    }

    private void OnDisable()
    {
        writer.OnShowCharacter.RemoveListener(UpdateProgress);
        writer.OnResetWriter.RemoveListener(UpdateProgress);
    }

    void UpdateProgress(CharData cData) => UpdateProgress(cData.index);

    void UpdateProgress(int index)
    {
        progress = Mathf.Lerp(0f, 1f, (float)index / (writer.TotalCharacterCount - 1));
    }

    void PrepareLayout()
    {
        width = EditorGUIUtility.currentViewWidth - 30;

        // Calculate wrappings
        wrapHeader = width < playLabelWidth + eventToggleWidth + commandToggleWidth - 10;
        wrapPlayer = (playButtonWidth + buttonWidth * 3) * 3f > width;

        // Calculate heights
        playLabelHeight = playLabelStyle.CalcHeight(playLabel, playLabelWidth);
        eventToggleHeight = playToggleStyle.CalcHeight(eventToggleLabel, eventToggleWidth);
        commandToggleHeight = playToggleStyle.CalcHeight(commandToggleLabel, commandToggleWidth);
        playButtonHeight = 30;
        buttonHeight = 20;
        progressBarThickness = 10;
        headerHeight = (wrapHeader ? playLabelHeight + playToggleStyle.lineHeight + Mathf.Max(eventToggleHeight, commandToggleHeight) : Mathf.Max(playLabelHeight, eventToggleHeight, commandToggleHeight));
        playerHeight = (wrapPlayer ? playButtonHeight + progressBarThickness + progressBarYOffset : playButtonHeight);
        dividerHeight = 15;

        // Prepare rects
        headerRect = new Rect(xOffset, yOffset, width, headerHeight);
        playerRect = new Rect(xOffset, yOffset + headerHeight + headerPlayerOffset, width, playerHeight);
        dividerRect = new Rect(xOffset, playerRect.y + playerRect.height + 9.5f, width + xOffset, 1f);

        playLabelRect = new Rect(headerRect.position, new Vector2(playLabelWidth, playLabelHeight));
        if (wrapHeader)
        {
            eventToggleRect = new Rect(headerRect.x, headerRect.y + playLabelHeight + playToggleStyle.lineHeight / 2, eventToggleWidth, eventToggleHeight);
            commandToggleRect = new Rect(headerRect.x + eventToggleWidth, headerRect.y + playLabelHeight + playToggleStyle.lineHeight / 2, commandToggleWidth, commandToggleHeight);
        }
        else
        {
            eventToggleRect = new Rect(headerRect.x + playLabelWidth, headerRect.y, eventToggleWidth, headerHeight);
            commandToggleRect = new Rect(headerRect.x + playLabelWidth + eventToggleWidth, headerRect.y, commandToggleWidth, headerHeight);
        }
        eventWarningRect = new Rect(eventToggleRect.x + eventToggleRect.width - 25, eventToggleRect.y, 20, 20);
        playButtonRect = new Rect(playerRect.x, playerRect.y, playButtonWidth, playButtonHeight);
        resetButtonRect = new Rect(playerRect.x + playButtonWidth + buttonOffset, playerRect.y + buttonHeight / 4, buttonWidth, buttonHeight);
        stopButtonRect = new Rect(resetButtonRect.x + buttonWidth, playerRect.y + buttonHeight / 4, buttonWidth, buttonHeight);
        skipButtonRect = new Rect(stopButtonRect.x + buttonWidth, playerRect.y + buttonHeight / 4, buttonWidth, buttonHeight);

        resetButtonRect.y += buttonHeight / 4;
        stopButtonRect.y += buttonHeight / 4;
        skipButtonRect.y += buttonHeight / 4;
        if (wrapPlayer)
        {
            progressBarRect = new Rect(playerRect.x + 5, playerRect.y + playButtonHeight + progressBarYOffset, width - progressBarXOffset + 10, progressBarThickness);
        }
        else
        {
            progressBarRect = new Rect(skipButtonRect.x + skipButtonRect.width + progressBarXOffset, playerRect.y + playButtonHeight / 2 - progressBarThickness / 2, width - (skipButtonRect.x - playerRect.x + skipButtonRect.width + progressBarXOffset) + 5, progressBarThickness);
            progressBarRect.y += buttonHeight / 4;
        }
        progressBarControllerRect = new Rect(progressBarRect.position.x + progressBarRect.width * progress - 2, progressBarRect.y, progressBarThickness, progressBarThickness);
    }

    void DrawPlayer()
    {
        // Draw play label
        EditorGUI.LabelField(playLabelRect, playLabel, playLabelStyle);

        // Draw play toggles
        eventsEnabledProp.boolValue = EditorGUI.ToggleLeft(eventToggleRect, eventToggleLabel, eventsEnabledProp.boolValue);

        if (eventsEnabledProp.boolValue)
        {
            GUI.Label(eventWarningRect, eventWarningContent);
        }

        commandsEnabledProp.boolValue = EditorGUI.ToggleLeft(commandToggleRect, commandToggleLabel, commandsEnabledProp.boolValue);

        EditorGUI.BeginDisabledGroup(!writer.enabled);

        // Draw Buttons
        if (writer.IsWriting || (wasWriting && clicked))
        {
            if (GUI.Button(playButtonRect, pauseButtonTexture))
            {
                PauseWriter();
            }
        }
        else if (GUI.Button(playButtonRect, playButtonTexture))
        {
            StartWriter();
        }

        if (GUI.Button(resetButtonRect, resetButtonTexture, buttonLeft2))
        {
            ResetWriter();
        }

        if (GUI.Button(stopButtonRect, stopButtonTexture, buttonMid2))
        {
            StopWriter();
        }

        if (GUI.Button(skipButtonRect, skipButtonTexture, buttonRight2))
        {
            FinishWriter();
        }

        // Draw progress bar
        EditorGUI.ProgressBar(progressBarRect, progress, "");
        GUI.Label(progressBarControllerRect, "", progressBarControllerStyle);

        EditorGUI.EndDisabledGroup();
    }

    void DrawDatabase()
    {
        GUILayout.BeginHorizontal();
        bool prevUseDefaultDatabase = useDefaultDatabase;
        useDefaultDatabase = EditorGUILayout.Toggle(useDefaultDatabaseLabel, useDefaultDatabase);

        if (prevUseDefaultDatabase != useDefaultDatabase)
        {
            // TODO
            Undo.RecordObject(writer, "Modified writer");
        }

        if (useDefaultDatabase)
        {
            if (!databaseProp.objectReferenceValue == defaultDatabase)
            {
                databaseProp.objectReferenceValue = defaultDatabase;
            }

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(databaseProp, GUIContent.none);
            EditorGUI.EndDisabledGroup();
        }
        else
        {
            EditorGUILayout.PropertyField(databaseProp, GUIContent.none);
        }

        GUILayout.EndHorizontal();
    }

    static bool eventFoldout = false;
    void DrawEventsFoldout()
    {
        eventFoldout = EditorGUILayout.Foldout(eventFoldout, new GUIContent("Events"));

        if (eventFoldout)
        {
            EditorGUILayout.PropertyField(onTextEventProp);
            EditorGUILayout.PropertyField(onShowCharacterProp);
            EditorGUILayout.PropertyField(onStartWriterProp);
            EditorGUILayout.PropertyField(onStopWriterProp);
            EditorGUILayout.PropertyField(onResetWriterProp);
            EditorGUILayout.PropertyField(onFinishWriterProp);
        }
    }

    void RepaintInspector()
    {
        DrawPlayer();

        EditorGUILayout.PropertyField(speedProp);
        EditorGUILayout.PropertyField(startOnPlayProp);

        DrawDatabase();

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("sceneCommands"));
        if (EditorGUI.EndChangeCheck()) writer.ForceReprocess();

        DrawEventsFoldout();
    }

    void HandleMouseDown()
    {
        if (progressBarRect.Contains(Event.current.mousePosition))
        {
            wasWriting = writer.IsWriting;
            clicked = true;
            writer.StopWriter();
            HandleMouseDrag();
        }
    }

    void HandleMouseUp()
    {
        if (clicked)
        {
            if (wasWriting)
                writer.StartWriter();
        }

        clicked = false;
    }

    void HandleMouseDrag()
    {
        if (!clicked) return;

        float xPos = Event.current.mousePosition.x;

        float min = progressBarRect.x;
        float max = progressBarRect.x + progressBarRect.width;

        progress = Mathf.InverseLerp(min, max, xPos);
        writer.ResetWriter(Mathf.RoundToInt(((writer.TotalCharacterCount - 1) * progress)));
    }

    bool styles = false;
    bool guiContent = false;
    void InitStyles()
    {
        if (styles) return;
        styles = true;

        // Create GUIStyles
        buttonStyle = new GUIStyle("minibuttonleft");
        buttonLeft = new GUIStyle("minibuttonleft");
        buttonRight = new GUIStyle("minibuttonright");
        buttonMid = new GUIStyle("minibuttonmid");
        buttonLeft2 = new GUIStyle("ButtonLeft");
        buttonRight2 = new GUIStyle("ButtonRight");
        buttonMid2 = new GUIStyle("ButtonMid");
        playLabelStyle = new GUIStyle("LargeBoldLabel");
        playToggleStyle = new GUIStyle("MiniLabel");
        progressBarControllerStyle = new GUIStyle("PreSliderThumb");
        warningStyle = new GUIStyle("CN EntryWarnIconSmall");
    }

    void InitGUIContent()
    {
        if (guiContent) return;
        guiContent = true;

        // Create GUIContent
        playLabel = new GUIContent("Writer preview");
        eventToggleLabel = new GUIContent("Raise events");
        commandToggleLabel = new GUIContent("Execute commands");
        useDefaultDatabaseLabel = new GUIContent("Use default database");
        eventWarningContent = EditorGUIUtility.IconContent("alertDialog", "|When previewing from edit mode, ensure all the listeners you want to be invoked are set to \"Editor and Runtime\".");
    }

    public override void OnInspectorGUI()
    {
        InitStyles();
        InitGUIContent();

        switch (Event.current.type)
        {
            case EventType.Layout: PrepareLayout(); break;
            case EventType.MouseDown: if (writer.enabled) HandleMouseDown(); break;
            case EventType.MouseUp: if (writer.enabled) HandleMouseUp(); break;
            case EventType.MouseDrag: if (writer.enabled) HandleMouseDrag(); break;
        }

        // Reserve space  
        GUILayoutUtility.GetRect(width, playerRect.y + playerHeight + dividerHeight);

        EditorGUI.DrawRect(dividerRect, new Color(48f / 255, 44f / 255, 44f / 255));

        if (writer.enabled != wasEnabled)
        {
            // If disabled this frame
            if (!writer.enabled)
            {
                //StopWriter();
                wasWriting = false;
                progress = 0;
            }

            wasEnabled = writer.enabled;
        }

        RepaintInspector();

        if (serializedObject.hasModifiedProperties)
        {
            Undo.RecordObject(writer, "Writer modified");
            serializedObject.ApplyModifiedProperties();
        }

        if (GUI.changed)
        {
            Repaint();
        }
    }

    Vector3 tl = Vector3.zero;

    void PauseWriter()
    {
        //if (!writer.enabled) return;
        writer.StopWriter();
    }

    void StartWriter()
    {
        //if (!writer.enabled) return;
        if (progress >= 1)
        {
            writer.ResetWriter();
            progress = 0;
        }
        writer.StartWriter();
    }

    void ResetWriter()
    {
        //if (!writer.enabled) return;
        bool wasWriting = writer.IsWriting;
        writer.ResetWriter();
        progress = 0;
        if (wasWriting) writer.StartWriter();
    }

    void StopWriter()
    {
        //if (!writer.enabled) return;
        writer.ResetWriter();
        progress = 0;
    }

    void FinishWriter()
    {
        writer.FinishWriter();
        progress = 1;
    }

    public override bool RequiresConstantRepaint()
    {
        if (changed)
        {
            changed = false;
            return true;
        }

        return false;
    }
}
