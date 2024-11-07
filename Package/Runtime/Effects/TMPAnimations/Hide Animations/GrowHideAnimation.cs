using TMPEffects.AutoParameters.Attributes;
using TMPEffects.Components.Animator;
using TMPEffects.CharacterData;
using TMPEffects.Extensions;
using UnityEngine;

namespace TMPEffects.TMPAnimations.HideAnimations
{
    [AutoParameters]
    [CreateAssetMenu(fileName = "new GrowHideAnimation", menuName = "TMPEffects/Animations/Hide Animations/Built-in/Grow")]
    public partial class GrowHideAnimation : TMPHideAnimation
    {
        [SerializeField, AutoParameter("duration", "dur", "d")] 
        [Tooltip("How long the animation will take to fully show the character.\nAliases: duration, dur, d")]
        float duration = 0.15f;
        
        [SerializeField, AutoParameter("curve", "crv", "c")] 
        [Tooltip("The curve used for getting the t-value to interpolate between the scales.\nAliases: curve, crv, c")]
        AnimationCurve curve = AnimationCurveUtility.EaseOutSine();
        
        [SerializeField, AutoParameter("targetscale", "targetscl", "target")] 
        [Tooltip("The scale to grow to from the initial scale.\nAliases: targetscale, targetscl, target")]
        Vector3 targetScale = Vector3.one * 2;

        private partial void Animate(CharData cData, AutoParametersData d, IAnimationContext context)
        {
            IAnimatorContext ac = context.AnimatorContext;

            float t = d.duration > 0 ? Mathf.Clamp01((ac.PassedTime - ac.StateTime(cData)) / d.duration) : 1f;
            float t2 = d.curve.Evaluate(t);

            Vector3 scale = Vector3.LerpUnclamped(cData.InitialScale, d.targetScale, t2);
            cData.SetScale(scale);

            if (t >= 1) context.FinishAnimation(cData);
        }
    }
}