using System.Collections.Generic;
using TMPEffects.Components.Animator;
using TMPEffects.CharacterData;
using System.Diagnostics;

namespace TMPEffects.TMPAnimations
{
    /// <summary>
    /// Basic interface for animation contexts.
    /// </summary>
    public interface IAnimationContext
    {
        /// <summary>
        /// The context of the animating TMPAnimator.
        /// </summary>
        public IAnimatorContext AnimatorContext { get; }
        /// <summary>
        /// Data about the animation segment.
        /// </summary>
        public SegmentData SegmentData { get; }

        /// <summary>
        /// The custom data object.
        /// </summary>
        public object CustomData { get; }

        /// <summary>
        /// The current state of the CharData, with the previous animations applied.
        /// </summary>
        public ReadOnlyCharDataState State { get; }

        /// <summary>
        /// Check if the animation is considered finished for the character at the given index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool Finished(int index);
        /// <summary>
        /// Check if the animation is considered finished for the given character.
        /// </summary>
        /// <param name="cData"></param>
        /// <returns></returns>
        public bool Finished(CharData cData);
        /// <summary>
        /// Set the animation to be considered finished for the given character.
        /// </summary>
        /// <param name="cData"></param>
        /// <returns></returns>
        public void FinishAnimation(CharData cData);

    }

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
        public SegmentData SegmentData { get; set; }
        /// <inheritdoc/>
        public object CustomData { get; }
        /// <inheritdoc/>
        public ReadOnlyCharDataState State { get; }

        public AnimationContext(ReadOnlyAnimatorContext animatorContext, ReadOnlyCharDataState state, SegmentData segmentData, object customData)
        {
            this.CustomData = customData;
            this.State = state;
            this.SegmentData = segmentData;
            this.AnimatorContext = animatorContext;
            finishedDict = new Dictionary<int, bool>(segmentData.effectiveLength);

            for (int i = segmentData.firstAnimationIndex; i < segmentData.firstAnimationIndex + segmentData.effectiveLength; i++)
            {
                finishedDict.Add(i, false);
            }
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

    public class ReadOnlyAnimationContext : IAnimationContext
    {
        /// <inheritdoc/>
        public bool Finished(int index) => context.Finished(index);
        /// <inheritdoc/>
        public bool Finished(CharData cData) => context.Finished(cData);
        /// <inheritdoc/>
        public IAnimatorContext AnimatorContext => context.AnimatorContext;
        /// <inheritdoc/>
        public SegmentData SegmentData => context.SegmentData;
        /// <inheritdoc/>
        public object CustomData => context.CustomData;
        /// <inheritdoc/>
        public ReadOnlyCharDataState State => context.State;

        public ReadOnlyAnimationContext(AnimationContext context)
        {
            this.context = context;
        }

        /// <inheritdoc/>
        public void FinishAnimation(CharData cData) => context.FinishAnimation(cData);

        private AnimationContext context;
    }
}
