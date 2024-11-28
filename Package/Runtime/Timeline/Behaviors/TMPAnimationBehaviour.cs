using System;
using TMPEffects.CharacterData;
using TMPEffects.Components;
using TMPEffects.Components.Animator;
using TMPEffects.Tags;
using TMPEffects.TMPAnimations;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class TMPAnimationBehaviour : PlayableBehaviour
{
    public GenericAnimation animation;
    private MockedAnimationContext animContext;

    // public TMPEffectTagIndices indices;
    [NonSerialized] public TimelineClip Clip = null;

    private TMPAnimator animator;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        animator = playerData as TMPAnimator;
        if (animator == null) return;

        animator.OnCharacterAnimated -= Animate;
        if (info.effectiveWeight == 0)
        {
            // animator.QueueCharacterReset();
            return;
        }

        animator.OnCharacterAnimated += Animate;
    }

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        animator.OnCharacterAnimated -= Animate;
        // animator.QueueCharacterReset();
    }

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        if (animator == null) return;
        animContext = new MockedAnimationContext(animator.AnimatorContext, animation.GetNewCustomData());
    }

    private void Animate(CharData cData)
    {
        if (animator == null) return;
        animContext ??= new MockedAnimationContext(animator.AnimatorContext, animation.GetNewCustomData());
        animation.Animate(cData, animContext);
    }

    private class MockedAnimationContext : IAnimationContext
    {
        public bool Finished(int index)
        {
            return false;
        }

        public bool Finished(CharData cData)
        {
            return false;
        }

        public void FinishAnimation(CharData cData)
        {
        }

        public IAnimatorContext AnimatorContext { get; set; }

        public SegmentData SegmentData { get; set; }

        public object CustomData { get; set; }

        public MockedAnimationContext(IAnimatorContext context, object customData)
        {
            AnimatorContext = context;
            CustomData = customData;

            TMPEffectTagIndices inds =
                new TMPEffectTagIndices(0, context.Animator.TextComponent.GetParsedText().Length, 0);
            SegmentData = new SegmentData(inds, context.Animator.CharData, (cd) => true);
        }
    }
}