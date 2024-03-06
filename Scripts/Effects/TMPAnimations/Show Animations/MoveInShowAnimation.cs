using System.Collections.Generic;
using TMPEffects.Components.Animator;
using TMPEffects.Components.CharacterData;
using TMPEffects.Extensions;
using UnityEngine;
using static TMPEffects.ParameterUtility;

namespace TMPEffects.TMPAnimations.ShowAnimations
{
    [CreateAssetMenu(fileName = "new MoveInShowAnimation", menuName = "TMPEffects/Show Animations/MoveIn")]
    public class MoveInShowAnimation : TMPShowAnimation
    {
        [SerializeField] float duration = 0.15f;
        [SerializeField] Vector3 startPosition = new Vector3(10, 10, 0);
        [SerializeField] bool startIsDelta = true;
        [SerializeField] AnimationCurve curve = AnimationCurveUtility.EaseOutElastic();

        public override void Animate(CharData cData, IAnimationContext context)
        {
            ReadOnlyAnimatorContext ac = context.animatorContext;
            Data d = context.customData as Data;

            float t = d.duration > 0 ? Mathf.Clamp01((ac.PassedTime - ac.StateTime(cData)) / d.duration) : 1f;
            float t2 = d.curve.Evaluate(t);

            Vector3 pos = Vector3.LerpUnclamped(d.startIsDelta ? cData.info.initialPosition + d.startPosition : d.startPosition, cData.info.initialPosition, t2);
            cData.SetPosition(pos);

            if (t == 1) context.FinishAnimation(cData);
        }

        public override void SetParameters(object customData, IDictionary<string, string> parameters)
        {
            Data d = customData as Data;
            if (TryGetFloatParameter(out float duration, parameters, "duration", "dur", "d")) d.duration = duration;
            if (TryGetBoolParameter(out bool b, parameters, "startIsDelta", "isDelta", "delta")) d.startIsDelta = b;
            if (TryGetVector3Parameter(out Vector3 v3, parameters, "startPosition", "startPos", "start")) d.startPosition = v3;
            if (TryGetAnimCurveParameter(out AnimationCurve curve, parameters, "curve", "crv", "c")) d.curve = curve;
        }

        public override bool ValidateParameters(IDictionary<string, string> parameters)
        {
            if (parameters == null) return true;
            if (HasNonFloatParameter(parameters, "duration", "d", "dur")) return false;
            if (HasNonAnimCurveParameter(parameters, "curve", "c", "crv")) return false;
            if (HasNonBoolParameter(parameters, "startIsDelta", "isDelta", "delta")) return false;
            if (HasNonVector3Parameter(parameters, "startPosition", "startPos", "start")) return false;
            return true;
        }

        public override object GetNewCustomData()
        {
            return new Data { curve = this.curve, duration = this.duration, startIsDelta = this.startIsDelta, startPosition = this.startPosition };
        }


        private class Data
        {
            public AnimationCurve curve;
            public float duration;

            public Vector3 startPosition = Vector3.one;
            public bool startIsDelta = true;
        }
    }
}