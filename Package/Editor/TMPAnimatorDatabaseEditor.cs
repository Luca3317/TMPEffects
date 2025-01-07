using TMPEffects.Databases;
using UnityEditor;
using TMPEffects.Databases.AnimationDatabase;
using UnityEngine;

namespace TMPEffects.Editor
{
    [CustomEditor(typeof(TMPAnimationDatabase))]
    public class TMPAnimatorDatabaseEditor : UnityEditor.Editor
    {
        private static class Styles
        {
            public static readonly GUIContent basicAnimationDatabase = new GUIContent("Basic Animation Database", "The basic animation database to use.");
            public static readonly GUIContent showAnimationDatabase = new GUIContent("Show Animation Database", "The show animation database to use.");
            public static readonly GUIContent hideAnimationDatabase = new GUIContent("Hide Animation Database", "The hide animation database to use.");
        }
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var basicDataBase = serializedObject.FindProperty("basicAnimationDatabase");
            EditorGUILayout.PropertyField(basicDataBase, Styles.basicAnimationDatabase);

            if (basicDataBase.objectReferenceValue != null)
            {
                SerializedObject so = new SerializedObject(basicDataBase.objectReferenceValue);
                var basicDatabaseAnimations = so.FindProperty("animations");
                EditorGUILayout.PropertyField(basicDatabaseAnimations);
            }

            EditorGUILayout.Space();

            var showDataBase = serializedObject.FindProperty("showAnimationDatabase");
            EditorGUILayout.PropertyField(showDataBase, Styles.showAnimationDatabase);

            if (showDataBase.objectReferenceValue != null)
            {
                SerializedObject so = new SerializedObject(showDataBase.objectReferenceValue);
                var showDatabaseAnimations = so.FindProperty("showAnimations");
                EditorGUILayout.PropertyField(showDatabaseAnimations);
            }

            EditorGUILayout.Space();

            var hideDataBase = serializedObject.FindProperty("hideAnimationDatabase");
            EditorGUILayout.PropertyField(hideDataBase, Styles.hideAnimationDatabase);

            if (hideDataBase.objectReferenceValue != null)
            {
                SerializedObject so = new SerializedObject(hideDataBase.objectReferenceValue);
                var hideDatabaseAnimations = so.FindProperty("hideAnimations");
                EditorGUILayout.PropertyField(hideDatabaseAnimations);
            }

            if (serializedObject.hasModifiedProperties)
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}


