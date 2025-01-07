using TMPEffects.Components;
using UnityEditor;
using UnityEngine;
using TMPEffects.CharacterData;
using System.Collections;

namespace TMPEffects.Editor
{
    [CustomEditor(typeof(TMPWriter))]
    public class TMPWriterEditor : UnityEditor.Editor
    {
        private static class Styles
        {
            public static readonly GUIContent commands = new GUIContent("Commands");
            public static readonly GUIContent events = new GUIContent("Events");
            public static readonly GUIContent writerSettings = new GUIContent("Writer Settings");

            public static readonly GUIContent delay = new GUIContent("Delay",
                "The delay between new characters shown by the writer, i.e. the inverse of the speed of the writer.");
            public static readonly GUIContent whiteSpaceDelay = new GUIContent("Whitespace Delay",
                "The delay after whitespace characters, either as percentage of the general delay or in seconds.");
            public static readonly GUIContent lineBreakDelay = new GUIContent("Linebreak Delay",
                "The delay after linebreaks, either as percentage of the general delay or in seconds.");
            public static readonly GUIContent punctuationDelay = new GUIContent("Punctuation Delay",
                "The delay after punctuation characters, either as percentage of the general delay or in seconds.");
            public static readonly GUIContent visibleDelay = new GUIContent("Visible Delay",
                "The delay after already visible characters, either as percentage of the general delay or in seconds.");

            public static readonly GUIContent database = new GUIContent("Use default database?",
                "The database used to process command tags (e.g. <!delay=0.05>.");
            public static readonly GUIContent maySkip = new GUIContent("May Skip",
                "Whether the text may be skipped by default.");
            public static readonly GUIContent writeOnStart = new GUIContent("Write On Start",
                "If checked, the writer will begin writing when it is first enabled. If not checked, you will have to manually start the writer from your own code.");
            public static readonly GUIContent writeOnNewText = new GUIContent("Write On New Text",
                "If checked, the writer will automatically begin writing when the text on the associated TMP_Text component is modified. If not checked, you will have to manually start the writer from your own code.");
            public static readonly GUIContent useScaledTime = new GUIContent("Use Scaled Time",
                "Whether the writer should use scaled time to wait for delays and wait commands.");
            public static readonly GUIContent sceneCommands = new GUIContent("Scene Commands",
                "Commands that may reference scene objects.\nNOT raised in preview mode.");
            
            public static readonly GUIContent playLabel = new GUIContent("Writer preview");
            public static readonly GUIContent eventToggleLabel = new GUIContent("Raise events");
            public static readonly GUIContent commandToggleLabel = new GUIContent("Execute commands");
            public static readonly GUIContent eventWarningContent = EditorGUIUtility.IconContent("alertDialog",
                "|When previewing from edit mode, ensure all the listeners you want to be invoked are set to \"Editor and Runtime\".");
            
            public static readonly GUIContent keywordDatabase = new GUIContent("Keyword Database",
                "A keyword database defining additional keywords. If the same keyword is present in the global keyword Database, this database will override it.");
            public static readonly GUIContent sceneKeywordDatabase = new GUIContent("Keyword Database",
                "A scene keyword database defining additional keywords. If the same keyword is present in the global keyword Database or keyword database on this writer, this database will override it.");
        }
        
        TMPWriter writer;

        float progress;
        bool clicked;
        bool wasWriting;
        bool wasEnabled;
        Coroutine hideCoroutine;

