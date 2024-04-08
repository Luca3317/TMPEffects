using TMPEffects.Components;
using UnityEditor;
using UnityEngine;
using TMPEffects.Databases.CommandDatabase;
using TMPEffects.CharacterData;
using TMPEffects.Databases.AnimationDatabase;
using System.Collections;

namespace TMPEffects.Editor
{
    [CustomEditor(typeof(TMPWriter))]
    public class TMPWriterEditor : UnityEditor.Editor
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
        Coroutine hideCoroutine;
        TMPCommandDatabase defaultDatabase;

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
        SerializedProperty eventsEnabledProp;
        SerializedProperty commandsEnabledProp;
        SerializedProperty onTextEventProp;
        SerializedProperty onShowCharacterProp;
        SerializedProperty onStartWriterProp;
        SerializedProperty onStopWriterProp;
        SerializedProperty onResetWriterProp;
        SerializedProperty onSkipWriterProp;
        SerializedProperty onFinishWriterProp;
        SerializedProperty maySkipProp;
        SerializedProperty sceneCommandsProp;

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
            delayProp = serializedObject.FindProperty("delay");
            punctuationDelayProp = serializedObject.FindProperty("punctuationDelay");
            whitespaceDelayProp = serializedObject.FindProperty("whiteSpaceDelay");
            visibleDelayProp = serializedObject.FindProperty("visibleDelay");
            linebreakDelayProp = serializedObject.FindProperty("linebreakDelay");
            whitespaceDelayTypeProp = serializedObject.FindProperty("whiteSpaceDelayType");
            visibleDelayTypeProp = serializedObject.FindProperty("visibleDelayType");
            punctuationDelayTypeProp = serializedObject.FindProperty("punctuationDelayType");
            linebreakDelayTypeProp = serializedObject.FindProperty("linebreakDelayType");
            startOnPlayProp = serializedObject.FindProperty("writeOnStart");
            eventsEnabledProp = serializedObject.FindProperty("eventsEnabled");
            commandsEnabledProp = serializedObject.FindProperty("commandsEnabled");
            onTextEventProp = serializedObject.FindProperty("OnTextEvent");
            onShowCharacterProp = serializedObject.FindProperty("OnCharacterShown");
            onStartWriterProp = serializedObject.FindProperty("OnStartWriter");
            onStopWriterProp = serializedObject.FindProperty("OnStopWriter");
            onResetWriterProp = serializedObject.FindProperty("OnResetWriter");
            onSkipWriterProp = serializedObject.FindProperty("OnSkipWriter");
            onFinishWriterProp = serializedObject.FindProperty("OnFinishWriter");
            maySkipProp = serializedObject.FindProperty("maySkip");
            sceneCommandsProp = serializedObject.FindProperty("sceneCommands");

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

            defaultDatabase = (TMPCommandDatabase)Resources.Load("DefaultCommandDatabase");
            useDefaultDatabase = defaultDatabase == databaseProp.objectReferenceValue || !serializedObject.FindProperty("initValidate").boolValue;

