using UnityEngine;

namespace TMPEffects.Databases
{
    /// <summary>
    /// Base class for all databases.
    /// </summary>
    /// <typeparam name="T">The type of effect stored in this database.</typeparam>
    public abstract class TMPEffectDatabase<T> : ScriptableObject, ITMPEffectDatabase<T>
    {
        public abstract bool Contains(string name);
        public abstract T GetEffect(string name);
    }
}