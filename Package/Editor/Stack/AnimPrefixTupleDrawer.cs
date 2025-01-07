using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPEffects.TMPAnimations.Animations;

namespace TMPEffects.Editor
{
    [CustomPropertyDrawer(typeof(AnimationStack<>.AnimPrefixTuple))]
    public class AnimPrefixTupleDrawer : PropertyDrawer
    {
        private static readonly GUIContent animationGUI = new GUIContent("Animation");
        private static readonly GUIContent prefixGUI = new GUIContent("Prefix", "The prefix used to identify parameters for the respective animation (e.g. if you use prefix \"w:\" for an animation, it will receive the parameter \"w:amp=10\" as \"amp=10\").");
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var animProp = property.FindPropertyRelative("animation");
            var prefixProp = property.FindPropertyRelative("prefix");

            Rect rect0 = new Rect(position.position, new Vector2(position.width / 2f, position.height));
            Rect rect1 = new Rect(new Vector2(position.x + position.width / 2f + 5, position.y), new Vector2(position.width / 2f - 5, position.height));

            var prev = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 40;
            EditorGUI.PropertyField(rect0, prefixProp, prefixGUI);
            EditorGUIUtility.labelWidth = 60;
            EditorGUI.PropertyField(rect1, animProp, animationGUI);
            EditorGUIUtility.labelWidth = prev;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}