        // Serialized properties
        SerializedProperty databaseProp;
        SerializedProperty delayProp;
        SerializedProperty whitespaceDelayProp;
        SerializedProperty visibleDelayProp;
        SerializedProperty punctuationDelayProp;
        SerializedProperty linebreakDelayProp;
        SerializedProperty whitespaceDelayTypeProp;
        SerializedProperty visibleDelayTypeProp;
        SerializedProperty punctuationDelayTypeProp;
        SerializedProperty linebreakDelayTypeProp;
        SerializedProperty startOnPlayProp;
        SerializedProperty startOnNewTextProp;
        SerializedProperty eventsEnabledProp;
        SerializedProperty commandsEnabledProp;
        SerializedProperty onTextEventProp;
        SerializedProperty onShowCharacterProp;
        SerializedProperty onStartWriterProp;
        SerializedProperty onStopWriterProp;
        SerializedProperty onWaitStartedProp;
        SerializedProperty onWaitEndedProp;
        SerializedProperty onResetWriterProp;
        SerializedProperty onSkipWriterProp;
        SerializedProperty onFinishWriterProp;
        SerializedProperty maySkipProp;
        SerializedProperty useScaledTimeProp;
        SerializedProperty sceneCommandsProp;
        SerializedProperty useDefaultDatabaseProp;
        SerializedProperty useDefaultKeywordDatabaseProp;
        SerializedProperty initDatabaseProp;
        SerializedProperty keywordDatabaseProp;
        SerializedProperty sceneKeywordDatabaseProp;

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
            SerializedProperty delaysProp = serializedObject.FindProperty("delays");
            delayProp = delaysProp.FindPropertyRelative("delay");
            punctuationDelayProp = delaysProp.FindPropertyRelative("punctuationDelay");
            whitespaceDelayProp = delaysProp.FindPropertyRelative("whitespaceDelay");
            visibleDelayProp = delaysProp.FindPropertyRelative("visibleDelay");
            linebreakDelayProp = delaysProp.FindPropertyRelative("linebreakDelay");
            whitespaceDelayTypeProp = delaysProp.FindPropertyRelative("whitespaceDelayType");
            visibleDelayTypeProp = delaysProp.FindPropertyRelative("visibleDelayType");
            punctuationDelayTypeProp = delaysProp.FindPropertyRelative("punctuationDelayType");
            linebreakDelayTypeProp = delaysProp.FindPropertyRelative("linebreakDelayType");
            startOnPlayProp = serializedObject.FindProperty("writeOnStart");
            startOnNewTextProp = serializedObject.FindProperty("writeOnNewText");
            eventsEnabledProp = serializedObject.FindProperty("eventsEnabled");
            commandsEnabledProp = serializedObject.FindProperty("commandsEnabled");
            onTextEventProp = serializedObject.FindProperty("OnTextEvent");
            onShowCharacterProp = serializedObject.FindProperty("OnCharacterShown");
            onStartWriterProp = serializedObject.FindProperty("OnStartWriter");
            onStopWriterProp = serializedObject.FindProperty("OnStopWriter");
            onWaitStartedProp = serializedObject.FindProperty("OnWaitStarted");
            onWaitEndedProp = serializedObject.FindProperty("OnWaitEnded");
            onResetWriterProp = serializedObject.FindProperty("OnResetWriter");
            onSkipWriterProp = serializedObject.FindProperty("OnSkipWriter");
            onFinishWriterProp = serializedObject.FindProperty("OnFinishWriter");
            maySkipProp = serializedObject.FindProperty("maySkip");
            useScaledTimeProp = serializedObject.FindProperty("useScaledTime");
            sceneCommandsProp = serializedObject.FindProperty("sceneCommands");
            useDefaultDatabaseProp = serializedObject.FindProperty("useDefaultDatabase");
            useDefaultKeywordDatabaseProp = serializedObject.FindProperty("useDefaultKeywordDatabase");
            initDatabaseProp = serializedObject.FindProperty("initDatabase");
            keywordDatabaseProp = serializedObject.FindProperty("keywordDatabase");
            sceneKeywordDatabaseProp = serializedObject.FindProperty("sceneKeywordDatabase");

            // Load Textures
            playButtonTexture = (Texture)Resources.Load("PlayerIcons/playButton");
            pauseButtonTexture = (Texture)Resources.Load("PlayerIcons/pauseButton");
            resetButtonTexture = (Texture)Resources.Load("PlayerIcons/resetButton");
            stopButtonTexture = (Texture)Resources.Load("PlayerIcons/stopButton");
            skipButtonTexture = (Texture)Resources.Load("PlayerIcons/skipButton");

            // Other Initialization work
            writer = target as TMPWriter;
            progress = 0f;
            clicked = false;
            wasWriting = false;
            wasEnabled = writer.enabled;

