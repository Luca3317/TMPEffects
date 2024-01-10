using UnityEngine.Events;
using TMPEffects.Tags;

namespace TMPEffects
{
    [System.Serializable]
    public class TMPEvent : UnityEvent<TMPEventTag>
    {
        public TMPEvent() : base() { }
    }
}