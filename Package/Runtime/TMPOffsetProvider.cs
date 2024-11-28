using TMPEffects.CharacterData;
using TMPEffects.Components.Animator;
using TMPEffects.TMPAnimations;
using UnityEngine;

namespace TMPEffects.Parameters
{
    public abstract class TMPOffsetProvider : ScriptableObject, ITMPOffsetProvider
    {
        public abstract float GetOffset(CharData cData, ITMPSegmentData segmentData, IAnimatorDataProvider animatorData,
            bool ignoreAnimatorScaling = false);

        public abstract void GetMinMaxOffset(out float min, out float max, ITMPSegmentData segmentData,
            IAnimatorDataProvider animatorData,
            bool ignoreAnimatorScaling = false);
    }
}