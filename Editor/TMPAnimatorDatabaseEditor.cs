using TMPEffects.Databases;
using UnityEditor;

namespace TMPEffects.Editor
{
    [CustomEditor(typeof(TMPAnimationDatabase))]
    public class TMPAnimatorDatabaseEditor : UnityEditor.Editor
    {
        private UnityEditor.Editor _editor1;
        private UnityEditor.Editor _editor2;
        private UnityEditor.Editor _editor3;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var dataBase = serializedObject.FindProperty("basicAnimationDatabase");
            EditorGUILayout.PropertyField(dataBase);

            if (dataBase.objectReferenceValue != null)
            {
                CreateCachedEditor(dataBase.objectReferenceValue, typeof(TMPBasicAnimationDatabaseEditor), ref _editor1);
                _editor1.OnInspectorGUI();
            }

            EditorGUILayout.Space();

            var showDataBase = serializedObject.FindProperty("showAnimationDatabase");
            EditorGUILayout.PropertyField(showDataBase);

            if (showDataBase.objectReferenceValue != null)
            {
                CreateCachedEditor(showDataBase.objectReferenceValue, typeof(TMPShowAnimationDatabaseEditor), ref _editor2);
                _editor2.OnInspectorGUI();
            }

            EditorGUILayout.Space();

            var hideDataBase = serializedObject.FindProperty("hideAnimationDatabase");
            EditorGUILayout.PropertyField(hideDataBase);

            if (hideDataBase.objectReferenceValue != null)
            {
                CreateCachedEditor(hideDataBase.objectReferenceValue, typeof(TMPHideAnimationDatabaseEditor), ref _editor3);
                _editor3.OnInspectorGUI();
            }

            if (serializedObject.hasModifiedProperties)
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}


