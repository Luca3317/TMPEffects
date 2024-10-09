using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPEffects.Components;
using UnityEditor;
using UnityEngine;



[CustomPropertyDrawer(typeof(TMPWriterClipData))]
public class TMPWriterClipDataDrawer : PropertyDrawer
{
    private string[] methodNames;
    private int selectedMethodIndex = 0;
    private MethodInfo selectedMethod;
    private object[] parameterValues;
    
    private void OnEnable()
    {
        MethodInfo[] methods = typeof(TMPWriter).GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        methodNames = new string[methods.Length];
        for (int i = 0; i < methods.Length; i++)
        {
            methodNames[i] = methods[i].Name;
        }
    }
    
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        
    }
}

// [CustomPropertyDrawer(typeof(TMPWriterClipData))]
// public class TMPWriterClipDataDrawer : PropertyDrawer
// {
//     public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//     {
//         
//         
//         
//         
//         // EditorGUI.PropertyField(position, property.FindPropertyRelative("method"));
//         // position.y += EditorGUIUtility.singleLineHeight;
//
//         // switch ((TMPWriterClipMethods)property.FindPropertyRelative("method").enumValueIndex)
//         // {
//         //     case TMPWriterClipMethods.Start:
//         //         DrawStart(position, property);
//         //         break;
//         //     case TMPWriterClipMethods.Stop:
//         //         DrawStop(position, property);
//         //         break;
//         //     case TMPWriterClipMethods.Skip:
//         //         DrawSkip(position, property);
//         //         break;
//         //     case TMPWriterClipMethods.Reset:
//         //         DrawReset(position, property);
//         //         break;
//         //     case TMPWriterClipMethods.Restart:
//         //         DrawRestart(position, property);
//         //         break;
//         //     case TMPWriterClipMethods.Wait:
//         //         DrawWait(position, property);
//         //         break;
//         //     case TMPWriterClipMethods.SetDelay:
//         //         DrawSetDelay(position, property);
//         //         break;
//         //     case TMPWriterClipMethods.SetSkippable:
//         //         DrawSetSkippable(position, property);
//         //         break;
//         //     default:
//         //         throw new System.NotImplementedException(
//         //             $"No drawer for {(TMPWriterClipMethods)property.FindPropertyRelative("method").enumValueIndex}");
//         // }
//
//         // if (property.serializedObject.hasModifiedProperties) property.serializedObject.ApplyModifiedProperties();
//     }
//
//     private void DrawSetSkippable(Rect position, SerializedProperty property)
//     {
//     }
//
//     private void DrawSetDelay(Rect position, SerializedProperty property)
//     {
//     }
//
//     private void DrawWait(Rect position, SerializedProperty property)
//     {
//     }
//
//     private void DrawRestart(Rect position, SerializedProperty property)
//     {
//     }
//
//     private void DrawReset(Rect position, SerializedProperty property)
//     {
//     }
//
//     private void DrawSkip(Rect position, SerializedProperty property)
//     {
//         var skipShowAnimProp = property.FindPropertyRelative("boolValue");
//         skipShowAnimProp.boolValue =
//             EditorGUI.Toggle(position, new GUIContent("Sss"), skipShowAnimProp.boolValue);
//     }
//
//     private TMPWriter w;
//
//     private void DrawStop(Rect position, SerializedProperty property)
//     {
//         //
//     }
//
//     private void DrawStart(Rect rect, SerializedProperty property)
//     {
//         //
//     }
//
//     public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
//     {
//         float val = EditorGUIUtility.singleLineHeight;
//         switch ((TMPWriterClipMethods)property.FindPropertyRelative("method").enumValueIndex)
//         {
//             case TMPWriterClipMethods.Start:
//                 val += GetHeightStart(property);
//                 break;
//             case TMPWriterClipMethods.Stop:
//                 val += GetHeightStop(property);
//                 break;
//             case TMPWriterClipMethods.Skip:
//                 val += GetHeightSkip(property);
//                 break;
//             case TMPWriterClipMethods.Reset:
//                 val += GetHeightReset(property);
//                 break;
//             case TMPWriterClipMethods.Restart:
//                 val += GetHeightRestart(property);
//                 break;
//             case TMPWriterClipMethods.Wait:
//                 val += GetHeightWait(property);
//                 break;
//             case TMPWriterClipMethods.SetDelay:
//                 val += GetHeightSetDelay(property);
//                 break;
//             case TMPWriterClipMethods.SetSkippable:
//                 val += GetHeightSetSkippable(property);
//                 break;
//             default:
//                 throw new System.NotImplementedException(
//                     $"No drawer for {(TMPWriterClipMethods)property.FindPropertyRelative("method").enumValueIndex}");
//         }
//
//         return val;
//     }
//
//     private float GetHeightSetSkippable(SerializedProperty property)
//     {
//         return 0;
//     }
//
//     private float GetHeightSetDelay(SerializedProperty property)
//     {
//         return 0;
//     }
//
//     private float GetHeightWait(SerializedProperty property)
//     {
//         return 0;
//     }
//
//     private float GetHeightRestart(SerializedProperty property)
//     {
//         return 0;
//     }
//
//     private float GetHeightReset(SerializedProperty property)
//     {
//         return 0;
//     }
//
//     private float GetHeightSkip(SerializedProperty property)
//     {
//         return EditorGUIUtility.singleLineHeight;
//     }
//
//     private float GetHeightStop(SerializedProperty property)
//     {
//         return 0;
//     }
//
//     private float GetHeightStart(SerializedProperty property)
//     {
//         return 0;
//     }
// }