            writer.OnResetWriterPreview -= CancelHideAfterFinish;
            writer.OnResetWriterPreview += CancelHideAfterFinish;
            writer.OnCharacterShownPreview -= CancelHideAfterFinish;
            writer.OnCharacterShownPreview += CancelHideAfterFinish;
            writer.OnSkipWriterPreview -= CancelHideAfterFinish;
            writer.OnSkipWriterPreview += CancelHideAfterFinish;
            writer.OnStartWriterPreview -= CancelHideAfterFinish;
            writer.OnStartWriterPreview += CancelHideAfterFinish;
            writer.OnStopWriterPreview -= CancelHideAfterFinish;
            writer.OnStopWriterPreview += CancelHideAfterFinish;
            writer.OnWaitStartedPreview -= CancelHideAfterFinish;
            writer.OnWaitStartedPreview += CancelHideAfterFinish;
            writer.OnWaitEndedPreview -= CancelHideAfterFinish;
            writer.OnWaitEndedPreview += CancelHideAfterFinish;
            writer.OnFinishWriterPreview -= StartHideAfterFinish;
            writer.OnFinishWriterPreview += StartHideAfterFinish;

            writer.OnResetWriterPreview -= UpdateProgress;
            writer.OnResetWriterPreview += UpdateProgress;
            writer.OnCharacterShownPreview -= UpdateProgress;
            writer.OnCharacterShownPreview += UpdateProgress;
            writer.OnFinishWriterPreview -= UpdateProgressFinish;
            writer.OnFinishWriterPreview += UpdateProgressFinish;
            writer.OnCharacterShown.RemoveListener(UpdateProgress);
            writer.OnCharacterShown.AddListener(UpdateProgress);
            writer.OnResetWriter.RemoveListener(UpdateProgress);
            writer.OnResetWriter.AddListener(UpdateProgress);
            writer.OnFinishWriter.RemoveListener(UpdateProgressFinish);
            writer.OnFinishWriter.AddListener(UpdateProgressFinish);

            writer.OnResetComponent -= SetResetDatabase;
            writer.OnResetComponent += SetResetDatabase;

            InitDatabase();
        }

        private void OnDisable()
        {
            writer.OnResetWriterPreview -= CancelHideAfterFinish;
            writer.OnCharacterShownPreview -= CancelHideAfterFinish;
            writer.OnSkipWriterPreview -= CancelHideAfterFinish;
            writer.OnStartWriterPreview -= CancelHideAfterFinish;
            writer.OnStopWriterPreview -= CancelHideAfterFinish;
            writer.OnWaitStartedPreview -= CancelHideAfterFinish;
            writer.OnWaitEndedPreview -= CancelHideAfterFinish;
            writer.OnFinishWriterPreview -= StartHideAfterFinish;

            writer.OnResetWriterPreview -= UpdateProgress;
            writer.OnCharacterShownPreview -= UpdateProgress;
            writer.OnFinishWriterPreview -= UpdateProgressFinish;
            writer.OnCharacterShown.RemoveListener(UpdateProgress);
            writer.OnResetWriter.RemoveListener(UpdateProgress);
            writer.OnFinishWriter.RemoveListener(UpdateProgressFinish);

            writer.OnResetComponent -= SetResetDatabase;
        }

        private void SetResetDatabase()
        {
            reset = true;
        }

        private void InitDatabase()
        {
            if (initDatabaseProp.boolValue) return;
            initDatabaseProp.boolValue = true;

            SetDatabase();
            SetKeywordDatabase();

            serializedObject.ApplyModifiedProperties();
        }

        bool reset = false;

        private void ResetDatabase()
        {
            if (!reset) return;
            reset = false;

            SetDatabase();
        }

        private void SetDatabase()
        {
            if (!useDefaultDatabaseProp.boolValue)
            {
                serializedObject.ApplyModifiedProperties();
                return;
            }

            if (TMPEffectsSettings.DefaultCommandDatabase == null)
            {
                useDefaultDatabaseProp.boolValue = false;
                serializedObject.ApplyModifiedProperties();
                return;
            }

            if (databaseProp.objectReferenceValue != TMPEffectsSettings.DefaultCommandDatabase)
            {
                databaseProp.objectReferenceValue = TMPEffectsSettings.DefaultCommandDatabase;
                serializedObject.ApplyModifiedProperties();
                writer.OnChangedDatabase();
                return;
            }
        }

