using UnityEditor;
using UnityEngine.UIElements;

namespace TMPEffects.Editor
{
    internal class TMPEffectsSettingsProvider : SettingsProvider
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
            var provider = new TMPEffectsSettingsProvider(PreferencesPath, SettingsScope.User);

            provider.keywords = GetSearchKeywordsFromGUIContentProperties<Styles>();
            return provider;
        }

        public TMPEffectsSettingsProvider(string path, SettingsScope scope = SettingsScope.User) : base(path, scope) { }

        public static bool IsSettingsAvailable() => TMPEffectsSettings.Get() != null;

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            EnsureSerializedObjectExists();
        }

        private void EnsureSerializedObjectExists()
        {
            if (serializedObject == null)
            {
                serializedObject = new SerializedObject(TMPEffectsSettings.Get());
                defaultAnimationDatabaseProp = serializedObject.FindProperty("defaultAnimationDatabase");
                defaultCommandDatabaseProp = serializedObject.FindProperty("defaultCommandDatabase");
            }
        }

        public override void OnGUI(string searchContext)
        {
            EnsureSerializedObjectExists();

            EditorGUI.indentLevel = 1;

            serializedObject.UpdateIfRequiredOrScript();

            EditorGUILayout.PropertyField(defaultAnimationDatabaseProp);
            EditorGUILayout.PropertyField(defaultCommandDatabaseProp);

            bool changed = serializedObject.ApplyModifiedProperties();
            if (changed)
            {
                TMPEffectsSettings.Save();
            }
        }
    }
}