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

        class Styles
        {
        }

        public TMPEffectsSettingsProvider() : base("Project/TMPEffects", SettingsScope.Project)
        {
        }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
        }

        public override void OnGUI(string searchContext)
        {
            bool optOutofWindow = EditorPrefs.GetBool(TMPEffectsEditorPrefsKeys.OptOutKey, true);
            bool newOptOutofWindow = EditorGUILayout.Toggle("Opt out of bug report window", optOutofWindow);
            if (newOptOutofWindow != optOutofWindow)
            {
                EditorPrefs.SetBool(TMPEffectsEditorPrefsKeys.OptOutKey, newOptOutofWindow);
            }

            EditorGUILayout.Space();
        }
    }
}