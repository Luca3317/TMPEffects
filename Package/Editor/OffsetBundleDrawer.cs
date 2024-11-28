using TMPEffects.Parameters;
using UnityEditor;
using UnityEngine;

namespace TMPEffects.Editor
{
    [CustomPropertyDrawer(typeof(OffsetBundle))]
    public class OffsetBundleDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var rect = position;
            rect.height = EditorGUIUtility.singleLineHeight;

            EditorGUI.BeginProperty(position, label, property);
            property.isExpanded = EditorGUI.Foldout(rect, property.isExpanded, label, true);
            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;
                rect.y += EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(rect, property.FindPropertyRelative("offsetProvider"));
                rect.y += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("offsetProvider"));
                EditorGUI.PropertyField(rect, property.FindPropertyRelative("uniformity"));
                rect.y += EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(rect, property.FindPropertyRelative("ignoreAnimatorScaling"));
                rect.y += EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(rect, property.FindPropertyRelative("zeroBasedOffset"));
                EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!property.isExpanded) return EditorGUIUtility.singleLineHeight;
            return EditorGUIUtility.singleLineHeight * 4 +
                   EditorGUI.GetPropertyHeight(property.FindPropertyRelative("offsetProvider"));
        }
    }
}