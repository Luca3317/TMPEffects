using System.Collections.Generic;
using TMPEffects.AutoParameters.Attributes;
using TMPEffects.Components.Animator;
using TMPEffects.CharacterData;
using TMPEffects.Extensions;
using UnityEngine;
using static TMPEffects.Parameters.TMPParameterUtility;

namespace TMPEffects.TMPAnimations.ShowAnimations
{
    [AutoParameters]
    [CreateAssetMenu(fileName = "new GrowShowAnimation",
        menuName = "TMPEffects/Animations/Show Animations/Built-in/Grow")]
    public partial class GrowShowAnimation : TMPShowAnimation
    {
        [SerializeField, AutoParameter("duration", "dur", "d")]
        [Tooltip("How long the animation will take to fully show the character.\nAliases: duration, dur, d")]
        float duration = 0.15f;

        [SerializeField, AutoParameter("curve", "crv", "c")]
        [Tooltip("The curve used for getting the t-value to interpolate between the scales.\nAliases: curve, crv, c")]
        AnimationCurve curve = AnimationCurveUtility.EaseOutSine();

        [SerializeField, AutoParameter("startscale", "startscl", "start")]
        [Tooltip("The scale to start growing to the initial scale from.\nAliases: startscale, startscl, start")]
        Vector3 startScale = Vector3.one * 2;

        private partial void Animate(CharData cData, AutoParametersData d, IAnimationContext context)
        {
            IAnimatorContext ac = context.AnimatorContext;

            float t = d.duration > 0 ? Mathf.Clamp01((ac.PassedTime - ac.StateTime(cData)) / d.duration) : 1f;
            float t2 = d.curve.Evaluate(t);

            Vector3 scale = Vector3.LerpUnclamped(d.startScale, cData.InitialScale, t2);
            cData.SetScale(scale);

            if (t == 1) context.FinishAnimation(cData);
        }
    }
}