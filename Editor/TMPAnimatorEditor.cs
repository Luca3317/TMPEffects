using UnityEngine;
using UnityEditor;
using TMPro;

[CustomEditor(typeof(TMPAnimator))]
public class TMPAnimatorEditor : Editor
{
    TMPAnimator animator;

    bool useDefaultDatabase;
    TMPAnimationDatabase defaultDatabase;

    // Serialized properties
    SerializedProperty databaseProp;
    SerializedProperty showAnimProp;
    SerializedProperty hideAnimProp;
    SerializedProperty updateFromProp;
    SerializedProperty animateOnStartProp;
    SerializedProperty contextProp;
    SerializedProperty contextScalingProp;
    SerializedProperty contextScaledTimeProp;
    SerializedProperty previewProp;
    //SerializedProperty animateOnTextChangeProp;

    GUIContent useDefaultDatabaseLabel;

    // TODO Move preview to TMPAnimator as editor-only field to make its state persistent between hierarchy selections; serialize too?
    //bool preview = false;
    bool guiContent = false;
    bool forceReprocess = false;

    private void OnEnable()
    {
        databaseProp = serializedObject.FindProperty("database");
        showAnimProp = serializedObject.FindProperty("defaultShowAnimation");
        hideAnimProp = serializedObject.FindProperty("defaultHideAnimation");
        updateFromProp = serializedObject.FindProperty("updateFrom");
        animateOnStartProp = serializedObject.FindProperty("animateOnStart");
        contextProp = serializedObject.FindProperty("context");
        contextScalingProp = contextProp.FindPropertyRelative("scaleAnimations");
        contextScaledTimeProp = contextProp.FindPropertyRelative("useScaledTime");
        previewProp = serializedObject.FindProperty("preview");

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
    } 

    void InitGUIContent()
    {
        if (guiContent) return;
        guiContent = true;

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
            // TODO
            Undo.RecordObject(animator, "Modified " + animator.name);
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

        if (EditorGUI.EndChangeCheck() || value != databaseProp.objectReferenceValue)
        {
            forceReprocess = true;
        }
    }

    void DrawDefaultHideShow()
    {
        EditorGUILayout.PropertyField(showAnimProp);
        EditorGUILayout.PropertyField(hideAnimProp);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("defaultShowString"));
    }

    void RepaintInspector()
    {
        bool prevPreview = previewProp.boolValue;

        EditorGUI.BeginDisabledGroup(Application.isPlaying);
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

        EditorGUILayout.PropertyField(updateFromProp);
        EditorGUILayout.PropertyField(animateOnStartProp);

        DrawDatabase();

        DrawDefaultHideShow();

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
        Debug.Log("ON UNDO REDO");

        if (previewProp.boolValue)
        {
            //animator.ForceReprocess();
            previewProp.boolValue = false;
            animator.StopPreview();

            EditorApplication.delayCall += () =>
            {
                serializedObject.FindProperty("preview").boolValue = true;
                serializedObject.ApplyModifiedProperties();
                animator.StartPreview();
            };
        }
    }
}