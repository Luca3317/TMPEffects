using System.Collections.Generic;
using TMPEffects.Components.Animator;
using TMPEffects.CharacterData;

namespace TMPEffects.TMPAnimations
{
    /// <inheritdoc/>
    public class AnimationContext : IAnimationContext
    {
        /// <inheritdoc/>
        public bool Finished(int index) => finishedDict[index];
        /// <inheritdoc/>
        public bool Finished(CharData cData) => finishedDict[cData.info.index];
        /// <inheritdoc/>
        public IAnimatorContext AnimatorContext { get; set; }
        /// <inheritdoc/>
        public SegmentData SegmentData 
        {
            get => segmentData;
            set
            {
                segmentData = value;
                finishedDict = new Dictionary<int, bool>(segmentData.effectiveLength);

                for (int i = segmentData.firstAnimationIndex; i < segmentData.firstAnimationIndex + segmentData.effectiveLength; i++)
                {
                    finishedDict.Add(i, false);
                }
            }
        }
        /// <inheritdoc/>
        public object CustomData { get; }
        /// <inheritdoc/>
        public ICharDataState State { get; }

        private SegmentData segmentData;

        public AnimationContext(ReadOnlyAnimatorContext animatorContext, ReadOnlyCharDataState state, SegmentData segmentData, object customData)
        {
            this.CustomData = customData;
            this.State = state;
            this.AnimatorContext = animatorContext;
            this.SegmentData = segmentData;
        }

        public void ResetFinishAnimation(int index)
        {
            finishedDict[index] = false;
        }

        public void FinishAnimation(CharData cData)
        {
            finishedDict[cData.info.index] = true;
        }

        public void ResetFinishAnimation(CharData cData)
        {
            finishedDict[cData.info.index] = false;
        }

        public void ResetFinishAnimation()
        {
            foreach (var key in finishedDict.Keys)
            {
                finishedDict[key] = false;
            }
        }

        public Dictionary<int, bool> finishedDict;
    }
}
