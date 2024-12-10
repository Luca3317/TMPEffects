using System;
using UnityEngine;
using UnityEditor;
using TMPEffects.Components;
using UnityEditorInternal;
using System.Collections.Generic;
using System.Reflection;
using TMPEffects.Components.Animator;
using TMPEffects.Databases;
using Object = UnityEngine.Object;

namespace TMPEffects.Editor
{
    [CustomEditor(typeof(TMPAnimator))]
    public class TMPAnimatorEditor : UnityEditor.Editor
    {
        private static class Styles
        {
            public static readonly GUIStyle playLabelStyle = new GUIStyle("LargeBoldLabel");

            public static readonly GUIContent database = new GUIContent("Use default database?",
                "The database used for processing animation tags (e.g. <wave>, <+fade>).");

            public static readonly GUIContent updateFrom = new GUIContent("Update From",
                "Where to update the animations from. If set to Script, you will have to manually update animations from your own script.");

            public static readonly GUIContent animateOnStart = new GUIContent("Animate On Start",
                "Whether to automatically start animating when entering playmode. Ignored if UpdateFrom is set to Script");

            public static readonly GUIContent animationsOverride = new GUIContent("Animations Override",
                "Whether animation tags override each other by default. You can set this individually on a per-tag basis by adding the override=true/false parameter to them.");

            public static readonly GUIContent excludedCharacters = new GUIContent("Excluded Characters",
                "Characters that are excluded from basic animations.");

            public static readonly GUIContent showExcludedCharacters = new GUIContent("Excluded Show Characters",
                "Characters that are excluded from show animations.");

            public static readonly GUIContent hideExcludedCharacters = new GUIContent("Excluded Hide Characters",
                "Characters that are excluded from hide animations.");

            public static readonly GUIContent excludePunctuation = new GUIContent("Exclude Punctuation",
                "Whether to exclude punctuation characters from animations.");


            public static readonly GUIContent alertDialogDefaultShow =
                new GUIContent((Texture)EditorGUIUtility.Load("alertDialog"));

            public static readonly GUIContent defaultAnimations = new GUIContent("Default Animations",
                "Default animations that will be applied to the entire text, without needing any tags.");

            public static readonly GUIContent defaultShowAnimations = new GUIContent("Default Show Animations",
                "Default show animations that will be applied to the entire text, without needing any tags.");

            public static readonly GUIContent defaultHideAnimations = new GUIContent("Default Hide Animations",
                "Default hide animations that will be applied to the entire text, without needing any tags.");

            public static readonly GUIContent databaseFoldoutLabel = new GUIContent("Animations");
            public static readonly GUIContent basic = new GUIContent("Basic");
            public static readonly GUIContent show = new GUIContent("Show");
            public static readonly GUIContent hide = new GUIContent("Hide");

            public static readonly GUIContent animatorPreview = new GUIContent("Animator Preview");
            public static readonly GUIContent resetTime = new GUIContent("Reset Time");
            public static readonly GUIContent updatesPerSecond = new GUIContent("Updates Per Second");
            public static readonly GUIContent previewTimeScale = new GUIContent("Preview Time Scale");

            public static readonly GUIContent keywordDatabase = new GUIContent("Keyword Database",
                "A keyword database defining additional keywords. " +
                "If the same keyword is present in the global keyword Database, this database will override it.");

            public static readonly GUIContent sceneKeywordDatabase = new GUIContent("Scene Keyword Database",
                "A scene keyword database defining additional keywords. " +
                "If the same keyword is present in the global keyword Database or keyword database on this animator, this database will override it.");

            public static readonly GUIContent animationSettings = new GUIContent("Animation Settings");
            public static readonly GUIContent animatorSettings = new GUIContent("Animator Settings");

            // Styles
            public static readonly GUIStyle previewLabelStyle = new GUIStyle("LargeBoldLabel");
        }


        TMPAnimator animator;

