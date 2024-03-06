using System.Collections.Generic;
using TMPEffects.Components.Animator;
using TMPEffects.Components.CharacterData;
using TMPEffects.Extensions;
using UnityEngine;
using static TMPEffects.ParameterUtility;

namespace TMPEffects.TMPAnimations.HideAnimations
{
    [CreateAssetMenu(fileName = "new GrowHideAnimation", menuName = "TMPEffects/Hide Animations/Grow")]
    public class GrowHideAnimation : TMPHideAnimation
    {
        [SerializeField] float duration = 0.15f;
        [SerializeField] Vector3 targetScale = Vector3.one * 2;
        [SerializeField] AnimationCurve curve = AnimationCurveUtility.EaseInExpo();

        public override void Animate(CharData cData, IAnimationContext context)
        {
            ReadOnlyAnimatorContext ac = context.animatorContext;
            Data d = context.customData as Data;

            float t = d.duration > 0 ? Mathf.Clamp01((ac.PassedTime - ac.StateTime(cData)) / d.duration) : 1f;
            float t2 = d.curve.Evaluate(t);

            Vector3 scale = Vector3.LerpUnclamped(cData.info.initialScale, targetScale, t2);
            cData.SetScale(scale);

            if (t == 1)
            {
                Debug.Log("DONE!");
                context.FinishAnimation(cData);
            }
        }

        public override void SetParameters(object customData, IDictionary<string, string> parameters)
        {
            Data d = customData as Data;
            if (TryGetFloatParameter(out float duration, parameters, "duration", "dur", "d")) d.duration = duration;
            if (TryGetAnimCurveParameter(out AnimationCurve curve, parameters, "curve", "crv", "c")) d.curve = curve;
            if (TryGetVector3Parameter(out Vector3 v3, parameters, "targetScale", "scale", "target")) d.targetScale = v3;
        }

        public override bool ValidateParameters(IDictionary<string, string> parameters)
        {
            if (parameters == null) return true;
            if (HasNonFloatParameter(parameters, "duration", "d", "dur")) return false;
            if (HasNonAnimCurveParameter(parameters, "curve", "c", "crv")) return false;
            if (HasNonVector3Parameter(parameters, "targetScale", "scale", "target")) return false;
            return true;
        }

        public override object GetNewCustomData()
        {
            return new Data { curve = this.curve, duration = this.duration, targetScale = this.targetScale };
        }

        private class Data
        {
            public AnimationCurve curve;
            public float duration;
            public Vector3 targetScale;
        }
    }
}