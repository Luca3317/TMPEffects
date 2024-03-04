using System.Collections;
using System.Collections.Generic;
using TMPEffects.Components.Animator;
using TMPEffects.Components.CharacterData;
using TMPEffects.Extensions;
using UnityEngine;
using static TMPEffects.EffectUtility;

namespace TMPEffects.TMPAnimations.ShowAnimations
{
    [CreateAssetMenu(fileName = "new TestMoveShowAnimation", menuName = "TestMoveShowAnimation")]
    public class TestMoveShowAnimation : TMPShowAnimation
    {
        [SerializeField] float duration;
        [SerializeField] AnimationCurve curve = AnimationCurveUtility.EaseOutElastic();

        public override void Animate(CharData cData, IAnimationContext context)
        {
            ReadOnlyAnimatorContext ac = context.animatorContext;
            Data d = context.customData as Data;

            float t = d.duration > 0 ? Mathf.Clamp01((ac.PassedTime - ac.StateTime(cData)) / d.duration) : 1f;
            float value = d.curve.Evaluate(t);

            if (t == 1) context.FinishAnimation(cData);

            cData.SetPosition(cData.info.initialPosition + Vector3.up * (1 - value) * 200);
        }

        public override void SetParameters(object customData, IDictionary<string, string> parameters)
        {
            Data d = customData as Data;
            if (TryGetFloatParameter(out float duration, parameters, "duration", "dur", "d")) d.duration = duration;
            if (TryGetAnimCurveParameter(out AnimationCurve curve, parameters, "curve", "cur", "c")) d.curve = curve;
        }

        public override bool ValidateParameters(IDictionary<string, string> parameters)
        {
            if (parameters == null) return true;
            if (HasNonFloatParameter(parameters, "duration", "d", "dur")) return false;
            if (HasNonAnimCurveParameter(parameters, "curve", "c", "cur")) return false;
            return true;
        }

        public override object GetNewCustomData()
        {
            return new Data { curve = this.curve, duration = this.duration };
        }


        private class Data
        {
            public AnimationCurve curve;
            public float duration;
        }
    }
}