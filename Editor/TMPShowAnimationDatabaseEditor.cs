using TMPEffects.Databases;
using UnityEditor;

namespace TMPEffects.Editor
{
    [CustomEditor(typeof(TMPShowAnimationDatabase))]
    public class TMPShowAnimationDatabaseEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("showAnimations"));
            if (serializedObject.hasModifiedProperties) serializedObject.ApplyModifiedProperties();
        }
    }
}
