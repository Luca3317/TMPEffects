using System.Collections.Generic;
using TMPEffects.Components.Animator;
using TMPEffects.CharacterData;
using TMPEffects.Extensions;
using UnityEngine;
using static TMPEffects.ParameterUtility;
using static TMPEffects.TMPAnimations.AnimationUtility;

namespace TMPEffects.TMPAnimations.HideAnimations
{
    [CreateAssetMenu(fileName = "new MoveInHideAnimation", menuName = "TMPEffects/Hide Animations/MoveIn")]
    public class MoveInHideAnimation : TMPHideAnimation
    {
        [Tooltip("How long the animation will take to fully hide the character.\nAliases: duration, dur, d")]
        [SerializeField] float duration = 1f;
        [Tooltip("The curve used for getting the t-value to interpolate between the start and target position.\nAliases: curve, crv, c")]
        [SerializeField] AnimationCurve curve = AnimationCurveUtility.EaseInBack();
        [Tooltip("The postion to move the character to.\nAliases: targetposition, targetpos, target")]
        [SerializeField] TypedVector3 targetPosition = new TypedVector3(VectorType.Offset, new Vector3(0, 1250, 0));

        public override void Animate(CharData cData, IAnimationContext context)
        {
            IAnimatorContext ac = context.AnimatorContext;
            Data d = context.CustomData as Data;

            float t = d.duration > 0 ? Mathf.Clamp01((ac.PassedTime - ac.StateTime(cData)) / d.duration) : 1f;
            float t2 = d.curve.Evaluate(t);

            Vector3 targetPos;

            switch (targetPosition.type)
            {
                case VectorType.Position: targetPos = targetPosition.vector; break;
                case VectorType.Anchor: targetPos = AnchorToPosition(targetPosition.vector, cData); break;
                case VectorType.Offset: targetPos = cData.InitialPosition + targetPosition.vector; break;

                default: throw new System.NotImplementedException(nameof(targetPosition.type));
            }

            Vector3 pos = Vector3.LerpUnclamped(cData.InitialPosition, targetPos, t2);

            cData.SetPosition(pos);

            if (t == 1) context.FinishAnimation(cData);
        }

        public override void SetParameters(object customData, IDictionary<string, string> parameters)
        {
            Data d = customData as Data;
            if (TryGetFloatParameter(out float duration, parameters, "duration", "dur", "d")) d.duration = duration;
            if (TryGetTypedVector3Parameter(out var tv3, parameters, "targetposition", "targetpos", "target")) d.targetPosition = tv3;
            if (TryGetAnimCurveParameter(out AnimationCurve curve, parameters, "curve", "crv", "c")) d.curve = curve;
        }

        public override bool ValidateParameters(IDictionary<string, string> parameters)
        {
            if (parameters == null) return true;
            if (HasNonFloatParameter(parameters, "duration", "d", "dur")) return false;
            if (HasNonAnimCurveParameter(parameters, "curve", "c", "crv")) return false;
            if (HasNonTypedVector3Parameter(parameters, "targetposition", "targetpos", "target")) return false;
            return true;
        }

        public override object GetNewCustomData()
        {
            return new Data { curve = this.curve, duration = this.duration, targetPosition = this.targetPosition };
        }


        private class Data
        {
            public AnimationCurve curve;
            public float duration;
            public TypedVector3 targetPosition;
        }
    }
}