        // Serialized properties
        SerializedProperty databaseProp;
        SerializedProperty updateFromProp;
        SerializedProperty animateOnStartProp;
        SerializedProperty animationsOverrideProp;
        SerializedProperty contextProp;
        SerializedProperty contextScalingProp;
        SerializedProperty contextUniformScalingProp;
        SerializedProperty contextScaledTimeProp;
        SerializedProperty previewProp;
        SerializedProperty excludedProp;
        SerializedProperty excludedShowProp;
        SerializedProperty excludedHideProp;
        SerializedProperty excludePunctuationProp;
        SerializedProperty excludePunctuationShowProp;
        SerializedProperty excludePunctuationHideProp;
        SerializedProperty sceneAnimationsProp;
        SerializedProperty sceneShowAnimationsProp;
        SerializedProperty sceneHideAnimationsProp;
        SerializedProperty defaultShowStringProp;
        SerializedProperty defaultHideStringProp;
        SerializedProperty useDefaultDatabaseProp;
        SerializedProperty useDefaultKeywordDatabaseProp;
        SerializedProperty initDatabaseProp;
        SerializedProperty previewUpdatesProp;
        SerializedProperty keywordDatabaseProp;
        SerializedProperty sceneKeywordDatabaseProp;
        SerializedProperty previewTimeScaleProp;

        // Default animations
        SerializedProperty defaultAnimationsProp, defaultShowAnimationsProp, defaultHideAnimationsProp;
        ReorderableList defaultAnimationsList, defaultShowAnimationsList, defaultHideAnimationsList;
        Dictionary<int, string> listWarningDict, showListWarningDict, hideListWarningDict;
        Vector2 defaultListOffset = new Vector2(15, 2.5f);

        bool initStyles = false;

        private void OnEnable()
        {
            databaseProp = serializedObject.FindProperty("database");
            updateFromProp = serializedObject.FindProperty("updateFrom");
            animateOnStartProp = serializedObject.FindProperty("animateOnStart");
            animationsOverrideProp = serializedObject.FindProperty("animationsOverride");
            contextProp = serializedObject.FindProperty("context");
            contextScalingProp = contextProp.FindPropertyRelative("scaleAnimations");
            contextUniformScalingProp = contextProp.FindPropertyRelative("scaleUniformly");
            contextScaledTimeProp = contextProp.FindPropertyRelative("useScaledTime");
            keywordDatabaseProp = serializedObject.FindProperty("keywordDatabase");
            sceneKeywordDatabaseProp = serializedObject.FindProperty("sceneKeywordDatabase");
            previewProp = serializedObject.FindProperty("preview");
            excludedProp = serializedObject.FindProperty("excludedCharacters");
            excludedShowProp = serializedObject.FindProperty("excludedCharactersShow");
            excludedHideProp = serializedObject.FindProperty("excludedCharactersHide");
            excludePunctuationProp = serializedObject.FindProperty("excludePunctuation");
            excludePunctuationShowProp = serializedObject.FindProperty("excludePunctuationShow");
            excludePunctuationHideProp = serializedObject.FindProperty("excludePunctuationHide");
            useDefaultDatabaseProp = serializedObject.FindProperty("useDefaultDatabase");
            useDefaultKeywordDatabaseProp = serializedObject.FindProperty("useDefaultKeywordDatabase");
            initDatabaseProp = serializedObject.FindProperty("initDatabase");
            previewUpdatesProp = serializedObject.FindProperty("previewUpdatesPerSecond");
            previewTimeScaleProp = serializedObject.FindProperty("previewTimeScale");

            sceneAnimationsProp = serializedObject.FindProperty("sceneAnimations");
            sceneShowAnimationsProp = serializedObject.FindProperty("sceneShowAnimations");
            sceneHideAnimationsProp = serializedObject.FindProperty("sceneHideAnimations");

            // Default animations
            defaultAnimationsProp = serializedObject.FindProperty("defaultAnimationsStrings");
            defaultShowAnimationsProp = serializedObject.FindProperty("defaultShowAnimationsStrings");
            defaultHideAnimationsProp = serializedObject.FindProperty("defaultHideAnimationsStrings");

            defaultAnimationsList =
                new ReorderableList(serializedObject, defaultAnimationsProp, true, false, true, true)
                {
                    drawElementCallback = DrawDefaultBasicAnimationsList
                };

            defaultShowAnimationsList =
                new ReorderableList(serializedObject, defaultShowAnimationsProp, true, false, true, true)
                {
                    drawElementCallback = DrawDefaultShowAnimationsList
                };

            defaultHideAnimationsList =
                new ReorderableList(serializedObject, defaultHideAnimationsProp, true, false, true, true)
                {
                    drawElementCallback = DrawDefaultHideAnimationsList
                };

            listWarningDict = new Dictionary<int, string>();
            showListWarningDict = new Dictionary<int, string>();
            hideListWarningDict = new Dictionary<int, string>();

            animator = target as TMPAnimator;

            animator.OnResetComponent -= SetResetDatabase;
            animator.OnResetComponent += SetResetDatabase;

            Undo.undoRedoPerformed -= OnUndoRedo;
            Undo.undoRedoPerformed += OnUndoRedo;

            InitDatabase();
        }

