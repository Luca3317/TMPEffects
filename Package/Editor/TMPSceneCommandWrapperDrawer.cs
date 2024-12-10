using System.Collections;
using System.Collections.Generic;
using TMPEffects.TMPCommands;
using UnityEditor;
using UnityEngine;

namespace TMPEffects.Editor
{
    [CustomPropertyDrawer(typeof(TMPSceneCommandWrapper))]
    public class TMPSceneCommandWrapperDrawer : PropertyDrawer
    {
        private static class Styles
        {
        }
        
        public static void DrawUILine(Rect rect, Color color, int thickness = 2, int padding = 10)
        {
            // Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding+thickness));
            rect.height = thickness;
            rect.y+=padding/2;
            rect.x-=2;
            rect.width +=6;
            EditorGUI.DrawRect(rect, color);
        }
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var rect = position;
            
            rect.height = EditorGUIUtility.singleLineHeight / 2f;
            DrawUILine(rect, Color.gray);
            rect.y += rect.height;
            rect.height = EditorGUIUtility.singleLineHeight;

            var typeProp = property.FindPropertyRelative("type");
            EditorGUI.PropertyField(rect, typeProp);
            rect.y += EditorGUIUtility.singleLineHeight;

            DrawUILine(rect, Color.gray);
            rect.y += EditorGUIUtility.singleLineHeight;
            
            if (typeProp.enumValueIndex ==
                (int)TMPSceneCommandWrapper.TMPSceneCommandType.Generic)
            {
                var genericProp = property.FindPropertyRelative("generic");
                rect.height = EditorGUI.GetPropertyHeight(genericProp);
                EditorGUI.PropertyField(rect, genericProp);
            }
            else
            {
                var customProp = property.FindPropertyRelative("custom");
                rect.height = EditorGUI.GetPropertyHeight(customProp);
                EditorGUI.PropertyField(rect, customProp, GUIContent.none); 
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.FindPropertyRelative("type").enumValueIndex ==
                (int)TMPSceneCommandWrapper.TMPSceneCommandType.Generic)
            {
                return (EditorGUIUtility.singleLineHeight * 3.5f) + EditorGUI.GetPropertyHeight(property.FindPropertyRelative("generic"));
            }
            else
            {
                return (EditorGUIUtility.singleLineHeight * 3.5f) + EditorGUI.GetPropertyHeight(property.FindPropertyRelative("custom"));
            }
        }
    }
}