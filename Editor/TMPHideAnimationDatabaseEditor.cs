using TMPEffects.Databases;
using UnityEditor;

namespace TMPEffects.Editor
{
    [CustomEditor(typeof(TMPHideAnimationDatabase))]
    public class TMPHideAnimationDatabaseEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("hideAnimations"));
            if (serializedObject.hasModifiedProperties) serializedObject.ApplyModifiedProperties();
        }
    }
}