        private void SetKeywordDatabase()
        {
            if (!useDefaultKeywordDatabaseProp.boolValue)
            {
                serializedObject.ApplyModifiedProperties();
                return;
            }

            if (TMPEffectsSettings.DefaultKeywordDatabase == null)
            {
                useDefaultKeywordDatabaseProp.boolValue = false;
                serializedObject.ApplyModifiedProperties();
                return;
            }

            if (keywordDatabaseProp.objectReferenceValue != TMPEffectsSettings.DefaultKeywordDatabase)
            {
                keywordDatabaseProp.objectReferenceValue = TMPEffectsSettings.DefaultKeywordDatabase;
                serializedObject.ApplyModifiedProperties();
                writer.OnChangedDatabase();
                return;
            }
        }

        void UpdateProgress(TMPWriter writer, CharData cData) => UpdateProgress(writer, cData.info.index);

        void UpdateProgress(TMPWriter writer, int index)
        {
            progress = Mathf.Lerp(0f, 1f, (float)index / (writer.TextComponent.textInfo.characterCount - 1));
            Repaint();
        }

        void UpdateProgressFinish(TMPWriter writer)
        {
            progress = 1f;
            Repaint();
        }

        private void StartHideAfterFinish(TMPWriter writer)
        {
            //CancelHideAfterFinish();
            hideCoroutine = writer.StartCoroutine(HideAfterFinish());
        }

        private void CancelHideAfterFinish(TMPWriter writer, float _) => CancelHideAfterFinish(writer);
        private void CancelHideAfterFinish(TMPWriter writer, CharData _) => CancelHideAfterFinish(writer);
        private void CancelHideAfterFinish(TMPWriter writer, int _) => CancelHideAfterFinish(writer);

        private void CancelHideAfterFinish(TMPWriter writer)
        {
            if (hideCoroutine != null)
            {
                writer.StopCoroutine(hideCoroutine);
                hideCoroutine = null;
            }
        }

        IEnumerator HideAfterFinish()
        {
            float passed = 0f;
            while (passed < 1f)
            {
                yield return null;
                passed += Time.deltaTime;
                EditorApplication.QueuePlayerLoopUpdate();
            }

            yield return null;
            writer.HideAll();
        }

        // this totally sucks but works and looks nice so not worth a rewrite for now
        void PrepareLayout()
        {
            width = EditorGUIUtility.currentViewWidth - 30;

            // Calculate wrappings
            wrapHeader = width < playLabelWidth + eventToggleWidth + commandToggleWidth - 10;
            wrapPlayer = width < (playButtonWidth + (buttonWidth * 3)) * 3f;

            // Calculate heights
            playLabelHeight = playLabelStyle.CalcHeight(Styles.playLabel, playLabelWidth);
            eventToggleHeight = playToggleStyle.CalcHeight(Styles.eventToggleLabel, eventToggleWidth);
            commandToggleHeight = playToggleStyle.CalcHeight(Styles.commandToggleLabel, commandToggleWidth);
            playButtonHeight = 30;
            buttonHeight = 20;
            progressBarThickness = 10;
            headerHeight = (wrapHeader
                ? playLabelHeight + playToggleStyle.lineHeight + Mathf.Max(eventToggleHeight, commandToggleHeight)
                : Mathf.Max(playLabelHeight, eventToggleHeight, commandToggleHeight));
            playerHeight =
                (wrapPlayer ? playButtonHeight + progressBarThickness + progressBarYOffset : playButtonHeight);
            dividerHeight = 15;

            // Prepare rects
            headerRect = new Rect(xOffset, yOffset, width, headerHeight);
            playerRect = new Rect(xOffset, yOffset + headerHeight + headerPlayerOffset, width, playerHeight);
            dividerRect = new Rect(xOffset, playerRect.y + playerRect.height + 9.5f, width + xOffset, 1f);

            playLabelRect = new Rect(headerRect.position, new Vector2(playLabelWidth, playLabelHeight));
            if (wrapHeader)
            {
                eventToggleRect = new Rect(headerRect.x,
                    headerRect.y + playLabelHeight + (playToggleStyle.lineHeight / 2), eventToggleWidth - 20,
                    eventToggleHeight);
                commandToggleRect = new Rect(headerRect.x + eventToggleWidth,
                    headerRect.y + playLabelHeight + (playToggleStyle.lineHeight / 2), commandToggleWidth,
                    commandToggleHeight);
            }
            else
            {
                eventToggleRect = new Rect(headerRect.x + playLabelWidth, headerRect.y, eventToggleWidth - 20,
                    headerHeight);
                commandToggleRect = new Rect(headerRect.x + playLabelWidth + eventToggleWidth, headerRect.y,
                    commandToggleWidth, headerHeight);
            }

            eventWarningRect = new Rect(eventToggleRect.x + eventToggleRect.width - 5, eventToggleRect.y, 20, 20);
            playButtonRect = new Rect(playerRect.x, playerRect.y, playButtonWidth, playButtonHeight);
            resetButtonRect = new Rect(playerRect.x + playButtonWidth + buttonOffset, playerRect.y + (buttonHeight / 4),
                buttonWidth, buttonHeight);
            stopButtonRect = new Rect(resetButtonRect.x + buttonWidth, playerRect.y + (buttonHeight / 4), buttonWidth,
                buttonHeight);
            skipButtonRect = new Rect(stopButtonRect.x + buttonWidth, playerRect.y + (buttonHeight / 4), buttonWidth,
                buttonHeight);

            resetButtonRect.y += buttonHeight / 4;
            stopButtonRect.y += buttonHeight / 4;
            skipButtonRect.y += buttonHeight / 4;
            if (wrapPlayer)
            {
                progressBarRect = new Rect(playerRect.x + 5, playerRect.y + playButtonHeight + progressBarYOffset,
                    width - progressBarXOffset + 10, progressBarThickness);
            }
            else
            {
                progressBarRect = new Rect(skipButtonRect.x + skipButtonRect.width + progressBarXOffset,
                    playerRect.y + (playButtonHeight / 2) - (progressBarThickness / 2),
                    width - (skipButtonRect.x - playerRect.x + skipButtonRect.width + progressBarXOffset) + 5,
                    progressBarThickness);
                progressBarRect.y += buttonHeight / 4;
            }

            progressBarControllerRect = new Rect(progressBarRect.position.x + (progressBarRect.width * progress) - 2,
                progressBarRect.y, progressBarThickness, progressBarThickness);
        }

