using UnityEditor;
using TMPEffects.TMPAnimations.Animations;

namespace TMPEffects.Editor
{
    [CustomEditor(typeof(AnimationStackObject))]
    public class AnimationStackObjectEditor : UnityEditor.Editor
    {
        SerializedProperty animsProp;

        void OnEnable()
        {
            animsProp = serializedObject.FindProperty("stack").FindPropertyRelative("animations");
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(animsProp);

            if (serializedObject.hasModifiedProperties)
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
