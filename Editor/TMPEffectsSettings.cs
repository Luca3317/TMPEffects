using UnityEngine;
using UnityEditor;
using TMPEffects.Databases.AnimationDatabase;
using TMPEffects.Databases.CommandDatabase;

namespace TMPEffects.Editor
{
    internal sealed class TMPEffectsSettings : ScriptableObject
    {
        public const string TMPEffects_Settings_Path = "Assets/Editor/TMPEffectsSettingsAsset.asset";//"Packages/TMPEffects/TMPEffectsSettingsAsset.asset";

        public TMPAnimationDatabase DefaultAnimationDatabase => defaultAnimationDatabase;
        public TMPCommandDatabase DefaultCommandDatabase => defaultCommandDatabase;

        [SerializeField] private TMPAnimationDatabase defaultAnimationDatabase;
        [SerializeField] private TMPCommandDatabase defaultCommandDatabase;

        internal static TMPEffectsSettings Get()
        {
            var settings = AssetDatabase.LoadAssetAtPath<TMPEffectsSettings>(TMPEffects_Settings_Path);
            if (settings == null)
            {
                settings = ScriptableObject.CreateInstance<TMPEffectsSettings>();

                var defaultcommanddb = Resources.Load("DefaultCommandDatabase");
                var defaultanimationdb = Resources.Load("DefaultAnimationDatabase");

                settings.defaultAnimationDatabase = defaultanimationdb as TMPAnimationDatabase;
                settings.defaultCommandDatabase = defaultcommanddb as TMPCommandDatabase;

                AssetDatabase.CreateAsset(settings, TMPEffects_Settings_Path);
                AssetDatabase.SaveAssets();
            }
            return settings;
        }

        internal static void Save()
        {
            AssetDatabase.SaveAssets();
        }
    }
} 