            if (!serializedObject.FindProperty("initValidate").boolValue)
            {
                databaseProp.objectReferenceValue = defaultDatabase;
                serializedObject.ApplyModifiedProperties();
            }
        }

        private void OnDisable()
        {
            writer.OnCharacterShown.RemoveListener(UpdateProgress);
            writer.OnResetWriter.RemoveListener(UpdateProgress);
        }

        void UpdateProgress(CharData cData) => UpdateProgress(cData.info.index);

        void UpdateProgress(int index)
        {
            progress = Mathf.Lerp(0f, 1f, (float)index / (writer.TextComponent.textInfo.characterCount - 1));
            Repaint();
        }

        void UpdateProgressFinish()
        {
            progress = 1f;
            Repaint();
        }

        private void StartHideAfterFinish()
        {
            //CancelHideAfterFinish();
            hideCoroutine = writer.StartCoroutine(HideAfterFinish());
        }

        private void CancelHideAfterFinish(CharData cdata) => CancelHideAfterFinish();
        private void CancelHideAfterFinish(int index) => CancelHideAfterFinish();
        private void CancelHideAfterFinish()
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
                eventToggleRect = new Rect(headerRect.x, headerRect.y + playLabelHeight + playToggleStyle.lineHeight / 2, eventToggleWidth - 20, eventToggleHeight);
                commandToggleRect = new Rect(headerRect.x + eventToggleWidth, headerRect.y + playLabelHeight + playToggleStyle.lineHeight / 2, commandToggleWidth, commandToggleHeight);
            }
            else
            {
                eventToggleRect = new Rect(headerRect.x + playLabelWidth, headerRect.y, eventToggleWidth - 20, headerHeight);
                commandToggleRect = new Rect(headerRect.x + playLabelWidth + eventToggleWidth, headerRect.y, commandToggleWidth, headerHeight);
            }
            eventWarningRect = new Rect(eventToggleRect.x + eventToggleRect.width - 5, eventToggleRect.y, 20, 20);
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

            EditorGUI.BeginDisabledGroup(!PlayerEnabled());

            // Draw play toggles
            eventsEnabledProp.boolValue = EditorGUI.ToggleLeft(eventToggleRect, eventToggleLabel, eventsEnabledProp.boolValue);

            if (eventsEnabledProp.boolValue)
            {
                GUI.Label(eventWarningRect, eventWarningContent);
            }

            commandsEnabledProp.boolValue = EditorGUI.ToggleLeft(commandToggleRect, commandToggleLabel, commandsEnabledProp.boolValue);

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
                Undo.RecordObject(writer, "Modified writer");
            }

            if (useDefaultDatabase)
            {
                if (databaseProp.objectReferenceValue != defaultDatabase)
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

        void DrawCommandsFoldout()
        {
            databaseProp.isExpanded = EditorGUILayout.Foldout(databaseProp.isExpanded, new GUIContent("Commands"), true);
            if (databaseProp.isExpanded)
            {
                EditorGUI.indentLevel++;
                DrawDatabase();
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(sceneCommandsProp);
                if (EditorGUI.EndChangeCheck()) writer.ForceReprocess();
                EditorGUI.indentLevel--;
            }
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
                EditorGUILayout.PropertyField(onSkipWriterProp);
                EditorGUILayout.PropertyField(onFinishWriterProp);
            }
        }

        private bool delayFoldout;
        void RepaintInspector()
        {
            DrawPlayer();

            EditorGUILayout.BeginHorizontal();
            delayFoldout = EditorGUILayout.Foldout(delayFoldout, "Delay");
            var pre = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 0;
            EditorGUILayout.PropertyField(delayProp, new GUIContent(""));
            EditorGUIUtility.labelWidth = pre;
            EditorGUILayout.EndHorizontal();
            if (delayFoldout)
            {
                EditorGUI.indentLevel++;

                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(new GUIContent("Whitespace Delay", whitespaceDelayProp.tooltip), GUILayout.ExpandWidth(false), GUILayout.MaxWidth(120));
                whitespaceDelayTypeProp.enumValueIndex = (int)(TMPWriter.DelayType)EditorGUILayout.EnumPopup((TMPWriter.DelayType)whitespaceDelayTypeProp.enumValueIndex, GUILayout.MaxWidth(100));
                whitespaceDelayProp.floatValue = EditorGUILayout.FloatField(GUIContent.none, whitespaceDelayProp.floatValue, GUILayout.ExpandWidth(true));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(new GUIContent("Linebreak Delay", linebreakDelayProp.tooltip), GUILayout.ExpandWidth(false), GUILayout.MaxWidth(120));
                linebreakDelayTypeProp.enumValueIndex = (int)(TMPWriter.DelayType)EditorGUILayout.EnumPopup((TMPWriter.DelayType)linebreakDelayTypeProp.enumValueIndex, GUILayout.MaxWidth(100));
                linebreakDelayProp.floatValue = EditorGUILayout.FloatField(GUIContent.none, linebreakDelayProp.floatValue, GUILayout.ExpandWidth(true));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(new GUIContent("Punctuation Delay", punctuationDelayProp.tooltip), GUILayout.ExpandWidth(false), GUILayout.MaxWidth(120));
                punctuationDelayTypeProp.enumValueIndex = (int)(TMPWriter.DelayType)EditorGUILayout.EnumPopup((TMPWriter.DelayType)punctuationDelayTypeProp.enumValueIndex, GUILayout.MaxWidth(100));
                punctuationDelayProp.floatValue = EditorGUILayout.FloatField(GUIContent.none, punctuationDelayProp.floatValue, GUILayout.ExpandWidth(true));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(new GUIContent("Visible Delay", visibleDelayProp.tooltip), GUILayout.ExpandWidth(false), GUILayout.MaxWidth(120));
                visibleDelayTypeProp.enumValueIndex = (int)(TMPWriter.DelayType)EditorGUILayout.EnumPopup((TMPWriter.DelayType)visibleDelayTypeProp.enumValueIndex, GUILayout.MaxWidth(100));
                visibleDelayProp.floatValue = EditorGUILayout.FloatField(GUIContent.none, visibleDelayProp.floatValue, GUILayout.ExpandWidth(true));
                GUILayout.EndHorizontal();

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.PropertyField(startOnPlayProp);
            EditorGUILayout.PropertyField(maySkipProp);

            EditorGUILayout.Space(10);
            DrawCommandsFoldout();

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

        bool PlayerEnabled()
        {
            return writer.isActiveAndEnabled && !Application.isPlaying;
        }

        void PauseWriter()
        {
            writer.StopWriter();
        }

        void StartWriter()
        {
            if (progress >= 1)
            {
                writer.ResetWriter();
                progress = 0;
            }
            writer.StartWriter();
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
        }

        void StopWriter()
        {
            writer.ResetWriter();
            progress = 0;
            writer.Show(0, writer.TextComponent.textInfo.characterCount, true);
        }

        void FinishWriter()
        {
            writer.SkipPlayer();
            progress = 1;
        }
    }
}