        void DrawDefaultBasicAnimationsList(Rect rect, int index, bool isActive, bool isFocused)
        {
            DrawDefaultAnimationsList(rect, index, isActive, isFocused, TMPAnimationType.Basic, defaultAnimationsProp,
                listWarningDict);
        }

        void DrawDefaultShowAnimationsList(Rect rect, int index, bool isActive, bool isFocused)
        {
            DrawDefaultAnimationsList(rect, index, isActive, isFocused, TMPAnimationType.Show,
                defaultShowAnimationsProp, showListWarningDict);
        }

        void DrawDefaultHideAnimationsList(Rect rect, int index, bool isActive, bool isFocused)
        {
            DrawDefaultAnimationsList(rect, index, isActive, isFocused, TMPAnimationType.Hide,
                defaultHideAnimationsProp, hideListWarningDict);
        }

        void DrawDefaultAnimationsList(Rect rect, int index, bool isActive, bool isFocused, TMPAnimationType type,
            SerializedProperty defaultAnimationProperty, IDictionary<int, string> warningDict)
        {
            //rect.position += defaultListOffset;
            rect.size -= defaultListOffset;
            rect.y += 2.5f;
            //rect.height -= 2.5f;

            Rect textRect = rect;
            textRect.x += 25;
            textRect.width -= 25;
            EditorGUI.PropertyField(textRect, defaultAnimationProperty.GetArrayElementAtIndex(index), GUIContent.none);

            if (warningDict.TryGetValue(index, out string warning) && warning != "")
            {
                GUIContent warnGUI = new GUIContent(Styles.alertDialogDefaultShow);
                warnGUI.tooltip = warning;

                Rect warnRect = rect;
                warnRect.size = new Vector2(20, 20);

                EditorGUI.LabelField(warnRect, warnGUI);
            }
        }

        private void OnDisable()
        {
            animator.OnResetComponent -= SetResetDatabase;
            Undo.undoRedoPerformed -= OnUndoRedo;
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
                animator.OnChangedDatabase();
                return;
            }
        }

        private void SetDatabase()
        {
            // TMPEffectsPreferences preferences = TMPEffectsPreferences.Get();
            if (!useDefaultDatabaseProp.boolValue)
            {
                serializedObject.ApplyModifiedProperties();
                return;
            }

            if (TMPEffectsSettings.DefaultAnimationDatabase == null)
            {
                useDefaultDatabaseProp.boolValue = false;
                serializedObject.ApplyModifiedProperties();
                return;
            }

            if (databaseProp.objectReferenceValue != TMPEffectsSettings.DefaultAnimationDatabase)
            {
                databaseProp.objectReferenceValue = TMPEffectsSettings.DefaultAnimationDatabase;
                serializedObject.ApplyModifiedProperties();
                animator.OnChangedDatabase();
                return;
            }
        }

