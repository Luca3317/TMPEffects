using UnityEngine.Events;
using TMPEffects.Tags;

namespace TMPEffects
{
    [System.Serializable]
    public class TMPEvent : UnityEvent<TMPEventArgs>
    {
        public TMPEvent() : base() { }
    }
}