using System.Collections.Generic;
using System.Diagnostics;
using TMPEffects.Components;
using TMPEffects.Components.Animator;
using TMPEffects.Components.CharacterData;
using UnityEngine.Windows.Speech;

namespace TMPEffects.TMPAnimations
{
    /// <summary>
    /// Basic interface for animation contexts.
    /// </summary>
    public interface IAnimationContext
    {
        /// <summary>
        /// The context of the animator.
        /// </summary>
        public ReadOnlyAnimatorContext animatorContext { get; }
        /// <summary>
        /// Data about the animation segment.
        /// </summary>
        public SegmentData segmentData { get; }

        public object customData { get; }

        public TMPAnimator.CharDataState state { get; }

        public float AnimationTime { get; }
        public float AnimationTimePassed { get; }   

        public bool Finished(int index);
        public bool Finished(CharData cData);
        public void FinishAnimation(int index);
        public void FinishAnimation(CharData cData);

    }

    public class AnimationContext : IAnimationContext
    {
        public bool Finished(int index) => finishedDict[index];
        public bool Finished(CharData cData) => finishedDict[cData.info.index];
        public ReadOnlyAnimatorContext animatorContext { get; set; }
        public SegmentData segmentData { get; set; }
        public object customData { get; }
        public TMPAnimator.CharDataState state { get; }
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

        public void FinishAnimation(int index)
        {
            finishedDict[index] = true;
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
        public bool Finished(int index) => context.Finished(index);
        public bool Finished(CharData cData) => context.Finished(cData);
        public ReadOnlyAnimatorContext animatorContext => context.animatorContext;
        public SegmentData segmentData => context.segmentData;
        public object customData => context.customData;
        public TMPAnimator.CharDataState state => context.state;

        public float AnimationTime=> context.AnimationTime;
        public float AnimationTimePassed => context.AnimationTimePassed;

        public ReadOnlyAnimationContext(AnimationContext context)
        {
            this.context = context;
        }

        public void FinishAnimation(int index) => context.FinishAnimation(index);
        public void FinishAnimation(CharData cData) => context.FinishAnimation(cData);

        private AnimationContext context;
    }
}
