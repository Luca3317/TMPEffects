using TMPEffects.Databases;
using TMPEffects.Databases.AnimationDatabase;
using TMPEffects.Databases.CommandDatabase;
using UnityEngine;

#if UNITY_EDITOR
using TMPEffects.Editor;
#endif

[System.Serializable]
[ExcludeFromPreset]
internal class TMPEffectsSettings : ScriptableObject
{
    private static TMPEffectsSettings instance;

    /// <summary>
    /// Get a singleton instance of the settings class.
    /// </summary>
    public static TMPEffectsSettings Instance
    {
        get
        {
            if (TMPEffectsSettings.instance == null)
            {
                TMPEffectsSettings.instance = Resources.Load<TMPEffectsSettings>("TMPEffects Settings");

#if UNITY_EDITOR
                if (TMPEffectsSettings.instance == null)
                {
                    TMPEffectsImporterWindow.ShowImporterWindow();
                }
#else
                if (TMPEffectsSettings.instance == null)
                {
                    Debug.LogError("Could not load TMPEffectsSettings. You must import it and rebuild in order for TMPEffects to work properly.");
                }
#endif
            }

            return TMPEffectsSettings.instance;
        }
    }


    public static TMPAnimationDatabase DefaultAnimationDatabase => Instance?.defaultAnimationDatabase;
    public static TMPCommandDatabase DefaultCommandDatabase => Instance?.defaultCommandDatabase;

    [SerializeField] private TMPAnimationDatabase defaultAnimationDatabase;
    [SerializeField] private TMPCommandDatabase defaultCommandDatabase;

    public static TMPKeywordDatabase DefaultKeywordDatabase => Instance?.defaultKeywordDatabase;

    [SerializeField] private TMPKeywordDatabase defaultKeywordDatabase;

    public static TMPKeywordDatabase GlobalKeywordDatabase => Instance?.globalKeywordDatabase;

    [SerializeField] private TMPKeywordDatabase globalKeywordDatabase;
}