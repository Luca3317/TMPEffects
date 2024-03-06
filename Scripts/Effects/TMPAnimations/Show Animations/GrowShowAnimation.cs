using System.Collections.Generic;
using TMPEffects.Components.Animator;
using TMPEffects.Components.CharacterData;
using TMPEffects.Extensions;
using UnityEngine;
using static TMPEffects.ParameterUtility;

namespace TMPEffects.TMPAnimations.ShowAnimations
{
    [CreateAssetMenu(fileName = "new GrowShowAnimation", menuName = "TMPEffects/Show Animations/Grow")]
    public class GrowShowAnimation : TMPShowAnimation
    {
        [SerializeField] float duration = 0.15f;
        [SerializeField] Vector3 startScale = Vector3.one * 2;
        [SerializeField] AnimationCurve curve = AnimationCurveUtility.EaseOutSine();

        public override void Animate(CharData cData, IAnimationContext context)
        {
            ReadOnlyAnimatorContext ac = context.animatorContext;
            Data d = context.customData as Data;

            float t = d.duration > 0 ? Mathf.Clamp01((ac.PassedTime - ac.StateTime(cData)) / d.duration) : 1f;
            float t2 = d.curve.Evaluate(t);

            Vector3 scale = Vector3.LerpUnclamped(d.startScale, cData.info.initialScale, t2);
            cData.SetScale(scale);

            if (t == 1) context.FinishAnimation(cData);
        }

        public override void SetParameters(object customData, IDictionary<string, string> parameters)
        {
            Data d = customData as Data;
            if (TryGetFloatParameter(out float duration, parameters, "duration", "dur", "d")) d.duration = duration;
            if (TryGetAnimCurveParameter(out AnimationCurve curve, parameters, "curve", "crv", "c")) d.curve = curve;
            if (TryGetVector3Parameter(out Vector3 v3, parameters, "startScale", "scale", "start")) d.startScale = v3;
        }

        public override bool ValidateParameters(IDictionary<string, string> parameters)
        {
            if (parameters == null) return true;
            if (HasNonFloatParameter(parameters, "duration", "d", "dur")) return false;
            if (HasNonAnimCurveParameter(parameters, "curve", "c", "crv")) return false;
            if (HasNonVector3Parameter(parameters, "startScale", "scale", "start")) return false;
            return true;
        }

        public override object GetNewCustomData()
        {
            return new Data { curve = this.curve, duration = this.duration, startScale = this.startScale };
        }

        private class Data
        {
            public AnimationCurve curve;
            public float duration;
            public Vector3 startScale;
        }
    }
}