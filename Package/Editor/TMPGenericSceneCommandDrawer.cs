using TMPEffects.TMPCommands;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(TMPGenericSceneCommand))]
public class TMPGenericSceneCommandDrawer : PropertyDrawer
{
    private static class Styles
    {
        public static readonly GUIContent commandType = EditorGUIUtility.TrTextContent("Command Type", "The type of command this is. Index-based commands don't need their tags to be closed, block-based commands do.");
        public static readonly GUIContent executeInstantly = EditorGUIUtility.TrTextContent("Execute Instantly", "Whether the command is executed the moment the TMPWriter begins writing. Otherwise, it is executed when the TMPWriter shows the character at the corresponding index.");
        public static readonly GUIContent executeOnSkip = EditorGUIUtility.TrTextContent("Execute On Skip", "Whether the command should be executed by the TMPWriter if its text position is skipped over.");
        public static readonly GUIContent executeRepeatable = EditorGUIUtility.TrTextContent("Execute Repeatable", "Whether the command may be executed multiple times if, for example, the TMPWriter is reset to an earlier text position after the command has been executed. An example for a command that should not be repeatable is one that triggers a quest, or adds an item to the player's inventory.");
    }
    
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var rect = position;
        rect.height = EditorGUIUtility.singleLineHeight;

        EditorGUI.PropertyField(rect, property.FindPropertyRelative("commandType"), Styles.commandType);
        rect.y += rect.height;
        EditorGUI.PropertyField(rect, property.FindPropertyRelative("executeInstantly"), Styles.executeInstantly);
        rect.y += rect.height;
        EditorGUI.PropertyField(rect, property.FindPropertyRelative("executeOnSkip"), Styles.executeOnSkip);
        rect.y += rect.height;
        EditorGUI.PropertyField(rect, property.FindPropertyRelative("executeRepeatable"), Styles.executeRepeatable);
        rect.y += rect.height;

        rect.height = EditorGUI.GetPropertyHeight(property.FindPropertyRelative("command"));
        EditorGUI.PropertyField(rect, property.FindPropertyRelative("command"));
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return (EditorGUIUtility.singleLineHeight * 4) + EditorGUI.GetPropertyHeight(property.FindPropertyRelative("command"));
    }
}
