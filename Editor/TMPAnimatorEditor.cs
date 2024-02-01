using UnityEngine;
using UnityEditor;
using TMPEffects.Components;
using TMPEffects.Databases;

[CustomEditor(typeof(TMPAnimator))]
public class TMPAnimatorEditor : Editor
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

        animator = target as TMPAnimator;

        //wasEnabled = writer.enabled;
        defaultDatabase = (TMPAnimationDatabase)Resources.Load("DefaultAnimationDatabase");
        useDefaultDatabase = defaultDatabase == databaseProp.objectReferenceValue || !serializedObject.FindProperty("initDatabase").boolValue;

        if (!serializedObject.FindProperty("initDatabase").boolValue)
        {
            databaseProp.objectReferenceValue = defaultDatabase;
            serializedObject.FindProperty("initDatabase").boolValue = true;
            serializedObject.ApplyModifiedProperties();
        }

        // TODO This line is necessary as this ensures the processors are up-to-date with the current state of
        // the TMPAnimationDatabase. Could also do this by making TMPAnimationDatabase return with ref; fine for now
        // even better would be a callback when changing the database so you dont have to select the gameobject for
        // changes to show
        animator.UpdateProcessorsWrapper();
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
    }

    GUIContent alertDialogDefaultShow;
    GUIContent alertDialogDefaultHide;
    void DrawDefaultHideShow()
    {
        alertDialogDefaultShow.tooltip = animator.CheckDefaultShowString();
        alertDialogDefaultHide.tooltip = animator.CheckDefaultHideString();

        EditorGUI.BeginChangeCheck();

        GUILayout.BeginHorizontal();
        string warning = animator.CheckDefaultShowString();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("defaultShowString"), GUILayout.ExpandWidth(true));

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
        string warning2 = animator.CheckDefaultHideString();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("defaultHideString"), GUILayout.ExpandWidth(true));

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
            animator.ForcePostProcess();
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

        DrawDatabase();

        EditorGUILayout.PropertyField(animationsOverrideProp);

        EditorGUILayout.Space(10);
        DrawDefaultHideShow();

        EditorGUILayout.Space(10);
        DrawExclusions();

        EditorGUILayout.Space(10);
        if (contextProp.isExpanded = EditorGUILayout.Foldout(contextProp.isExpanded, new GUIContent("Animation Settings")))
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