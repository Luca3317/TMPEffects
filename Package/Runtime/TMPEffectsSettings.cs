using TMPEffects.Databases;
using UnityEngine;
using UnityEditor;
using TMPEffects.Databases.AnimationDatabase;
using TMPEffects.Databases.CommandDatabase;

namespace TMPEffects
{
    // TODO REMOVE!
    [CreateAssetMenu(fileName = "TMPEffectsSettings", menuName = "TMPEffects Settings")]
    internal sealed class TMPEffectsSettings : ScriptableObject
    {
        public TMPKeywordDatabase GlobalKeywordDatabase => globalKeywordDatabase;

        [SerializeField] private TMPKeywordDatabase globalKeywordDatabase;
        [SerializeField] private char animationTagPrefix = '\0';
        [SerializeField] private char showAnimationTagPrefix = '+';
        [SerializeField] private char hideAnimationTagPrefix = '-';
        [SerializeField] private char commandTagPrefix = '!';
        [SerializeField] private char eventTagPrefix = '?';
      
        internal static TMPEffectsSettings Get()
        {
            var settings = Resources.Load<TMPEffectsSettings>($"TMPEffectsSettings_Project"); 
            // TODO This should be "TMPEffectsSettings"; In TMPEffectsPreferences it should be "TMPEffectsPreferences"
            // TODO Fix that and update the resource package accordingly

            return settings;
        }
    }
}