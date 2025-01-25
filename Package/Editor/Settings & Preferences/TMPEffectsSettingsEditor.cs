using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace TMPEffects.Editor
{
    [CustomEditor(typeof(TMPEffectsSettings))]
    public class TMPEffectsSettingsEditor : UnityEditor.Editor
    {
        internal class Styles
        {
            public static readonly GUIContent optOutBugReport = new GUIContent("Opt out of Bug Report Window",
                "Whether you will get a bug report window for some TMPEffects bugs or not.");
            
            public static readonly GUIContent defaultAnimationDatabase = new GUIContent("Default Animation Database",
                "The default animation database that TMPAnimator components will use.");

            public static readonly GUIContent defaultCommandDatabase = new GUIContent("Default Command Database",
                "The default command database that TMPWriter components will use.");

            public static readonly GUIContent defaultKeywordDatabase = new GUIContent("Default Keyword Database",
                "The default keyword database that TMPEffects components will use.");

            public static readonly GUIContent globalKeywordDatabase = new GUIContent("Global Keyword Database",
                "The keyword database that defines globally valid keywords.");
        }

        private SerializedProperty defaultAnimationDatabase;
        private SerializedProperty defaultCommandDatabase;
        private SerializedProperty defaultKeywordDatabase;
        private SerializedProperty globalKeywordDatabase;

        private void OnEnable()
        {
            defaultAnimationDatabase = serializedObject.FindProperty("defaultAnimationDatabase");
            defaultCommandDatabase = serializedObject.FindProperty("defaultCommandDatabase");
            defaultKeywordDatabase = serializedObject.FindProperty("defaultKeywordDatabase");
            globalKeywordDatabase = serializedObject.FindProperty("globalKeywordDatabase");
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("Databases", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(defaultAnimationDatabase, Styles.defaultAnimationDatabase);
            EditorGUILayout.PropertyField(defaultCommandDatabase, Styles.defaultCommandDatabase);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(defaultKeywordDatabase, Styles.defaultKeywordDatabase);
            EditorGUILayout.PropertyField(globalKeywordDatabase, Styles.globalKeywordDatabase);
            EditorGUI.indentLevel--;
            
            EditorGUILayout.Space();
            
            EditorGUILayout.LabelField("Miscellaneous", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            bool optOutofWindow = EditorPrefs.GetBool(TMPEffectsEditorPrefsKeys.OptOutKey, true);
            bool newOptOutofWindow = EditorGUILayout.Toggle(Styles.optOutBugReport, optOutofWindow);
            if (newOptOutofWindow != optOutofWindow)
            {
                EditorPrefs.SetBool(TMPEffectsEditorPrefsKeys.OptOutKey, newOptOutofWindow);
            }
            EditorGUI.indentLevel--;

            if (serializedObject.hasModifiedProperties)
                serializedObject.ApplyModifiedProperties();
        }
    }

    internal class TMPEffectsSettingsProvider : SettingsProvider
    {
        [SettingsProviderGroup]
        static SettingsProvider[] CreateTMPEffectsSettingsProviders()
        {
            var providers = new List<SettingsProvider>() { new TMPEffectsSettingsProvider() };

            if (GetSettings() != null)
            {
                var provider = new AssetSettingsProvider("Project/TMPEffects/Settings", GetSettings);
                provider.PopulateSearchKeywordsFromGUIContentProperties<TMPEffectsSettingsEditor.Styles>();
                providers.Add(provider);
            }

            return providers.ToArray();
        }

        static UnityEngine.Object GetSettings()
        {
            return Resources.Load<TMPEffectsSettings>("TMPEffects Settings");
        }

        public const string SettingsPath = "Project/TMPEffects";

        public TMPEffectsSettingsProvider() : base("Project/TMPEffects", SettingsScope.Project)
        {
        }

        public override void OnGUI(string searchContext)
        {
            EditorGUI.indentLevel++;
            
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.richText = true;
            
            GUIContent content = new GUIContent();
            content.text = @"Settings for the TMPEffects package.

Full <color=lightblue>DOCUMENTATION</color> can be found here: https://tmpeffects.luca3317.dev
The <color=lightblue>GITHUB</color> repository can be found here: https://github.com/Luca3317/TMPEffects";

            float height = style.CalcHeight(content, EditorGUIUtility.currentViewWidth);
            EditorGUILayout.SelectableLabel(content.text, style, GUILayout.Height(height));
            EditorGUI.indentLevel--;
        }
        
    }
}