using UnityEngine;
using UnityEditor;
using TMPEffects.Components;
using TMPEffects.Databases.AnimationDatabase;

namespace TMPEffects.Editor
{
    [CustomEditor(typeof(TMPAnimator))]
    public class TMPAnimatorEditor : UnityEditor.Editor
    {
        TMPAnimator animator;

        bool useDefaultDatabase;
        TMPAnimationDatabase defaultDatabase;

        // Serialized properties
        SerializedProperty databaseProp;
        SerializedProperty updateFromProp;
        SerializedProperty animateOnStartProp;
        SerializedProperty animationsOverrideProp;
        SerializedProperty contextProp;
        SerializedProperty contextScalingProp;
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

        //SerializedProperty animateOnTextChangeProp;

        GUIContent useDefaultDatabaseLabel;

        bool guiContent = false;
        bool forceReprocess = false;

        private void OnEnable()
        {
            databaseProp = serializedObject.FindProperty("database");
            updateFromProp = serializedObject.FindProperty("updateFrom");
            animateOnStartProp = serializedObject.FindProperty("animateOnStart");
            animationsOverrideProp = serializedObject.FindProperty("animationsOverride");
            contextProp = serializedObject.FindProperty("context");
            contextScalingProp = contextProp.FindPropertyRelative("scaleAnimations");
            contextScaledTimeProp = contextProp.FindPropertyRelative("useScaledTime");
            passedTimeProp = contextProp.FindPropertyRelative("passedTime");
            previewProp = serializedObject.FindProperty("preview");
            excludedProp = serializedObject.FindProperty("excludedCharacters");
            excludedShowProp = serializedObject.FindProperty("excludedCharactersShow");
            excludedHideProp = serializedObject.FindProperty("excludedCharactersHide");
            excludePunctuationProp = serializedObject.FindProperty("excludePunctuation");
            excludePunctuationShowProp = serializedObject.FindProperty("excludePunctuationShow");
            excludePunctuationHideProp = serializedObject.FindProperty("excludePunctuationHide");

            sceneAnimationsProp = serializedObject.FindProperty("sceneAnimations");
            sceneShowAnimationsProp = serializedObject.FindProperty("sceneShowAnimations");
            sceneHideAnimationsProp = serializedObject.FindProperty("sceneHideAnimations");

            defaultShowStringProp = serializedObject.FindProperty("defaultShowString");
            defaultHideStringProp = serializedObject.FindProperty("defaultHideString");

            animator = target as TMPAnimator;

            //wasEnabled = writer.enabled;
            defaultDatabase = (TMPAnimationDatabase)Resources.Load("DefaultAnimationDatabase");
            useDefaultDatabase = defaultDatabase == databaseProp.objectReferenceValue || !serializedObject.FindProperty("initValidate").boolValue;

            if (!serializedObject.FindProperty("initValidate").boolValue)
            {
                databaseProp.objectReferenceValue = defaultDatabase;
                //serializedObject.FindProperty("initValidate").boolValue = true;
                serializedObject.ApplyModifiedProperties();
            }

            animator.ForceReprocess();

            Undo.undoRedoPerformed += OnUndoRedo;
        }

        private void OnDisable()
        {
            Undo.undoRedoPerformed -= OnUndoRedo;
            Undo.undoRedoPerformed -= OnUndoRedo;
        }

        void InitGUIContent()
        {
            if (guiContent) return;
            guiContent = true;

            alertDialogDefaultShow = new GUIContent();
            alertDialogDefaultHide = new GUIContent();
            alertDialogDefaultShow.image = (Texture)EditorGUIUtility.Load("alertDialog");
            alertDialogDefaultHide.image = (Texture)EditorGUIUtility.Load("alertDialog");
            alertDialogDefaultShow.text = "";
            alertDialogDefaultHide.text = "";
            useDefaultDatabaseLabel = new GUIContent("Use default database");
        }

