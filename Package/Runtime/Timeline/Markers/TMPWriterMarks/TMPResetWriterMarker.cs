using System.ComponentModel;
using UnityEngine;
using UnityEngine.Timeline;

namespace TMPEffects.Timeline.Markers
{
    [CustomStyle("TMPResetWriterMarkerStyle")]
    [DisplayName("TMPEffects Marker/TMPWriter/Reset writer")]
    public class TMPResetWriterMarker : TMPEffectsMarker
    {
        public override PropertyName id => new PropertyName();

        public override NotificationFlags flags =>
            (retroactive ? NotificationFlags.Retroactive : default) |
            (triggerOnce ? NotificationFlags.TriggerOnce : default) |
            (triggerInEditMode ? NotificationFlags.TriggerInEditMode : default);

        [Space] [Tooltip("What text index to reset the TMPWriter to.")]
        [SerializeField] private int textIndex;
    
        public int TextIndex => textIndex;

        private void OnValidate()
        {
            if (textIndex < 0) textIndex = 0; 
        }
    }
}