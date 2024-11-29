using System;
using UnityEngine;
using UnityEditor;
using TMPEffects.Components;
using UnityEditorInternal;
using System.Collections.Generic;
using TMPEffects.Components.Animator;
using TMPEffects.Databases;
using Object = UnityEngine.Object;

namespace TMPEffects.Editor
{
    [CustomEditor(typeof(TMPAnimator))]
    public class TMPAnimatorEditor : UnityEditor.Editor
    {
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
        SerializedProperty passedTimeProp;
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

        // Default animations
        SerializedProperty defaultAnimationsProp, defaultShowAnimationsProp, defaultHideAnimationsProp;
        ReorderableList defaultAnimationsList, defaultShowAnimationsList, defaultHideAnimationsList;
        Dictionary<int, string> listWarningDict, showListWarningDict, hideListWarningDict;
        Vector2 defaultListOffset = new Vector2(15, 2.5f);

        // Styles
        GUIStyle previewLabelStyle;


        GUIContent useDefaultDatabaseLabel;

        bool initGuiContent = false;
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
            passedTimeProp = contextProp.FindPropertyRelative("passedTime");
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
                GUIContent warnGUI = new GUIContent(alertDialogDefaultShow);
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

        void InitGUIContent()
        {
            if (initGuiContent) return;
            initGuiContent = true;

            alertDialogDefaultShow = new GUIContent();
            alertDialogDefaultHide = new GUIContent();
            alertDialogDefaultShow.image = (Texture)EditorGUIUtility.Load("alertDialog");
            alertDialogDefaultHide.image = (Texture)EditorGUIUtility.Load("alertDialog");
            alertDialogDefaultShow.text = "";
            alertDialogDefaultHide.text = "";
            useDefaultDatabaseLabel = new GUIContent("Use default database");
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

            previewLabelStyle = new GUIStyle("LargeBoldLabel");
        }

