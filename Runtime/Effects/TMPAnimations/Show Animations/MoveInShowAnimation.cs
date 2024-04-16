using System.Collections.Generic;
using TMPEffects.Components.Animator;
using TMPEffects.CharacterData;
using TMPEffects.Extensions;
using UnityEngine;
using static TMPEffects.ParameterUtility;
using static TMPEffects.TMPAnimations.AnimationUtility;

namespace TMPEffects.TMPAnimations.ShowAnimations
{
    [CreateAssetMenu(fileName = "new MoveInShowAnimation", menuName = "TMPEffects/Show Animations/MoveIn")]
    public class MoveInShowAnimation : TMPShowAnimation
    {
        [Tooltip("How long the animation will take to fully show the character.\nAliases: duration, dur, d")]
        [SerializeField] float duration = 1f;
        [Tooltip("The curve used for getting the t-value to interpolate between the start and target position.\nAliases: curve, crv, c")]
        [SerializeField] AnimationCurve curve = AnimationCurveUtility.EaseOutElastic();
        [Tooltip("The postion to move the character in from.\nAliases: startposition, startpos, start")]
        [SerializeField] TypedVector3 startPosition = new TypedVector3(VectorType.Offset, Vector3.one * 100);

        public override void Animate(CharData cData, IAnimationContext context)
        {
            IAnimatorContext ac = context.AnimatorContext;
            Data d = context.CustomData as Data;

            float t = d.duration > 0 ? Mathf.Clamp01((ac.PassedTime - ac.StateTime(cData)) / d.duration) : 1f;
            float t2 = d.curve.Evaluate(t);

            Vector3 startPos;

            switch (startPosition.type)
            {
                case VectorType.Position: startPos = startPosition.vector; break;
                case VectorType.Anchor: startPos = AnchorToPosition(startPosition.vector, cData); break;
                case VectorType.Offset: startPos = cData.InitialPosition + startPosition.vector; break;

                default: throw new System.NotImplementedException(nameof(startPosition.type));
            }

            Vector3 pos = Vector3.LerpUnclamped(startPos, cData.InitialPosition, t2);

            cData.SetPosition(pos);

            if (t == 1) context.FinishAnimation(cData);
        }

        public override void SetParameters(object customData, IDictionary<string, string> parameters)
        {
            Data d = customData as Data;
            if (TryGetFloatParameter(out float duration, parameters, "duration", "dur", "d")) d.duration = duration;
            if (TryGetTypedVector3Parameter(out var tv3, parameters, "startposition", "startpos", "start")) d.startPosition = tv3;
            if (TryGetAnimCurveParameter(out AnimationCurve curve, parameters, "curve", "crv", "c")) d.curve = curve;
        }

        public override bool ValidateParameters(IDictionary<string, string> parameters)
        {
            if (parameters == null) return true;
            if (HasNonFloatParameter(parameters, "duration", "d", "dur")) return false;
            if (HasNonAnimCurveParameter(parameters, "curve", "c", "crv")) return false;
            if (HasNonTypedVector3Parameter(parameters, "startposition", "startpos", "start")) return false;
            return true;
        }

        public override object GetNewCustomData()
        {
            return new Data { curve = this.curve, duration = this.duration, startPosition = this.startPosition };
        }


        private class Data
        {
            public AnimationCurve curve;
            public float duration;
            public TypedVector3 startPosition;
        }
    }
}