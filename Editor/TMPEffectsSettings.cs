using UnityEngine;
using UnityEditor;
using TMPEffects.Databases.AnimationDatabase;
using TMPEffects.Databases.CommandDatabase;

namespace TMPEffects.Editor
{
    internal sealed class TMPEffectsSettings : ScriptableObject
    {
        public TMPAnimationDatabase DefaultAnimationDatabase => defaultAnimationDatabase;
        public TMPCommandDatabase DefaultCommandDatabase => defaultCommandDatabase;

        [SerializeField] private TMPAnimationDatabase defaultAnimationDatabase;
        [SerializeField] private TMPCommandDatabase defaultCommandDatabase;

        internal static TMPEffectsSettings Get()
        {
            var settings = Resources.Load<TMPEffectsSettings>("TMPEffectsSettings");

            if (settings != null) return settings;

            TMPEffectsImporterWindow.ShowImporterWindow();

            return null;
        }
    }
} 