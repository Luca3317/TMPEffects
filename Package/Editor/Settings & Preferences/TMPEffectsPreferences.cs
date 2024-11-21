using TMPEffects.Databases;
using UnityEngine;
using UnityEditor;
using TMPEffects.Databases.AnimationDatabase;
using TMPEffects.Databases.CommandDatabase;

namespace TMPEffects.Editor
{
    internal sealed class TMPEffectsPreferences : ScriptableObject
    {
        public TMPAnimationDatabase DefaultAnimationDatabase => defaultAnimationDatabase;
        public TMPCommandDatabase DefaultCommandDatabase => defaultCommandDatabase;
        public TMPKeywordDatabase DefaultKeywordDatabase => defaultKeywordDatabase;

        [SerializeField] private TMPAnimationDatabase defaultAnimationDatabase;
        [SerializeField] private TMPCommandDatabase defaultCommandDatabase;
        [SerializeField] private TMPKeywordDatabase defaultKeywordDatabase;

        internal static TMPEffectsPreferences Get()
        {
            var preferences = Resources.Load<TMPEffectsPreferences>("TMPEffectsSettings");

            if (preferences != null) return preferences;

            TMPEffectsImporterWindow.ShowImporterWindow();

            return null;
        }
    }
}  