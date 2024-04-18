using TMPEffects.TMPAnimations;

namespace TMPEffects.Databases.AnimationDatabase
{
    /// <summary>
    /// Base class for databases storing <see cref="ITMPAnimation"/>.
    /// </summary>
    public abstract class TMPAnimationDatabaseBase<T> : TMPEffectDatabase<T> where T : ITMPAnimation
    {

    }
}