        void DrawAnimationsFoldout()
        {
            if (reset) ResetDatabase();

            databaseProp.isExpanded =
                EditorGUILayout.Foldout(databaseProp.isExpanded, new GUIContent("Animations"), true);
            if (databaseProp.isExpanded)
            {
                EditorGUI.indentLevel++;
                if (TMPEffectsDrawerUtility.DrawDefaultableDatabase(databaseProp, useDefaultDatabaseProp,
                        useDefaultDatabaseLabel, GetDefaultAnimationDatabase, "animation"))
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

        GUIContent alertDialogDefaultShow;
        GUIContent alertDialogDefaultHide;

        string defaultShowTooltip;

        void DrawDefaults()
        {
            DrawDefault(
                TMPAnimationType.Basic,
                defaultAnimationsProp,
                defaultAnimationsList,
                new GUIContent("Default Animations",
                    "Default animations that will be applied to the entire text, without needing any tags."),
                listWarningDict);

            DrawDefault(
                TMPAnimationType.Show,
                defaultShowAnimationsProp,
                defaultShowAnimationsList,
                new GUIContent("Default Show Animations",
                    "Default show animations that will be applied to the entire text, without needing any tags."),
                showListWarningDict);

            DrawDefault(
                TMPAnimationType.Hide,
                defaultHideAnimationsProp,
                defaultHideAnimationsList,
                new GUIContent("Default Hide Animations",
                    "Default hide animations that will be applied to the entire text, without needing any tags."),
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
            EditorGUILayout.LabelField("Exclude punctuation?");
            EditorGUIUtility.labelWidth = 50;

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(excludePunctuationProp, new GUIContent("Basic"));
            if (EditorGUI.EndChangeCheck())
            {
                if (serializedObject.hasModifiedProperties) serializedObject.ApplyModifiedProperties();
                animator.OnChangedBasicExclusion();
            }

            EditorGUIUtility.labelWidth = 50;
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(excludePunctuationShowProp, new GUIContent("Show"));
            if (EditorGUI.EndChangeCheck())
            {
                if (serializedObject.hasModifiedProperties) serializedObject.ApplyModifiedProperties();
                animator.OnChangedShowExclusion();
            }

            EditorGUIUtility.labelWidth = 45;
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(excludePunctuationHideProp, new GUIContent("Hide"));
            if (EditorGUI.EndChangeCheck())
            {
                if (serializedObject.hasModifiedProperties) serializedObject.ApplyModifiedProperties();
                animator.OnChangedHideExclusion();
            }


            EditorGUIUtility.labelWidth = prev;
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();


            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(excludedProp);
            if (EditorGUI.EndChangeCheck())
            {
                if (serializedObject.hasModifiedProperties) serializedObject.ApplyModifiedProperties();
                animator.OnChangedBasicExclusion();
            }

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(excludedShowProp);
            if (EditorGUI.EndChangeCheck())
            {
                if (serializedObject.hasModifiedProperties) serializedObject.ApplyModifiedProperties();
                animator.OnChangedShowExclusion();
            }

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(excludedHideProp);
            if (EditorGUI.EndChangeCheck())
            {
                if (serializedObject.hasModifiedProperties) serializedObject.ApplyModifiedProperties();
                animator.OnChangedHideExclusion();
            }
        }

        void DrawPreview()
        {
            if (Application.isPlaying)
            {
                EditorGUILayout.Space();

                GUIStyle animationButtonStyle = new GUIStyle(GUI.skin.button);
                animationButtonStyle.richText = true;
                char animationC = animator.IsAnimating ? '\u2713' : '\u2717';
                GUIContent animationButtonContent = new GUIContent("Toggle animation " +
                                                                   (animator.IsAnimating
                                                                       ? "<color=#90ee90>"
                                                                       : "<color=#f1807e>") + animationC.ToString() +
                                                                   "</color>");

                EditorGUI.BeginDisabledGroup( /*!animator.IsAnimating || */animator.UpdateFrom == UpdateFrom.Script);

                if (GUILayout.Button(animationButtonContent, animationButtonStyle))
                {
                    if (animator.IsAnimating) animator.StopAnimating();
                    else animator.StartAnimating();
                }

                EditorGUI.EndDisabledGroup();

                EditorGUILayout.Space();
                HorizontalLine(Color.gray);
                EditorGUILayout.Space();
                return;
            }

            bool prevPreview = previewProp.boolValue;

            // Draw Label and update slider
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Animator preview"), previewLabelStyle);
            EditorGUI.BeginChangeCheck();
            previewUpdatesProp.intValue =
                EditorGUILayout.IntSlider(GUIContent.none, previewUpdatesProp.intValue, 1, 100);
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                animator.UpdatePreviewUpdates();
            }

            EditorGUILayout.EndHorizontal();

            // Draw preview toggle and reset time button
            EditorGUI.BeginDisabledGroup(!PreviewEnabled());
            EditorGUILayout.BeginHorizontal();

            char c = previewProp.boolValue ? '\u2713' : '\u2717';

            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.richText = true;
            GUIContent buttonContent = new GUIContent("Toggle preview " +
                                                      (previewProp.boolValue ? "<color=#90ee90>" : "<color=#f1807e>") +
                                                      c.ToString() + "</color>");


            if (GUILayout.Button(buttonContent, buttonStyle))
            {
                previewProp.boolValue = !prevPreview;
            }

            if (GUILayout.Button(new GUIContent("Reset time")))
            {
                animator.ResetTime();
            }

            EditorGUILayout.EndHorizontal();

            EditorGUI.EndDisabledGroup();

            if (previewProp.boolValue)
            {
                if (!prevPreview)
                {
                    animator.StartPreview();
                }
            }
            else if (prevPreview)
            {
                animator.StopPreview();
            }

            if (serializedObject.hasModifiedProperties) serializedObject.ApplyModifiedPropertiesWithoutUndo();

            EditorGUILayout.Space();
            HorizontalLine(Color.gray);
            EditorGUILayout.Space();
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

            if (GUILayout.Button("Trigger bug report"))
            {
                try
                {
                    List<string> sss = null;
                    Debug.LogWarning(sss.Count);
                }
                catch (System.Exception ex)
                {
                    TMPEffectsBugReport.BugReportPrompt(ex);
                }
            }

            EditorGUI.BeginDisabledGroup(Application.isPlaying);
            EditorGUILayout.PropertyField(updateFromProp);
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.PropertyField(animateOnStartProp);

            EditorGUILayout.Space(10);
            DrawAnimationsFoldout();

            EditorGUILayout.Space(10);
            animationsOverrideProp.isExpanded = EditorGUILayout.Foldout(animationsOverrideProp.isExpanded,
                new GUIContent("Animator settings"), true);
            if (animationsOverrideProp.isExpanded)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(animationsOverrideProp);
                EditorGUILayout.Space(10);
                DrawDefaults();

                EditorGUILayout.Space(10);
                DrawExclusions();


                EditorGUILayout.Space(10);
                GUIContent cont = new GUIContent("Keyword Database");
                cont.tooltip = "A keyword database defining additional keywords. " +
                               "If the same keyword is present in the global keyword Database, this database will override it.";

                if (TMPEffectsDrawerUtility.DrawDefaultableDatabase(keywordDatabaseProp, useDefaultKeywordDatabaseProp,
                        cont, GetDefaultKeywordDatabase, "keyword"))
                {
                    animator.OnChangedDatabase();
                }

                EditorGUI.BeginChangeCheck();
                cont = new GUIContent("Scene Keyword Database");
                cont.tooltip = "A scene keyword database defining additional keywords. " +
                               "If the same keyword is present in the global keyword Database or keyword database on this animator, this database will override it.";
                EditorGUILayout.PropertyField(sceneKeywordDatabaseProp, cont);
                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();
                    animator.OnChangedDatabase();
                }

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space(10);

            contextProp.isExpanded =
                EditorGUILayout.Foldout(contextProp.isExpanded, new GUIContent("Animation Settings"), true);
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
            InitGUIContent();
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

            // EditorGUILayout.LabelField("Use default database?");
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