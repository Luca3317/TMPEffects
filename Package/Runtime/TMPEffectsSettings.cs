
using TMPEffects.Databases;
using TMPEffects.Databases.AnimationDatabase;
using TMPEffects.Databases.CommandDatabase;
using TMPEffects.Editor;
using UnityEngine;

[System.Serializable]
[ExcludeFromPreset]
[CreateAssetMenu(fileName = "TMPEffects Settings", menuName = "CREATE SETTINGS")]
public class TMPEffectsSettings : ScriptableObject
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
                    // TODO Move the ImporterWindow into non-editor assembly
                    // and wrap that in a preprocessor directive
                    TMPEffectsImporterWindow.ShowImporterWindow();
                }
#endif
            }

            return TMPEffectsSettings.instance;
        }
    }
    
    
    public static TMPAnimationDatabase DefaultAnimationDatabase => Instance.defaultAnimationDatabase;
    public static TMPCommandDatabase DefaultCommandDatabase => Instance.defaultCommandDatabase;
    
    // TODO Remove those tooltips, for thte ones in the editor
    [SerializeField, Tooltip("The default animation database that TMPAnimator component will use.")]
    private TMPAnimationDatabase defaultAnimationDatabase;
    [SerializeField, Tooltip("The default command database that TMPWriter components will use.")]
    private TMPCommandDatabase defaultCommandDatabase;
    
    public static TMPKeywordDatabase DefaultKeywordDatabase => Instance.defaultKeywordDatabase;
    
    [SerializeField, Tooltip("The default keyword database that TMPEffects components will use.")]
    private TMPKeywordDatabase defaultKeywordDatabase;

    public static TMPKeywordDatabase GlobalKeywordDatabase => Instance.globalKeywordDatabase;
    
    [SerializeField, Tooltip("The keyword database that defines globally valid keywords.")]
    private TMPKeywordDatabase globalKeywordDatabase;

    public static char AnimationPrefix => Instance.animationPrefix;
    public static char ShowAnimationPrefix => Instance.showAnimationPrefix;
    public static char HideAnimationPrefix => Instance.hideAnimationPrefix;
    
    // TODO Need to enforce these are valid according to rules in parsingutility
    [SerializeField, Tooltip("The prefix for animation tags.")]
    private char animationPrefix;
    [SerializeField, Tooltip("The prefix for show animation tags.")]
    private char showAnimationPrefix;
    [SerializeField, Tooltip("The prefix for hide animation tags.")]
    private char hideAnimationPrefix;
    
    public static char CommandPrefix => Instance.commandPrefix;
    public static char EventPrefix => Instance.eventPrefix;
    
    [SerializeField, Tooltip("The prefix for command tags.")]
    private char commandPrefix;
    [SerializeField, Tooltip("The prefix for event tags.")]
    private char eventPrefix;
}