        void DrawDatabase()
        {
            EditorGUI.BeginChangeCheck();
            GUILayout.BeginHorizontal();

            var value = databaseProp.objectReferenceValue;

            bool prevUseDefaultDatabase = useDefaultDatabase;
            useDefaultDatabase = EditorGUILayout.Toggle(useDefaultDatabaseLabel, useDefaultDatabase);

            if (prevUseDefaultDatabase != useDefaultDatabase)
            {
                Undo.RecordObject(animator, "Modified " + animator.name);
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

            if (EditorGUI.EndChangeCheck() || value != databaseProp.objectReferenceValue)
            {
                if (serializedObject.hasModifiedProperties) serializedObject.ApplyModifiedProperties();
                forceReprocess = true;
            }


            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(sceneAnimationsProp);
            EditorGUILayout.PropertyField(sceneShowAnimationsProp);
            EditorGUILayout.PropertyField(sceneHideAnimationsProp);
            if (EditorGUI.EndChangeCheck())
            {
                if (serializedObject.hasModifiedProperties) serializedObject.ApplyModifiedProperties();
                forceReprocess = true;
            }
        }

        GUIContent alertDialogDefaultShow;
        GUIContent alertDialogDefaultHide;
        void DrawDefaultHideShow()
        {
            alertDialogDefaultShow.tooltip = animator.CheckDefaultString(Components.Animator.TMPAnimationType.Show);
            alertDialogDefaultHide.tooltip = animator.CheckDefaultString(Components.Animator.TMPAnimationType.Hide);

            EditorGUI.BeginChangeCheck();

            GUILayout.BeginHorizontal();
            string warning = animator.CheckDefaultString(Components.Animator.TMPAnimationType.Show);
            EditorGUILayout.PropertyField(defaultShowStringProp, GUILayout.ExpandWidth(true));

            Rect rect = GUILayoutUtility.GetLastRect();
            rect.x = EditorGUIUtility.labelWidth; ;
            rect.width = 20;

            if (warning != "")
            {
                GUI.Label(rect, alertDialogDefaultShow);
            }
            else
            {
                GUI.Label(rect, new GUIContent(""));
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            string warning2 = animator.CheckDefaultString(Components.Animator.TMPAnimationType.Hide);
            EditorGUILayout.PropertyField(defaultHideStringProp, GUILayout.ExpandWidth(true));

            rect = GUILayoutUtility.GetLastRect();
            rect.x = EditorGUIUtility.labelWidth; ;
            rect.width = 20;


            if (warning2 != "")
            {
                GUI.Label(rect, alertDialogDefaultHide);
            }
            else
            {
                GUI.Label(rect, new GUIContent(""));
            }

            GUILayout.EndHorizontal();
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                animator.UpdateDefaultStrings();
                //animator.ForcePostProcess();
            }
        }

        void DrawExclusions()
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.BeginHorizontal();
            var prev = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 100;
            EditorGUILayout.LabelField("Exclude punctuation?");

            EditorGUIUtility.labelWidth = 40;
            EditorGUILayout.PropertyField(excludePunctuationProp, new GUIContent("Basic"));
            EditorGUILayout.PropertyField(excludePunctuationShowProp, new GUIContent("Show"));
            EditorGUILayout.PropertyField(excludePunctuationHideProp, new GUIContent("Hide"));
            //EditorGUILayout.Toggle(new GUIContent("Basic"), );
            //EditorGUILayout.Toggle(new GUIContent("Show"), true);
            //EditorGUILayout.Toggle(new GUIContent("Hide"), true);
            EditorGUIUtility.labelWidth = prev;
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.PropertyField(excludedProp);
            EditorGUILayout.PropertyField(excludedShowProp);
            EditorGUILayout.PropertyField(excludedHideProp);
            if (EditorGUI.EndChangeCheck())
            {
                if (serializedObject.hasModifiedProperties) serializedObject.ApplyModifiedProperties();
                //forceReprocess = true;
            }
        }

        void RepaintInspector()
        {
            bool prevPreview = previewProp.boolValue;

            EditorGUI.BeginDisabledGroup(Application.isPlaying || !animator.enabled);
            previewProp.boolValue = EditorGUILayout.Toggle(new GUIContent("Preview"), previewProp.boolValue);
            EditorGUI.EndDisabledGroup();

            //if (prevPreview != previewProp.boolValue) forceReprocess = true;

            if (previewProp.boolValue)
            {
                if (!prevPreview)
                {
                    animator.StartPreview();
                }
                //if (!prevPreview) animator.StartAnimating();

                //animator.UpdateAnimations();
                //EditorApplication.QueuePlayerLoopUpdate();
            }
            else if (prevPreview)
            {
                animator.StopPreview();
                //animator.StopAnimating();
                //animator.ResetAnimations();
            }

            if (serializedObject.hasModifiedProperties) serializedObject.ApplyModifiedPropertiesWithoutUndo();

            EditorGUILayout.PropertyField(updateFromProp);
            EditorGUILayout.PropertyField(animateOnStartProp);


            EditorGUILayout.Space(10);
            databaseProp.isExpanded = EditorGUILayout.Foldout(databaseProp.isExpanded, new GUIContent("Animations"), true);
            if (databaseProp.isExpanded)
            {
                EditorGUI.indentLevel++;
                DrawDatabase();
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space(10);
            animationsOverrideProp.isExpanded = EditorGUILayout.Foldout(animationsOverrideProp.isExpanded, new GUIContent("Animator settings"), true);
            if (animationsOverrideProp.isExpanded)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(animationsOverrideProp);
                EditorGUILayout.Space(10);
                DrawDefaultHideShow();

                EditorGUILayout.Space(10);
                DrawExclusions();
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.Space(10);

            if (contextProp.isExpanded = EditorGUILayout.Foldout(contextProp.isExpanded, new GUIContent("Animation Settings"), true))
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(contextScalingProp);
                EditorGUILayout.PropertyField(contextScaledTimeProp);
                EditorGUI.indentLevel--;
            }
        }

        public override void OnInspectorGUI()
        {
            InitGUIContent();

            RepaintInspector();

            if (serializedObject.hasModifiedProperties) serializedObject.ApplyModifiedProperties();

            if (forceReprocess)
            {
                forceReprocess = false;
                animator.ForceReprocess();
            }
        }

        void OnUndoRedo()
        {

            if (previewProp.boolValue)
            {
                //previewProp.boolValue = false;
                animator.StopPreview();

                EditorApplication.delayCall += () =>
                {
                    //serializedObject.FindProperty("preview").boolValue = true;
                    //serializedObject.ApplyModifiedProperties();
                    animator.StartPreview();
                };
            }
        }
    }
}