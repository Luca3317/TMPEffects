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
            }

            return true;
        }

        public override void OnGUI(string searchContext)
        {
            if (!EnsureSerializedObjectExists()) return;

            EditorGUI.indentLevel = 1;

            serializedObject.UpdateIfRequiredOrScript();

            EditorGUILayout.PropertyField(defaultAnimationDatabaseProp);
            EditorGUILayout.PropertyField(defaultCommandDatabaseProp);

            bool changed = serializedObject.ApplyModifiedProperties();
            if (changed)
            {
                AssetDatabase.SaveAssetIfDirty(serializedObject.targetObject);
            }
        }
    }
}

