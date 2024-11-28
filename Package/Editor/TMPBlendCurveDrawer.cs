using TMPEffects.Parameters;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(TMPBlendCurve))]
public class TMPBlendCurveDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // EditorGUI.PropertyField(position, property.FindPropertyRelative("curve"), label);
        EditorGUI.BeginProperty(position, label, property);
        
        var rect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        
        var width = EditorStyles.label.CalcSize(label);
        var ctrlRect = EditorGUI.PrefixLabel(rect, GUIUtility.GetControlID(FocusType.Passive, rect), label);
        var foldoutRect = new Rect(rect.x + width.x + 15, rect.y, 20, EditorGUIUtility.singleLineHeight);
        
        var curveProp = property.FindPropertyRelative("curve");
        
        curveProp.isExpanded = EditorGUI.Foldout(foldoutRect, curveProp.isExpanded, new GUIContent(""));
        
        // Weird ugly fix for indenting issue w/ prefixlabel
        EditorGUI.indentLevel--;
        EditorGUI.indentLevel--;
        EditorGUI.PropertyField(ctrlRect, curveProp, new GUIContent(""));
        EditorGUI.indentLevel++; 
        EditorGUI.indentLevel++;
        
        if (curveProp.isExpanded)
        {
            EditorGUI.indentLevel++; 
            rect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("offsetProvider"));
            rect.y += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("offsetProvider"));
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("uniformity"));
            rect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("ignoreAnimatorScaling"));
            rect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("finishWholeSegmentInTime"));
            rect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("zeroBasedOffset"));
            EditorGUI.indentLevel--;
        }
        
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var curveProp = property.FindPropertyRelative("curve");
        if (curveProp.isExpanded)
        {
            return EditorGUIUtility.singleLineHeight * 5 + EditorGUI.GetPropertyHeight(property.FindPropertyRelative("offsetProvider"));
        }
        
        return EditorGUIUtility.singleLineHeight;
    }
}