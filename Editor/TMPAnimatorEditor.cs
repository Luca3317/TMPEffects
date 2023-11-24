using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Codice.Client.BaseCommands;

[CustomEditor(typeof(TMPAnimatorFinal))]
public class TMPAnimatorEditor : Editor
{
    TMPAnimatorFinal animator;

    bool useDefaultDatabase;
    TMPEffectsDatabase defaultDatabase;

    // Serialized properties
    SerializedProperty databaseProp;
    SerializedProperty updateFromProp;
    SerializedProperty animateOnStartProp;
    //SerializedProperty animateOnTextChangeProp;

    GUIContent useDefaultDatabaseLabel;

    bool guiContent = false;
    bool forceReprocess = false;

    private void OnEnable()
    {
        databaseProp = serializedObject.FindProperty("database");
        updateFromProp = serializedObject.FindProperty("updateFrom");
        animateOnStartProp = serializedObject.FindProperty("animateOnStart");

        animator = target as TMPAnimatorFinal;
        //wasEnabled = writer.enabled;
        defaultDatabase = (TMPEffectsDatabase)Resources.Load("DefaultAnimationDatabase");
        useDefaultDatabase = defaultDatabase == databaseProp.objectReferenceValue || databaseProp.objectReferenceValue == null;

        if (databaseProp.objectReferenceValue == null)
        {
            databaseProp.objectReferenceValue = defaultDatabase;
            serializedObject.ApplyModifiedProperties();
            animator.ForceReprocess();
        }
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
            Debug.Log("CHANGHED");
            forceReprocess = true;
        }
    }

    bool preview = false;

    void RepaintInspector()
    {
        bool prevPreview = preview;

        EditorGUI.BeginDisabledGroup(Application.isPlaying);
        preview = EditorGUILayout.Toggle(new GUIContent("Preview"), preview);
        EditorGUI.EndDisabledGroup();

        if (preview)
        {
            if (!prevPreview) animator.StartAnimating();

            animator.UpdateAnimations();
            EditorApplication.QueuePlayerLoopUpdate();
        }
        else if (prevPreview)
        {
            animator.StopAnimating();
            animator.ResetAnimations();
        }

        EditorGUILayout.PropertyField(updateFromProp);
        EditorGUILayout.PropertyField(animateOnStartProp);

        DrawDatabase();
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

    public override bool RequiresConstantRepaint()
    {
        return true;
        return preview;
    }
}