        GUIStyle horizontalLine;

        void InitStyles()
        {
            if (initStyles) return;
            initStyles = true;

            horizontalLine = new GUIStyle();
            horizontalLine.normal.background = EditorGUIUtility.whiteTexture;
            horizontalLine.margin = new RectOffset(0, 0, 4, 4);
            horizontalLine.fixedHeight = 1;
        }

        void DrawAnimationsFoldout()
        {
            if (reset) ResetDatabase();

            databaseProp.isExpanded =
                EditorGUILayout.Foldout(databaseProp.isExpanded, Styles.databaseFoldoutLabel, true);
            if (databaseProp.isExpanded)
            {
                EditorGUI.indentLevel++;
                if (TMPEffectsDrawerUtility.DrawDefaultableDatabase(databaseProp, useDefaultDatabaseProp,
                        Styles.database, GetDefaultAnimationDatabase, "animation"))
                {
                    animator.OnChangedDatabase();
                }

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(sceneAnimationsProp);
                EditorGUILayout.PropertyField(sceneShowAnimationsProp);
                EditorGUILayout.PropertyField(sceneHideAnimationsProp);
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
                    animator.OnChangedDatabase();
                }

                EditorGUI.indentLevel--;
            }
        }

        private Object GetDefaultAnimationDatabase()
        {
            var database = TMPEffectsSettings.DefaultAnimationDatabase;
            if (database == null) return null;

            if (database == null)
            {
                Debug.LogWarning("No default animation database set in ProjectSettings/TMPEffects");
            }

            return database;
        }

        private Object GetDefaultKeywordDatabase()
        {
            var database = TMPEffectsSettings.DefaultKeywordDatabase;
            if (database == null) return null;

            if (database == null)
            {
                Debug.LogWarning("No default keyword database set in ProjectSettings/TMPEffects");
            }

            return database;
        }

        string defaultShowTooltip;

        void DrawDefaults()
        {
            DrawDefault(
                TMPAnimationType.Basic,
                defaultAnimationsProp,
                defaultAnimationsList,
                Styles.defaultAnimations,
                listWarningDict);

            DrawDefault(
                TMPAnimationType.Show,
                defaultShowAnimationsProp,
                defaultShowAnimationsList,
                Styles.defaultShowAnimations,
                showListWarningDict);

            DrawDefault(
                TMPAnimationType.Hide,
                defaultHideAnimationsProp,
                defaultHideAnimationsList,
                Styles.defaultHideAnimations,
                hideListWarningDict);
        }


        void DrawDefault(TMPAnimationType type, SerializedProperty defaultAnimationsProp,
            ReorderableList defaultAnimationsList, GUIContent label, IDictionary<int, string> warningDict)
        {
            EditorGUI.BeginChangeCheck();

            if ((defaultAnimationsProp.isExpanded =
                    EditorGUILayout.Foldout(defaultAnimationsProp.isExpanded, label, true)))
            {
                Rect rect = GUILayoutUtility.GetRect(0f, defaultAnimationsList.GetHeight());
                rect = new Rect(rect.x + defaultListOffset.x, rect.y + defaultListOffset.y,
                    rect.width - defaultListOffset.x, rect.height - defaultListOffset.y);
                defaultAnimationsList.DoList(rect);
            }

            if (EditorGUI.EndChangeCheck())
            {
                warningDict.Clear();
                for (int i = 0; i < defaultAnimationsProp.arraySize; i++)
                {
                    warningDict.Add(i,
                        animator.CheckDefaultString(type, defaultAnimationsProp.GetArrayElementAtIndex(i).stringValue));
                }

                serializedObject.ApplyModifiedProperties();
                animator.UpdateDefaultAnimations(type);
            }
        }

        void DrawExclusions()
        {
            EditorGUILayout.BeginHorizontal();
            var prev = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 100;
            EditorGUILayout.LabelField(Styles.excludePunctuation);
            EditorGUIUtility.labelWidth = 50;

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(excludePunctuationProp, Styles.basic);
            if (EditorGUI.EndChangeCheck())
            {
                if (serializedObject.hasModifiedProperties) serializedObject.ApplyModifiedProperties();
                animator.OnChangedBasicExclusion();
            }

            EditorGUIUtility.labelWidth = 50;
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(excludePunctuationShowProp, Styles.show);
            if (EditorGUI.EndChangeCheck())
            {
                if (serializedObject.hasModifiedProperties) serializedObject.ApplyModifiedProperties();
                animator.OnChangedShowExclusion();
            }

            EditorGUIUtility.labelWidth = 45;
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(excludePunctuationHideProp, Styles.hide);
            if (EditorGUI.EndChangeCheck())
            {
                if (serializedObject.hasModifiedProperties) serializedObject.ApplyModifiedProperties();
                animator.OnChangedHideExclusion();
            }


            EditorGUIUtility.labelWidth = prev;
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();


            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(excludedProp, Styles.excludedCharacters);
            if (EditorGUI.EndChangeCheck())
            {
                if (serializedObject.hasModifiedProperties) serializedObject.ApplyModifiedProperties();
                animator.OnChangedBasicExclusion();
            }

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(excludedShowProp, Styles.showExcludedCharacters);
            if (EditorGUI.EndChangeCheck())
            {
                if (serializedObject.hasModifiedProperties) serializedObject.ApplyModifiedProperties();
                animator.OnChangedShowExclusion();
            }

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(excludedHideProp, Styles.hideExcludedCharacters);
            if (EditorGUI.EndChangeCheck())
            {
                if (serializedObject.hasModifiedProperties) serializedObject.ApplyModifiedProperties();
                animator.OnChangedHideExclusion();
            }
        }

        void DrawPreview()
        {
            EditorGUI.BeginDisabledGroup(!PreviewEnabled());

            var rect = EditorGUILayout.GetControlRect(false,
                (EditorGUIUtility.singleLineHeight * (Application.isPlaying ? 2 : (previewProp.isExpanded ? 4.5f : 2))) +
                10);
            var dividerRect = new Rect(10, rect.y + rect.height - (EditorGUIUtility.singleLineHeight / 2f), rect.width,
                1);

            GUIStyle animationButtonStyle2 = new GUIStyle(GUI.skin.button);
            animationButtonStyle2.richText = true;
            char animationC2 = animator.IsAnimating ? '\u2713' : '\u2717';
            GUIContent animationButtonContent2 = new GUIContent("Toggle animation " +
                                                                (animator.IsAnimating
                                                                    ? "<color=#90ee90>"
                                                                    : "<color=#f1807e>") + animationC2.ToString() +
                                                                "</color>");

            var size = Styles.playLabelStyle.CalcSize(Styles.animatorPreview);
            var button1Size = animationButtonStyle2.CalcSize(animationButtonContent2);
            var button2Size = animationButtonStyle2.CalcSize(Styles.resetTime);

            var headerRect = new Rect(15, 10, size.x, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(headerRect, "Animator Preview", Styles.playLabelStyle);

            var settingsFoldoutRect = new Rect(15, headerRect.y, size.x + 10, headerRect.height);

            if (Application.isPlaying)
            {
                EditorGUI.Foldout(settingsFoldoutRect, false, GUIContent.none);
            }
            else
                previewProp.isExpanded =
                    EditorGUI.Foldout(settingsFoldoutRect, previewProp.isExpanded, GUIContent.none, true);

            var usedWidth = 15 + size.x + 10;
            var remainingWidth = rect.width - headerRect.width - 10;
            var button1Rect = new Rect(usedWidth, 10, Mathf.Max(button1Size.x, remainingWidth * 0.6f),
                EditorGUIUtility.singleLineHeight);
            var button2Rect = new Rect(usedWidth + button1Rect.width, 10,
                Mathf.Max(button2Size.x, remainingWidth * 0.4f), EditorGUIUtility.singleLineHeight);

            bool prevPreview2 = previewProp.boolValue;

            if (GUI.Button(button1Rect, animationButtonContent2, animationButtonStyle2))
            {
                previewProp.boolValue = !prevPreview2;
            }

            if (GUI.Button(button2Rect, "Reset Time", animationButtonStyle2))
            {
                animator.ResetTime();
            }

            EditorGUI.EndDisabledGroup();
            if (Application.isPlaying)
            {
                EditorGUI.DrawRect(dividerRect, new Color(48f / 255, 44f / 255, 44f / 255));
                return;
            }

            if (previewProp.isExpanded)
            {
                EditorGUI.indentLevel++;

                var updateRect = headerRect;
                updateRect.width = rect.width;
                updateRect.y += EditorGUIUtility.singleLineHeight * 1.5f;

                EditorGUI.BeginChangeCheck();
                previewUpdatesProp.intValue =
                    EditorGUI.IntSlider(updateRect, Styles.updatesPerSecond, previewUpdatesProp.intValue,
                        1, 100);


                updateRect.y += EditorGUIUtility.singleLineHeight;

                var editorgui = typeof(EditorGUI);
                MethodInfo powerSliderMethod =
                    editorgui.GetMethod("PowerSlider", BindingFlags.NonPublic | BindingFlags.Static, null, new Type[]
                    {
                        typeof(Rect),
                        typeof(GUIContent),
                        typeof(float),
                        typeof(float),
                        typeof(float),
                        typeof(GUIStyle),
                        typeof(float),
                    }, null);

                if (powerSliderMethod != null)
                {
                    object[] parameters = new object[]
                    {
                        updateRect, Styles.previewTimeScale,
                        previewTimeScaleProp.floatValue, 0.01f, 5f,
                        EditorStyles.numberField,
                        1.3475f // Totally arbitrary value that makes a nice distribution
                    };

                    previewTimeScaleProp.floatValue =
                        (float)powerSliderMethod.Invoke(null, parameters);
                }
                else
                {
                    if (!EditorPrefs.GetBool(TMPEffectsEditorPrefsKeys.RanIntoMissingReflectedPowerSlider, false))
                    {
                        TMPEffectsBugReport.BugReportPrompt(
                            "Could not find internal PowerSlider method.\nThis error wont show again, and a fallback method will be used from now on:\n");
                        EditorPrefs.SetBool(TMPEffectsEditorPrefsKeys.RanIntoMissingReflectedPowerSlider, true);
                    }

                    previewTimeScaleProp.floatValue =
                        EditorGUI.Slider(updateRect, Styles.previewTimeScale,
                            previewTimeScaleProp.floatValue, 0.01f, 5f);
                }

                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();
                    animator.UpdatePreviewUpdates();
                }

                EditorGUI.indentLevel--;
            }

            EditorGUI.DrawRect(dividerRect, new Color(48f / 255, 44f / 255, 44f / 255));

            if (previewProp.boolValue)
            {
                if (!prevPreview2)
                {
                    animator.StartPreview();
                }
            }
            else if (prevPreview2)
            {
                animator.StopPreview();
            }

            if (serializedObject.hasModifiedProperties) serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        // utility method
        void HorizontalLine(Color color)
        {
            var c = GUI.color;
            GUI.color = color;
            GUILayout.Box(GUIContent.none, horizontalLine);
            GUI.color = c;
        }

        void RepaintInspector()
        {
            DrawPreview();

            EditorGUI.BeginDisabledGroup(Application.isPlaying);
            EditorGUILayout.PropertyField(updateFromProp, Styles.updateFrom);
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.PropertyField(animateOnStartProp, Styles.animateOnStart);

            EditorGUILayout.Space(10);
            DrawAnimationsFoldout();

            EditorGUILayout.Space(10);
            animationsOverrideProp.isExpanded =
                EditorGUILayout.Foldout(animationsOverrideProp.isExpanded, Styles.animatorSettings, true);
            if (animationsOverrideProp.isExpanded)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(animationsOverrideProp, Styles.animationsOverride);
                EditorGUILayout.Space(10);
                DrawDefaults();

                EditorGUILayout.Space(10);
                DrawExclusions();


                EditorGUILayout.Space(10);


                if (TMPEffectsDrawerUtility.DrawDefaultableDatabase(keywordDatabaseProp, useDefaultKeywordDatabaseProp,
                        Styles.keywordDatabase, GetDefaultKeywordDatabase, "keyword"))
                {
                    animator.OnChangedDatabase();
                }

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(sceneKeywordDatabaseProp, Styles.sceneKeywordDatabase);
                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();
                    animator.OnChangedDatabase();
                }

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space(10);

            contextProp.isExpanded =
                EditorGUILayout.Foldout(contextProp.isExpanded, Styles.animationSettings, true);
            if (contextProp.isExpanded)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(contextScalingProp);
                EditorGUILayout.PropertyField(contextUniformScalingProp);
                EditorGUILayout.PropertyField(contextScaledTimeProp);
                EditorGUI.indentLevel--;
            }
        }

        public override void OnInspectorGUI()
        {
            InitStyles();

            RepaintInspector();

            if (serializedObject.hasModifiedProperties) serializedObject.ApplyModifiedProperties();
        }

        bool PreviewEnabled()
        {
            return animator.isActiveAndEnabled && !Application.isPlaying;
        }

        void OnUndoRedo()
        {
            if (previewProp.boolValue)
            {
                animator.StopPreview();

                EditorApplication.delayCall += () => { animator.StartPreview(); };
            }
        }
    }


    internal static class TMPEffectsDrawerUtility
    {
        public static bool DrawDefaultableDatabase(SerializedProperty dbProp, SerializedProperty useDefaultProp,
            GUIContent label,
            Func<UnityEngine.Object> getDefault, string databaseName)
        {
            SerializedObject serializedObject = dbProp.serializedObject;
            bool changed = false;
            GUILayout.BeginHorizontal();

            var rect = EditorGUILayout.GetControlRect();

            GUIStyle style = EditorStyles.toggle;
            Vector2 size = style.CalcSize(GUIContent.none);

            size.x -= 10;
            var toggleRect = new Rect(rect.x, rect.y, size.x, rect.height);
            var dbRect = new Rect(rect.x + EditorGUIUtility.labelWidth + size.x, rect.y,
                rect.width - EditorGUIUtility.labelWidth - size.x, rect.height);

            bool prevUseDefaultDatabase = useDefaultProp.boolValue;
            useDefaultProp.boolValue =
                EditorGUI.Toggle(toggleRect, label, useDefaultProp.boolValue);

            if (prevUseDefaultDatabase != useDefaultProp.boolValue)
            {
                if (useDefaultProp.boolValue)
                {
                    var defaultDatabase = getDefault();
                    if (defaultDatabase == null)
                    {
                        Debug.LogWarning($"No default {databaseName} database set in Project Settings/TMPEffects");
                        useDefaultProp.boolValue = false;
                        serializedObject.ApplyModifiedProperties();
                    }
                    else
                    {
                        dbProp.objectReferenceValue = defaultDatabase;
                        serializedObject.ApplyModifiedProperties();
                        changed = true;
                    }
                }
            }
            else
            {
                var defaultDatabase = getDefault();
                if (defaultDatabase != null && defaultDatabase != dbProp.objectReferenceValue)
                {
                    useDefaultProp.boolValue = false;
                    serializedObject.ApplyModifiedProperties();
                }
            }

            EditorGUI.BeginChangeCheck();
            EditorGUI.BeginDisabledGroup(useDefaultProp.boolValue);
            EditorGUI.PropertyField(dbRect, dbProp, GUIContent.none);
            EditorGUI.EndDisabledGroup();
            if (EditorGUI.EndChangeCheck())
            {
                if (serializedObject.hasModifiedProperties) serializedObject.ApplyModifiedProperties();
                changed = true;
            }

            GUILayout.EndHorizontal();
            return changed;
        }
    }
}