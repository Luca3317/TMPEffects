using TMPEffects.Databases;
using UnityEditor;

namespace TMPEffects.Editor
{
    [CustomEditor(typeof(TMPBasicAnimationDatabase))]
    public class TMPBasicAnimationDatabaseEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("animations"));
            if (serializedObject.hasModifiedProperties) serializedObject.ApplyModifiedProperties();
        }
    }
}
