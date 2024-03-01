using System.ComponentModel;

namespace TMPEffects.Databases
{
    public interface ITMPEffectDatabase
    {
        /// <summary>
        /// Check whether this database contains an effect associated with the given name.
        /// </summary>
        /// <param name="name">The identifier of the effect.</param>
        /// <returns>true if this database contains an effect associated with the given name; false otherwise.</returns>
        public bool ContainsEffect(string name);
    }

    public interface ITMPEffectDatabase<out T> : ITMPEffectDatabase
    {
        /// <summary>
        /// Get the effect associated with the given name.
        /// </summary>
        /// <param name="name">The identifier of the effect.</param>
        /// <returns>The effect associated with the given name.</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException"></exception>
        public T GetEffect(string name);
    }
}