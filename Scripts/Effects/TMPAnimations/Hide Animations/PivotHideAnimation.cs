using System.Collections.Generic;
using UnityEngine;
using TMPEffects.Components.CharacterData;
using static TMPEffects.ParameterUtility;
using static TMPEffects.TMPAnimations.AnimationUtility;
using TMPEffects.Extensions;

namespace TMPEffects.TMPAnimations.Animations
{
    [CreateAssetMenu(fileName = "new PivotHideAnimation", menuName = "TMPEffects/Hide Animations/Pivot")]
    public class PivotHideAnimation : TMPHideAnimation
    {
        [SerializeField] private float duration = 1f;
        [SerializeField] private VectorType pivotType = VectorType.Anchor;
        [SerializeField] private Vector3 pivot = Vector2.zero;
        [SerializeField] private Vector3 startAngle = Vector3.zero;
        [SerializeField] private Vector3 targetAngle = new Vector3(0, 0, 210);
        [SerializeField] private AnimationCurve curve = AnimationCurveUtility.EaseInBack();

        public override void Animate(CharData cData, IAnimationContext context)
        {
            Data d = context.customData as Data;

            float t = Mathf.Lerp(0, 1, (context.animatorContext.PassedTime - context.animatorContext.StateTime(cData)) / d.duration);

            if (t == 1)
            {
                context.FinishAnimation(cData);
            }

            float t2 = d.curve.Evaluate(t);
            Vector3 angle = Vector3.LerpUnclamped(d.startAngle, d.targetAngle, t2);

            cData.SetRotation(Quaternion.Euler(angle));

            switch (d.pivotType)
            {
                case VectorType.Position:
                    cData.SetPivot(d.pivot);
                    break;
                case VectorType.PositionOffset:
                    cData.SetPivot(cData.info.initialPosition + d.pivot);
                    break;
                case VectorType.Anchor:
                    Vector3 position = AnchorToPosition(d.pivot, cData);
                    position.z = 0;
                    SetPivotRaw(position, cData, ref context);
                    break;
            }
        }

        public override void SetParameters(object customData, IDictionary<string, string> parameters)
        {
            if (parameters == null) return;

            Data d = customData as Data;
            if (TryGetFloatParameter(out float f, parameters, "duration", durationAliases)) d.duration = f;
            if (TryGetVector3Parameter(out Vector3 v, parameters, "startAngle", startAngleAngleAliases)) d.startAngle = v;
            if (TryGetVector3Parameter(out v, parameters, "targetAngle", targetAngleAliases)) d.targetAngle = v;
            if (TryGetAnimCurveParameter(out var cv, parameters, "curve", curveAliases)) d.curve = cv;

            if (TryGetDefinedParameter(out string value, parameters, "pivot", pivotAliases))
            {
                if (TryGetAnchorParameter(out v, parameters, value))
                {
                    d.pivotType = VectorType.Anchor;
                    d.pivot = v;
                }
                else if (TryGetOffsetParameter(out v, parameters, value))
                {
                    d.pivotType = VectorType.PositionOffset;
                    d.pivot = v;
                }
                else if (TryGetVector3Parameter(out v, parameters, value))
                {
                    d.pivotType = VectorType.Position;
                    d.pivot = v;
                }
            }
        }

        public override bool ValidateParameters(IDictionary<string, string> parameters)
        {
            if (parameters == null) return true;

            if (HasNonFloatParameter(parameters, "duration", durationAliases)) return false;
            if (HasNonVector3Parameter(parameters, "startAngle", startAngleAngleAliases)) return false;
            if (HasNonVector3Parameter(parameters, "targetAngle", targetAngleAliases)) return false;
            if (HasNonAnimCurveParameter(parameters, "curve", curveAliases)) return false;

            if (TryGetDefinedParameter(out string value, parameters, "pivot", pivotAliases))
            {
                if (!HasVector3Parameter(parameters, value) && !HasAnchorParameter(parameters, value) && !HasOffsetParameter(parameters, value))
                {
                    return false;
                }
            }

            return true;
        }

        public override object GetNewCustomData()
        {
            return new Data
            {
                duration = this.duration,
                pivot = this.pivot,
                pivotType = this.pivotType,
                startAngle = this.startAngle,
                targetAngle = this.targetAngle,
                curve = this.curve
            };
        }

        private readonly string[] durationAliases = new string[] { "dur", "d" };
        private readonly string[] pivotAliases = new string[] { "p", "pv" };
        private readonly string[] startAngleAngleAliases = new string[] { "start", "st" };
        private readonly string[] targetAngleAliases = new string[] { "target", "tg" };
        private readonly string[] curveAliases = new string[] { "c", "crv" };

        private class Data
        {
            public float duration = 1f;
            public VectorType pivotType;
            public Vector3 pivot = Vector2.zero;
            public Vector3 startAngle;
            public Vector3 targetAngle;
            public AnimationCurve curve = AnimationCurveUtility.EaseInOutCubic();

            public bool pivotOnAnchor = false;
        }
    }
}