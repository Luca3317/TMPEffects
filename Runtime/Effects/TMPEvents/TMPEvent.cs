using TMPEffects.Components;
using UnityEngine.Events;

namespace TMPEffects.TMPEvents
{
    /// <summary>
    /// Class used by <see cref="TMPWriter"/> to raise event tags.
    /// </summary>
    [System.Serializable]
    public class TMPEvent : UnityEvent<TMPEventArgs>
    {
        public TMPEvent() : base() { }
    }
}
