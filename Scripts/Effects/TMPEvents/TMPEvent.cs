using TMPEffects.Components;
using UnityEngine.Events;

namespace TMPEffects.TMPEvents
{
    [System.Serializable]
    public class TMPEvent : UnityEvent<TMPEventArgs>
    {
        public TMPEvent() : base() { }
    }
}
