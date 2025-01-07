using TMPEffects.Parameters;
using UnityEditor;
using UnityEngine;

namespace TMPEffects.Editor
{
    [CustomPropertyDrawer(typeof(OffsetBundle))]
    public class OffsetBundleDrawer : OffsetBundleDrawerBase
    {
    }

    [CustomPropertyDrawer(typeof(SceneOffsetBundle))]
    public class SceneOffsetBundleDrawer : OffsetBundleDrawerBase
    {
    }

    public class OffsetBundleDrawerBase : PropertyDrawer
    {
        internal static class Styles
        {
            public static readonly GUIContent offsetProvider = new GUIContent("Offset Provider",
                "The offset provider used to calculate the offset.");

            public static readonly GUIContent uniformity = new GUIContent("Uniformity",
                "The uniformity that will be applied to the offset.");

            public static readonly GUIContent ignoreAnimatorScaling = new GUIContent("Ignore Animator Scaling",
                "Whether to ignore the animator's scaling of the offset, if applicable.");

            public static readonly GUIContent zeroBasedOffset = new GUIContent("Zero Based Offset",
                "Whether to zero-base the offset, i.e. whether to shift it so the minimum offset is zero.");
        }

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
                var offsetProvider = property.FindPropertyRelative("offsetProvider");
                EditorGUI.PropertyField(rect, offsetProvider, Styles.offsetProvider);
                rect.y += EditorGUI.GetPropertyHeight(offsetProvider);
                EditorGUI.PropertyField(rect, property.FindPropertyRelative("uniformity"), Styles.uniformity);
                rect.y += EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(rect, property.FindPropertyRelative("ignoreAnimatorScaling"),
                    Styles.ignoreAnimatorScaling);
                rect.y += EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(rect, property.FindPropertyRelative("zeroBasedOffset"), Styles.zeroBasedOffset);
                EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!property.isExpanded) return EditorGUIUtility.singleLineHeight;
            return (EditorGUIUtility.singleLineHeight * 4) +
                   EditorGUI.GetPropertyHeight(property.FindPropertyRelative("offsetProvider"));
        }
    }
}