using TMPEffects;
using UnityEditor;
using TMPEffects.TMPCommands;

namespace TMPEffects.Editor
{
    [CustomEditor(typeof(TMPCommand), editorForChildClasses: true)]
    public class TMPCommandEditor : UnityEditor.Editor
    {
        SerializedProperty commandTypeProp;
        SerializedProperty executeInstantlyProp;

        private void OnEnable()
        {
            commandTypeProp = serializedObject.FindProperty("commandType");
            executeInstantlyProp = serializedObject.FindProperty("executeInstantly");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            //commandTypeProp.enumValueIndex = System.Convert.ToInt32(EditorGUILayout.EnumPopup(new GUIContent("Command Type"), (TMPCommand.CommandType)commandTypeProp.enumValueIndex));

            //if ((TMPCommand.CommandType)commandTypeProp.enumValueIndex != TMPCommand.CommandType.Index)
            //{
            //    executeInstantlyProp.boolValue = EditorGUILayout.Toggle(new GUIContent("Execute instantly"), executeInstantlyProp.boolValue);
            //}

            //if (serializedObject.hasModifiedProperties)
            //{
            //    serializedObject.ApplyModifiedProperties();
            //}
        }
    }
}