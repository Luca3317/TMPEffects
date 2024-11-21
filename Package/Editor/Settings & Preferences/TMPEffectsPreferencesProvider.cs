using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace TMPEffects.Editor
{
    internal class TMPEffectsPreferencesProvider : SettingsProvider
    {
        public const string PreferencesPath = "Preferences/TMPEffects";

        private SerializedObject serializedObject;
        private SerializedProperty defaultAnimationDatabaseProp;
        private SerializedProperty defaultCommandDatabaseProp;
        private SerializedProperty defaultKeywordDatabaseProp;

        class Styles
        {
        }

        [SettingsProvider]
        public static SettingsProvider CreateProvider()
        {
            var provider = new TMPEffectsPreferencesProvider(PreferencesPath, SettingsScope.User);
            provider.keywords = GetSearchKeywordsFromGUIContentProperties<Styles>();
            return provider;
        }

        public TMPEffectsPreferencesProvider(string path, SettingsScope scope = SettingsScope.User) : base(path, scope) { }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            EnsureSerializedObjectExists();
        }

        private bool EnsureSerializedObjectExists()
        {
            var settings = TMPEffectsPreferences.Get();

            if (settings == null)
            {
                return false;
            }

            if (serializedObject == null)
            {
                serializedObject = new SerializedObject(settings);
                defaultAnimationDatabaseProp = serializedObject.FindProperty("defaultAnimationDatabase");
                defaultCommandDatabaseProp = serializedObject.FindProperty("defaultCommandDatabase");
                defaultKeywordDatabaseProp = serializedObject.FindProperty("defaultKeywordDatabase");
            }

            return true;
        }

        public override void OnGUI(string searchContext)
        {
            if (!EnsureSerializedObjectExists()) return;

            EditorGUI.indentLevel = 1;

            serializedObject.UpdateIfRequiredOrScript();
      
            EditorGUIUtility.labelWidth = 180;
            EditorGUILayout.PropertyField(defaultAnimationDatabaseProp);
            EditorGUILayout.PropertyField(defaultCommandDatabaseProp);
            EditorGUILayout.PropertyField(defaultKeywordDatabaseProp);
            EditorGUIUtility.labelWidth = -1;

            bool changed = serializedObject.ApplyModifiedProperties();
            if (changed)
            {
                AssetDatabase.SaveAssetIfDirty(serializedObject.targetObject);
            } 
        }
    }
}

