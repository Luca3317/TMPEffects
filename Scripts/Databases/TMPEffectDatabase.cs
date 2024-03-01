using System.ComponentModel;
using TMPEffects.ObjectChanged;
using UnityEngine;

namespace TMPEffects.Databases
{
    /// <summary>
    /// Base class for all databases.
    /// </summary>
    /// <typeparam name="T">The type of effect stored in this database.</typeparam>
    public abstract class TMPEffectDatabase<T> : ScriptableObject, ITMPEffectDatabase<T>, INotifyObjectChanged
    {
        public event ObjectChangedEventHandler ObjectChanged;

        public void ListenForChanges(ObjectChangedEventHandler handler)
        {
            Debug.Log("Started to listen on " + name);
            ObjectChanged -= handler;
            ObjectChanged += handler;
        }
        public void StopListenForChanges(ObjectChangedEventHandler handler)
        {
            Debug.Log("stopped to listen on " + name);
            ObjectChanged -= handler;
        }

        /// <summary>
        /// Check whether this database contains an effect associated with the given name.
        /// </summary>
        /// <param name="name">The identifier of the effect.</param>
        /// <returns>true if this database contains an effect associated with the given name; false otherwise.</returns>
        public abstract bool ContainsEffect(string name);

        /// <summary>
        /// Get the effect associated with the given name.
        /// </summary>
        /// <param name="name">The identifier of the effect.</param>
        /// <returns>The effect associated with the given name.</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException"></exception>
        public abstract T GetEffect(string name);

        protected virtual void OnValidate()
        {
            RaiseDatabaseChanged();
        }

        protected void RaiseDatabaseChanged()
        {
            Debug.Log("Raising " + name + " propertychange null = " + (ObjectChanged == null));
            ObjectChanged?.Invoke(this);
        }
    }
}