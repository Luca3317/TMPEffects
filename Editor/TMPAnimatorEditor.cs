using UnityEngine;
using UnityEditor;

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
        updateFromProp = serializedObject.FindProperty("updateFrom");
        animateOnStartProp = serializedObject.FindProperty("animateOnStart");
        contextProp = serializedObject.FindProperty("context");
        contextScalingProp = contextProp.FindPropertyRelative("scaleAnimations");
        contextScaledTimeProp = contextProp.FindPropertyRelative("useScaledTime");
        previewProp = serializedObject.FindProperty("preview");

        animator = target as TMPAnimatorFinal;
        //wasEnabled = writer.enabled;
        defaultDatabase = (TMPEffectsDatabase)Resources.Load("DefaultAnimationDatabase");
        useDefaultDatabase = defaultDatabase == databaseProp.objectReferenceValue || databaseProp.objectReferenceValue == null;

        if (databaseProp.objectReferenceValue == null)
        {
            databaseProp.objectReferenceValue = defaultDatabase;
            serializedObject.ApplyModifiedProperties();
        }
         
        animator.ForceReprocess();

        //if (previewProp.boolValue)
        //{
        //    animator.NewStartPreview(); 
        //}
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

    void RepaintInspector()
    {
        bool prevPreview = previewProp.boolValue;

        EditorGUI.BeginDisabledGroup(Application.isPlaying);
        previewProp.boolValue = EditorGUILayout.Toggle(new GUIContent("Preview"), previewProp.boolValue);
        EditorGUI.EndDisabledGroup();

        if (previewProp.boolValue)
        {
            if (!prevPreview) animator.StartPreview();
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

    public override bool RequiresConstantRepaint()
    {
        //return true;
        return previewProp.boolValue;
    }
}
 