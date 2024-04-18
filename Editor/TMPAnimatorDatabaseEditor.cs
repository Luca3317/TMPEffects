using TMPEffects.Databases;
using UnityEditor;
using TMPEffects.Databases.AnimationDatabase;

namespace TMPEffects.Editor
{
    [CustomEditor(typeof(TMPAnimationDatabase))]
    public class TMPAnimatorDatabaseEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var basicDataBase = serializedObject.FindProperty("basicAnimationDatabase");
            EditorGUILayout.PropertyField(basicDataBase);

            if (basicDataBase.objectReferenceValue != null)
            {
                SerializedObject so = new SerializedObject(basicDataBase.objectReferenceValue);
                var basicDatabaseAnimations = so.FindProperty("animations");
                EditorGUILayout.PropertyField(basicDatabaseAnimations);
            }

            EditorGUILayout.Space();

            var showDataBase = serializedObject.FindProperty("showAnimationDatabase");
            EditorGUILayout.PropertyField(showDataBase);

            if (showDataBase.objectReferenceValue != null)
            {
                SerializedObject so = new SerializedObject(showDataBase.objectReferenceValue);
                var showDatabaseAnimations = so.FindProperty("showAnimations");
                EditorGUILayout.PropertyField(showDatabaseAnimations);
            }

            EditorGUILayout.Space();

            var hideDataBase = serializedObject.FindProperty("hideAnimationDatabase");
            EditorGUILayout.PropertyField(hideDataBase);

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


