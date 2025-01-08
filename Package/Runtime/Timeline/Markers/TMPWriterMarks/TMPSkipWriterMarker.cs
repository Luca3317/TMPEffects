using System.ComponentModel;
using UnityEngine;
using UnityEngine.Timeline;

namespace TMPEffects.Timeline.Markers
{
    [CustomStyle("TMPSkipWriterMarkerStyle")]
    [DisplayName("TMPEffects Marker/TMPWriter/Skip writer")]
    public class TMPSkipWriterMarker : TMPEffectsMarker
    {
        public override PropertyName id => new PropertyName();

        public override NotificationFlags flags =>
            (retroactive ? NotificationFlags.Retroactive : default) |
            (triggerOnce ? NotificationFlags.TriggerOnce : default) |
            (triggerInEditMode ? NotificationFlags.TriggerInEditMode : default);

        [Space] [Tooltip("Whether to skip the show animations (if any) when skipping the current text.")] [SerializeField]
        private bool skipShowAnimation = false;

        public bool SkipShowAnimation => skipShowAnimation;
    }
}