using System.Collections.Generic;
using TMPEffects.Components;
using TMPEffects.Components.Animator;
using TMPEffects.CharacterData;

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
        public ReadOnlyAnimatorContext animatorContext { get; }
        /// <summary>
        /// Data about the animation segment.
        /// </summary>
        public SegmentData segmentData { get; }

        /// <summary>
        /// The custom data object.
        /// </summary>
        public object customData { get; }

        /// <summary>
        /// The current state of the CharData, with the previous animations applied.
        /// </summary>
        public TMPAnimator.CharDataState state { get; }

        /// <summary>
        /// The time of when the animation started playing.
        /// </summary>
        public float AnimationTime { get; }
        /// <summary>
        /// The time that has passed since the animation started playing.
        /// </summary>
        public float AnimationTimePassed { get; }

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
        public ReadOnlyAnimatorContext animatorContext { get; set; }
        /// <inheritdoc/>
        public SegmentData segmentData { get; set; }
        /// <inheritdoc/>
        public object customData { get; }
        /// <inheritdoc/>
        public TMPAnimator.CharDataState state { get; }
        /// <inheritdoc/>
        public float AnimationTime
        {
            get
            {
                return aaa;
            }
            set
            {
                aaa = value;
            }
        }
        private float aaa;
        /// <inheritdoc/>
        public float AnimationTimePassed => animatorContext.PassedTime - AnimationTime;

        public AnimationContext(ReadOnlyAnimatorContext animatorContext, TMPAnimator.CharDataState state, SegmentData segmentData, object customData)
        {
            this.customData = customData;
            this.state = state;
            this.segmentData = segmentData;
            this.animatorContext = animatorContext;
            AnimationTime = -1;
            finishedDict = new Dictionary<int, bool>(segmentData.length);

            for (int i = segmentData.startIndex; i < segmentData.startIndex + segmentData.length; i++)
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
        public ReadOnlyAnimatorContext animatorContext => context.animatorContext;
        /// <inheritdoc/>
        public SegmentData segmentData => context.segmentData;
        /// <inheritdoc/>
        public object customData => context.customData;
        /// <inheritdoc/>
        public TMPAnimator.CharDataState state => context.state;
        /// <inheritdoc/>
        public float AnimationTime => context.AnimationTime;
        /// <inheritdoc/>
        public float AnimationTimePassed => context.AnimationTimePassed;

        public ReadOnlyAnimationContext(AnimationContext context)
        {
            this.context = context;
        }

        /// <inheritdoc/>
        public void FinishAnimation(CharData cData) => context.FinishAnimation(cData);

        private AnimationContext context;
    }
}
