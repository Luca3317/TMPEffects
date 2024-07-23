using UnityEngine;
using UnityEditor;
using TMPEffects.Components;
using UnityEditorInternal;
using System.Collections.Generic;
using TMPEffects.Components.Animator;

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
        SerializedProperty initDatabaseProp;

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
            previewProp = serializedObject.FindProperty("preview");
            excludedProp = serializedObject.FindProperty("excludedCharacters");
            excludedShowProp = serializedObject.FindProperty("excludedCharactersShow");
            excludedHideProp = serializedObject.FindProperty("excludedCharactersHide");
            excludePunctuationProp = serializedObject.FindProperty("excludePunctuation");
            excludePunctuationShowProp = serializedObject.FindProperty("excludePunctuationShow");
            excludePunctuationHideProp = serializedObject.FindProperty("excludePunctuationHide");
            useDefaultDatabaseProp = serializedObject.FindProperty("useDefaultDatabase");
            initDatabaseProp = serializedObject.FindProperty("initDatabase");

            sceneAnimationsProp = serializedObject.FindProperty("sceneAnimations");
            sceneShowAnimationsProp = serializedObject.FindProperty("sceneShowAnimations");
            sceneHideAnimationsProp = serializedObject.FindProperty("sceneHideAnimations");

            // Default animations
            defaultAnimationsProp = serializedObject.FindProperty("defaultAnimationsStrings");
            defaultShowAnimationsProp = serializedObject.FindProperty("defaultShowAnimationsStrings");
            defaultHideAnimationsProp = serializedObject.FindProperty("defaultHideAnimationsStrings");

            defaultAnimationsList = new ReorderableList(serializedObject, defaultAnimationsProp, true, false, true, true)
            {
                drawElementCallback = DrawDefaultBasicAnimationsList
            };

            defaultShowAnimationsList = new ReorderableList(serializedObject, defaultShowAnimationsProp, true, false, true, true)
            {
                drawElementCallback = DrawDefaultShowAnimationsList
            };

            defaultHideAnimationsList = new ReorderableList(serializedObject, defaultHideAnimationsProp, true, false, true, true)
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
            DrawDefaultAnimationsList(rect, index, isActive, isFocused, TMPAnimationType.Basic, defaultAnimationsProp, listWarningDict);
        }

        void DrawDefaultShowAnimationsList(Rect rect, int index, bool isActive, bool isFocused)
        {
            DrawDefaultAnimationsList(rect, index, isActive, isFocused, TMPAnimationType.Show, defaultShowAnimationsProp, showListWarningDict);
        }

        void DrawDefaultHideAnimationsList(Rect rect, int index, bool isActive, bool isFocused)
        {
            DrawDefaultAnimationsList(rect, index, isActive, isFocused, TMPAnimationType.Hide, defaultHideAnimationsProp, hideListWarningDict);
        }

        void DrawDefaultAnimationsList(Rect rect, int index, bool isActive, bool isFocused, TMPAnimationType type, SerializedProperty defaultAnimationProperty, IDictionary<int, string> warningDict)
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
            TMPEffectsSettings settings = TMPEffectsSettings.Get();
            if (settings == null || !useDefaultDatabaseProp.boolValue)
            {
                serializedObject.ApplyModifiedProperties();
                return;
            }

            if (settings.DefaultAnimationDatabase == null)
            {
                Debug.LogWarning("No default animation database set in Preferences/TMPEffects");
                useDefaultDatabaseProp.boolValue = false;
                serializedObject.ApplyModifiedProperties();
                return;
            }

            if (databaseProp.objectReferenceValue != settings.DefaultAnimationDatabase)
            {
                databaseProp.objectReferenceValue = settings.DefaultAnimationDatabase;
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

            databaseProp.isExpanded = EditorGUILayout.Foldout(databaseProp.isExpanded, new GUIContent("Animations"), true);
            if (databaseProp.isExpanded)
            {
                EditorGUI.indentLevel++;
                DrawDatabase();
                EditorGUI.indentLevel--;
            }
        }

        void DrawDatabase()
        {
            GUILayout.BeginHorizontal();

            bool prevUseDefaultDatabase = useDefaultDatabaseProp.boolValue;
            useDefaultDatabaseProp.boolValue = EditorGUILayout.Toggle(useDefaultDatabaseLabel, useDefaultDatabaseProp.boolValue);

            if (prevUseDefaultDatabase != useDefaultDatabaseProp.boolValue)
            {
                if (useDefaultDatabaseProp.boolValue)
                {
                    TMPEffectsSettings settings = TMPEffectsSettings.Get();
                    if (settings == null)
                    {
                        useDefaultDatabaseProp.boolValue = false;
                        serializedObject.ApplyModifiedProperties();
                    }
                    else if (settings.DefaultAnimationDatabase == null)
                    {
                        Debug.LogWarning("No default animation database set in Preferences/TMPEffects");
                        useDefaultDatabaseProp.boolValue = false;
                        serializedObject.ApplyModifiedProperties();
                    }
                    else
                    {
                        databaseProp.objectReferenceValue = settings.DefaultAnimationDatabase;
                        serializedObject.ApplyModifiedProperties();
                        animator.OnChangedDatabase();
                    }
                }
            }
            else
            {
                TMPEffectsSettings settings = TMPEffectsSettings.Get();
                if (settings != null && settings.DefaultAnimationDatabase != databaseProp.objectReferenceValue)
                {
                    useDefaultDatabaseProp.boolValue = false;
                    serializedObject.ApplyModifiedProperties();
                }
            }

            EditorGUI.BeginChangeCheck();
            EditorGUI.BeginDisabledGroup(useDefaultDatabaseProp.boolValue);
            EditorGUILayout.PropertyField(databaseProp, GUIContent.none);
            EditorGUI.EndDisabledGroup();
            if (EditorGUI.EndChangeCheck())
            {
                if (serializedObject.hasModifiedProperties) serializedObject.ApplyModifiedProperties();
                animator.OnChangedDatabase();
            }
            GUILayout.EndHorizontal();

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
                new GUIContent("Default Animations", "Default animations that will be applied to the entire text, without needing any tags."),
                listWarningDict);

            DrawDefault(
                TMPAnimationType.Show,
                defaultShowAnimationsProp,
                defaultShowAnimationsList,
                new GUIContent("Default Show Animations", "Default show animations that will be applied to the entire text, without needing any tags."),
                showListWarningDict);

            DrawDefault(
                TMPAnimationType.Hide,
                defaultHideAnimationsProp,
                defaultHideAnimationsList,
                new GUIContent("Default Hide Animations", "Default hide animations that will be applied to the entire text, without needing any tags."),
                hideListWarningDict);
        }


        void DrawDefault(TMPAnimationType type, SerializedProperty defaultAnimationsProp, ReorderableList defaultAnimationsList, GUIContent label, IDictionary<int, string> warningDict)
        {
            EditorGUI.BeginChangeCheck();

            if ((defaultAnimationsProp.isExpanded = EditorGUILayout.Foldout(defaultAnimationsProp.isExpanded, label, true)))
            {
                Rect rect = GUILayoutUtility.GetRect(0f, defaultAnimationsList.GetHeight());
                rect = new Rect(rect.x + defaultListOffset.x, rect.y + defaultListOffset.y, rect.width - defaultListOffset.x, rect.height - defaultListOffset.y);
                defaultAnimationsList.DoList(rect);
            }

            if (EditorGUI.EndChangeCheck())
            {
                warningDict.Clear();
                for (int i = 0; i < defaultAnimationsProp.arraySize; i++)
                {
                    warningDict.Add(i, animator.CheckDefaultString(type, defaultAnimationsProp.GetArrayElementAtIndex(i).stringValue));
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
            EditorGUIUtility.labelWidth = 40;

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(excludePunctuationProp, new GUIContent("Basic"));
            if (EditorGUI.EndChangeCheck())
            {
                if (serializedObject.hasModifiedProperties) serializedObject.ApplyModifiedProperties();
                animator.OnChangedBasicExclusion();
            }

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(excludePunctuationShowProp, new GUIContent("Show"));
            if (EditorGUI.EndChangeCheck())
            {
                if (serializedObject.hasModifiedProperties) serializedObject.ApplyModifiedProperties();
                animator.OnChangedShowExclusion();
            }

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
            bool prevPreview = previewProp.boolValue;

            EditorGUILayout.LabelField(new GUIContent("Animator preview"), previewLabelStyle);

            EditorGUI.BeginDisabledGroup(!PreviewEnabled());
            EditorGUILayout.BeginHorizontal();

            char c = previewProp.boolValue ? '\u2713' : '\u2717';

            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.richText = true;
            GUIContent buttonContent = new GUIContent("Toggle preview " + (previewProp.boolValue ? "<color=#90ee90>" : "<color=#f1807e>") + c.ToString() + "</color>");


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

            EditorGUI.BeginDisabledGroup(Application.isPlaying);
            EditorGUILayout.PropertyField(updateFromProp);
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.PropertyField(animateOnStartProp);

            EditorGUILayout.Space(10);
            DrawAnimationsFoldout();

            EditorGUILayout.Space(10);
            animationsOverrideProp.isExpanded = EditorGUILayout.Foldout(animationsOverrideProp.isExpanded, new GUIContent("Animator settings"), true);
            if (animationsOverrideProp.isExpanded)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(animationsOverrideProp);
                EditorGUILayout.Space(10);
                DrawDefaults();

                EditorGUILayout.Space(10);
                DrawExclusions();
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.Space(10);

            if (contextProp.isExpanded = EditorGUILayout.Foldout(contextProp.isExpanded, new GUIContent("Animation Settings"), true))
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

                EditorApplication.delayCall += () =>
                {
                    animator.StartPreview();
                };
            }
        }
    }
}