        void DrawPlayer()
        {
            // Draw play label
            EditorGUI.LabelField(playLabelRect, Styles.playLabel, playLabelStyle);

            EditorGUI.BeginDisabledGroup(!PlayerEnabled());

            // Draw play toggles
            eventsEnabledProp.boolValue =
                EditorGUI.ToggleLeft(eventToggleRect, Styles.eventToggleLabel, eventsEnabledProp.boolValue);

            if (eventsEnabledProp.boolValue)
            {
                GUI.Label(eventWarningRect, Styles.eventWarningContent);
            }

            commandsEnabledProp.boolValue =
                EditorGUI.ToggleLeft(commandToggleRect, Styles.commandToggleLabel, commandsEnabledProp.boolValue);

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

        UnityEngine.Object GetDefaultCommandDatabase()
        {
            var database = TMPEffectsSettings.DefaultCommandDatabase;
            if (database == null) return null;

            if (database == null)
                Debug.LogWarning("No default command database set in Project Settings/TMPEffects");
            return database;
        }

        UnityEngine.Object GetDefaultKeywordDatabase()
        {
            var database = TMPEffectsSettings.DefaultKeywordDatabase;
            if (database == null) return null;

            if (database == null)
                Debug.LogWarning("No default keyword database set in Project Settings/TMPEffects");
            return database;
        }

        void DrawCommandsFoldout()
        {
            if (reset) ResetDatabase();

            databaseProp.isExpanded =
                EditorGUILayout.Foldout(databaseProp.isExpanded, Styles.commands, true);
            if (databaseProp.isExpanded)
            {
                EditorGUI.indentLevel++;

                if (TMPEffectsDrawerUtility.DrawDefaultableDatabase(databaseProp, useDefaultDatabaseProp,
                        Styles.database, GetDefaultCommandDatabase, "command"))
                {
                    writer.OnChangedDatabase();
                }

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(sceneCommandsProp, Styles.sceneCommands);
                if (EditorGUI.EndChangeCheck())
                {
                    // SerializedObservableDictionary does not raise events when the operations involves
                    // (de)serializing the actual object (as opposed to the contained objects).
                    // Could be solved by using a custom interface, instead of INotifyPropertyChanged
                    // that passes a bool "delay". If bool is set, schedule the mesh reprocess (inside of TMPAnimator)
                    // instead of instantly performing it.
                    // Alternatively, simpler approach is to always schedule reprocesses, and then execute them
                    // in Update of TMPAnimator.
                    if (serializedObject.hasModifiedProperties) serializedObject.ApplyModifiedProperties();
                    writer.OnChangedDatabase();
                }

                EditorGUI.indentLevel--;
            }
        }

        static bool eventFoldout = false;

        void DrawEventsFoldout()
        {
            eventFoldout = EditorGUILayout.Foldout(eventFoldout, Styles.events, true);

            if (eventFoldout)
            {
                EditorGUILayout.PropertyField(onTextEventProp);
                EditorGUILayout.PropertyField(onShowCharacterProp);
                EditorGUILayout.PropertyField(onStartWriterProp);
                EditorGUILayout.PropertyField(onStopWriterProp);
                EditorGUILayout.PropertyField(onWaitStartedProp);
                EditorGUILayout.PropertyField(onWaitEndedProp);
                EditorGUILayout.PropertyField(onResetWriterProp);
                EditorGUILayout.PropertyField(onSkipWriterProp);
                EditorGUILayout.PropertyField(onFinishWriterProp);
            }
        }

        private bool delayFoldout;

        void RepaintInspector()
        {
            DrawPlayer();

            EditorGUILayout.BeginHorizontal();
            delayFoldout = EditorGUILayout.Foldout(delayFoldout, Styles.delay, true);
            var pre = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 0;
            EditorGUILayout.PropertyField(delayProp, GUIContent.none);
            EditorGUIUtility.labelWidth = pre;
            EditorGUILayout.EndHorizontal();
            if (delayFoldout)
            {
                EditorGUI.indentLevel++;

                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(Styles.whiteSpaceDelay,
                    GUILayout.ExpandWidth(false), GUILayout.MaxWidth(120));
                whitespaceDelayTypeProp.enumValueIndex =
                    (int)(TMPWriter.DelayType)EditorGUILayout.EnumPopup(
                        (TMPWriter.DelayType)whitespaceDelayTypeProp.enumValueIndex, GUILayout.MaxWidth(100));
                whitespaceDelayProp.floatValue = EditorGUILayout.FloatField(GUIContent.none,
                    whitespaceDelayProp.floatValue, GUILayout.ExpandWidth(true));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(Styles.lineBreakDelay,
                    GUILayout.ExpandWidth(false), GUILayout.MaxWidth(120));
                linebreakDelayTypeProp.enumValueIndex =
                    (int)(TMPWriter.DelayType)EditorGUILayout.EnumPopup(
                        (TMPWriter.DelayType)linebreakDelayTypeProp.enumValueIndex, GUILayout.MaxWidth(100));
                linebreakDelayProp.floatValue = EditorGUILayout.FloatField(GUIContent.none,
                    linebreakDelayProp.floatValue, GUILayout.ExpandWidth(true));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(Styles.punctuationDelay,
                    GUILayout.ExpandWidth(false), GUILayout.MaxWidth(120));
                punctuationDelayTypeProp.enumValueIndex =
                    (int)(TMPWriter.DelayType)EditorGUILayout.EnumPopup(
                        (TMPWriter.DelayType)punctuationDelayTypeProp.enumValueIndex, GUILayout.MaxWidth(100));
                punctuationDelayProp.floatValue = EditorGUILayout.FloatField(GUIContent.none,
                    punctuationDelayProp.floatValue, GUILayout.ExpandWidth(true));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(Styles.visibleDelay,
                    GUILayout.ExpandWidth(false), GUILayout.MaxWidth(120));
                visibleDelayTypeProp.enumValueIndex =
                    (int)(TMPWriter.DelayType)EditorGUILayout.EnumPopup(
                        (TMPWriter.DelayType)visibleDelayTypeProp.enumValueIndex, GUILayout.MaxWidth(100));
                visibleDelayProp.floatValue = EditorGUILayout.FloatField(GUIContent.none, visibleDelayProp.floatValue,
                    GUILayout.ExpandWidth(true));
                GUILayout.EndHorizontal();

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space(10);
            DrawCommandsFoldout();

            EditorGUILayout.Space(10);
            startOnPlayProp.isExpanded =
                EditorGUILayout.Foldout(startOnPlayProp.isExpanded, Styles.writerSettings, true);
            if (startOnPlayProp.isExpanded)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(startOnPlayProp, Styles.writeOnStart);
                EditorGUILayout.PropertyField(startOnNewTextProp, Styles.writeOnNewText);
                EditorGUILayout.PropertyField(maySkipProp, Styles.maySkip);
                EditorGUILayout.PropertyField(useScaledTimeProp, Styles.useScaledTime);

                EditorGUILayout.Space(10);
                if (TMPEffectsDrawerUtility.DrawDefaultableDatabase(keywordDatabaseProp, useDefaultKeywordDatabaseProp,
                        Styles.keywordDatabase, GetDefaultKeywordDatabase, "keyword"))
                {
                    writer.OnChangedDatabase();
                }

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(sceneKeywordDatabaseProp, Styles.sceneKeywordDatabase);
                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();
                    writer.OnChangedDatabase();
                }

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space(10);
            DrawEventsFoldout();
        }

        void HandleMouseDown()
        {
            if (!PlayerEnabled()) return;

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
            if (!PlayerEnabled()) return;
            if (clicked)
            {
                if (wasWriting)
                    writer.StartWriter();
            }

            clicked = false;
        }

        void HandleMouseDrag()
        {
            if (!PlayerEnabled()) return;
            if (!clicked) return;

            float xPos = Event.current.mousePosition.x;

            float min = progressBarRect.x;
            float max = progressBarRect.x + progressBarRect.width;

            progress = Mathf.InverseLerp(min, max, xPos);

            writer.SetWriter(Mathf.RoundToInt(((writer.TextComponent.textInfo.characterCount - 1) * progress)));
            EditorApplication.QueuePlayerLoopUpdate();
        }

        bool styles = false;

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

        public override void OnInspectorGUI()
        {
            InitStyles();

            switch (Event.current.type)
            {
                case EventType.Layout:
                    PrepareLayout();
                    break;
                case EventType.MouseDown:
                    if (writer.enabled) HandleMouseDown();
                    break;
                case EventType.MouseUp:
                    if (writer.enabled) HandleMouseUp();
                    break;
                case EventType.MouseDrag:
                    if (writer.enabled) HandleMouseDrag();
                    break;
            }

            // Reserve space
            GUILayoutUtility.GetRect(width, playerRect.y + playerHeight + dividerHeight);

            EditorGUI.DrawRect(dividerRect, new Color(48f / 255, 44f / 255, 44f / 255));

            if (writer.enabled != wasEnabled)
            {
                // If disabled this frame
                if (!writer.enabled)
                {
                    wasWriting = false;
                    progress = 0;
                }

                wasEnabled = writer.enabled;
            }

            RepaintInspector();

            if (serializedObject.hasModifiedProperties)
            {
                serializedObject.ApplyModifiedProperties();
            }

            if (GUI.changed)
            {
                Repaint();
            }
        }

        bool PlayerEnabled()
        {
            return writer.isActiveAndEnabled && !Application.isPlaying;
        }

        void PauseWriter()
        {
            writer.StopWriter();
            EditorApplication.QueuePlayerLoopUpdate();
        }

        void StartWriter()
        {
            if (progress >= 1)
            {
                writer.ResetWriter();
                progress = 0;
            }

            writer.StartWriter();
            EditorApplication.QueuePlayerLoopUpdate();
        }

        void ResetWriter()
        {
            bool wasWriting = writer.IsWriting;
            writer.ResetWriter();
            progress = 0;
            if (wasWriting) writer.StartWriter();
            else
            {
                writer.Show(0, writer.TextComponent.textInfo.characterCount, true);
            }

            EditorApplication.QueuePlayerLoopUpdate();
        }

        void StopWriter()
        {
            writer.ResetWriter();
            progress = 0;
            writer.Show(0, writer.TextComponent.textInfo.characterCount, true);
            EditorApplication.QueuePlayerLoopUpdate();
        }

        void FinishWriter()
        {
            writer.SkipPlayer();
            progress = 1;
            EditorApplication.QueuePlayerLoopUpdate();
        }
    }
}