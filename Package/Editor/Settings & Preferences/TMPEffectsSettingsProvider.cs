using System.Collections;using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace TMPEffects.Editor
{
    internal class TMPEffectsSettingsProvider : SettingsProvider
    {
        public const string SettingsPath = "Project/TMPEffects";

        private SerializedObject serializedObject;
        private SerializedProperty globalKeywordDatabaseProp;
        private SerializedProperty animationTagPrefixProp;
        private SerializedProperty showAnimationTagPrefixProp;
        private SerializedProperty hideAnimationTagPrefixProp;
        private SerializedProperty commandTagPrefixProp;
        private SerializedProperty eventTagPrefixProp;

        class Styles
        {
        }

        [SettingsProvider]
        public static SettingsProvider CreateProvider()
        {
            var provider = new TMPEffectsSettingsProvider(SettingsPath, SettingsScope.Project);
            provider.keywords = GetSearchKeywordsFromGUIContentProperties<Styles>();
            return provider;
        }

        public TMPEffectsSettingsProvider(string path, SettingsScope scope = SettingsScope.Project) : base(path, scope) { }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            EnsureSerializedObjectExists();
        }

        private bool EnsureSerializedObjectExists()
        {
            var settings = TMPEffectsSettings.Get();

            if (settings == null)
            {
                return false;
            }

            if (serializedObject == null)
            {
                serializedObject = new SerializedObject(settings);
                globalKeywordDatabaseProp = serializedObject.FindProperty("globalKeywordDatabase");
                animationTagPrefixProp = serializedObject.FindProperty("animationTagPrefix");
                showAnimationTagPrefixProp = serializedObject.FindProperty("showAnimationTagPrefix");
                hideAnimationTagPrefixProp = serializedObject.FindProperty("hideAnimationTagPrefix");
                commandTagPrefixProp = serializedObject.FindProperty("commandTagPrefix");
                eventTagPrefixProp = serializedObject.FindProperty("eventTagPrefix");
            }

            return true;
        }

        public override void OnGUI(string searchContext)
        {
            if (!EnsureSerializedObjectExists()) return;

            EditorGUI.indentLevel = 1;

            serializedObject.UpdateIfRequiredOrScript();

            EditorGUILayout.PropertyField(globalKeywordDatabaseProp);
            EditorGUILayout.PropertyField(animationTagPrefixProp);
            EditorGUILayout.PropertyField(showAnimationTagPrefixProp);
            EditorGUILayout.PropertyField(hideAnimationTagPrefixProp);
            EditorGUILayout.PropertyField(commandTagPrefixProp);
            EditorGUILayout.PropertyField(eventTagPrefixProp);

            bool changed = serializedObject.ApplyModifiedProperties();
            if (changed)
            {
                AssetDatabase.SaveAssetIfDirty(serializedObject.targetObject);
            }
        }
    }
}