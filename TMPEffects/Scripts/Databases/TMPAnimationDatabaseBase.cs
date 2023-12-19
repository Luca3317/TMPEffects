namespace TMPEffects.Databases
{
    /// <summary>
    /// Base class for databases storing <see cref="TMPAnimation"/>.
    /// </summary>
    public abstract class TMPAnimationDatabaseBase<T> : TMPEffectDatabase<T> where T : ITMPAnimation
    {

    }
}