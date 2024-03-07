using System.Collections.Generic;
using TMPEffects.Components.Animator;
using TMPEffects.Components.CharacterData;
using TMPEffects.Extensions;
using UnityEngine;
using static TMPEffects.ParameterUtility;

namespace TMPEffects.TMPAnimations.HideAnimations
{
    [CreateAssetMenu(fileName = "new MoveInHideAnimation", menuName = "TMPEffects/Hide Animations/MoveIn")]
    public class MoveInHideAnimation : TMPHideAnimation
    {
        [SerializeField] float duration = 0.55f;
        [SerializeField] Vector3 targetPosition = new Vector3(0, 1250, 0);
        [SerializeField] bool targetIsDelta = true;
        [SerializeField] AnimationCurve curve = AnimationCurveUtility.EaseInBack();

        public override void Animate(CharData cData, IAnimationContext context)
        {
            ReadOnlyAnimatorContext ac = context.animatorContext;
            Data d = context.customData as Data;

            float t = d.duration > 0 ? Mathf.Clamp01((ac.PassedTime - ac.StateTime(cData)) / d.duration) : 1f;
            float t2 = d.curve.Evaluate(t);

            Vector3 pos = Vector3.LerpUnclamped(cData.info.initialPosition, d.targetIsDelta ? cData.info.initialPosition + d.targetPosition : d.targetPosition, t2);
            cData.SetPosition(pos);

            if (t == 1) context.FinishAnimation(cData);
        }

        public override void SetParameters(object customData, IDictionary<string, string> parameters)
        {
            Data d = customData as Data;
            if (TryGetFloatParameter(out float duration, parameters, "duration", "dur", "d")) d.duration = duration;
            if (TryGetBoolParameter(out bool b, parameters, "targetIsDelta", "isDelta", "delta")) d.targetIsDelta = b;
            if (TryGetVector3Parameter(out Vector3 v3, parameters, "targetPosition", "targetPos", "target")) d.targetPosition = v3;
            if (TryGetAnimCurveParameter(out AnimationCurve curve, parameters, "curve", "crv", "c")) d.curve = curve;
        }

        public override bool ValidateParameters(IDictionary<string, string> parameters)
        {
            if (parameters == null) return true;
            if (HasNonFloatParameter(parameters, "duration", "d", "dur")) return false;
            if (HasNonAnimCurveParameter(parameters, "curve", "c", "crv")) return false;
            if (HasNonBoolParameter(parameters, "targetIsDelta", "isDelta", "delta")) return false;
            if (HasNonVector3Parameter(parameters, "targetPosition", "targetPos", "target")) return false;
            return true;
        }

        public override object GetNewCustomData()
        {
            return new Data { curve = this.curve, duration = this.duration, targetIsDelta = this.targetIsDelta, targetPosition = this.targetPosition };
        }

        private class Data
        {
            public AnimationCurve curve;
            public float duration;

            public Vector3 targetPosition = Vector3.one;
            public bool targetIsDelta = true;
        }
    }
}