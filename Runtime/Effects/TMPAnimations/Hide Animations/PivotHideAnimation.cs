using System.Collections.Generic;
using UnityEngine;
using TMPEffects.CharacterData;
using static TMPEffects.ParameterUtility;
using static TMPEffects.TMPAnimations.AnimationUtility;
using TMPEffects.Extensions;

namespace TMPEffects.TMPAnimations.HideAnimations
{
    [CreateAssetMenu(fileName = "new PivotHideAnimation", menuName = "TMPEffects/Hide Animations/Pivot")]
    public class PivotHideAnimation : TMPHideAnimation
    {
        [Tooltip("How long the animation will take to fully hide the character.\nAliases: duration, dur, d")]
        [SerializeField] private float duration = 1f;
        [Tooltip("The pivot position of the rotation.\nAliases: pivot, pv, p")]
        [SerializeField] private TypedVector2 pivot = new TypedVector2(VectorType.Anchor, Vector3.zero);
        [Tooltip("The start euler angles.\nAliases: startangle, start")]
        [SerializeField] private Vector3 startAngle = Vector3.zero;
        [Tooltip("The start euler angles.\nAliases: targetangle, target")]
        [SerializeField] private Vector3 targetAngle = new Vector3(0, 0, 210);
        [Tooltip("The curve used for getting the t-value to interpolate between the angles.\nAliases: curve, crv, c")]
        [SerializeField] private AnimationCurve curve = AnimationCurveUtility.EaseOutBack();

        public override void Animate(CharData cData, IAnimationContext context)
        {
            Data d = context.CustomData as Data;

            float t = Mathf.Lerp(0, 1, (context.AnimatorContext.PassedTime - context.AnimatorContext.StateTime(cData)) / d.duration);

            if (t == 1)
            {
                context.FinishAnimation(cData);
            }

            float t2 = d.curve.Evaluate(t);
            Vector3 angle = Vector3.LerpUnclamped(d.startAngle, d.targetAngle, t2);

            cData.SetRotation(Quaternion.Euler(angle));

            switch (d.pivot.type)
            {
                case VectorType.Position:
                    cData.SetPivot(d.pivot.vector);
                    break;
                case VectorType.Offset:
                    cData.SetPivot(cData.InitialPosition + (Vector3)d.pivot.vector);
                    break;
                case VectorType.Anchor:
                    Vector3 position = AnchorToPosition(d.pivot.vector, cData);
                    position.z = 0;
                    SetPivotRaw(position, cData, context);
                    break;
            }
        }

        public override void SetParameters(object customData, IDictionary<string, string> parameters)
        {
            if (parameters == null) return;

            Data d = customData as Data;
            if (TryGetFloatParameter(out float f, parameters, "duration", durationAliases)) d.duration = f;
            if (TryGetVector3Parameter(out Vector3 v, parameters, "startangle", startAngleAngleAliases)) d.startAngle = v;
            if (TryGetVector3Parameter(out v, parameters, "targetangle", targetAngleAliases)) d.targetAngle = v;
            if (TryGetAnimCurveParameter(out var cv, parameters, "curve", curveAliases)) d.curve = cv;
            if (TryGetTypedVector2Parameter(out var tv, parameters, "pivot", pivotAliases)) d.pivot = tv;
        }

        public override bool ValidateParameters(IDictionary<string, string> parameters)
        {
            if (parameters == null) return true;

            if (HasNonFloatParameter(parameters, "duration", durationAliases)) return false;
            if (HasNonVector3Parameter(parameters, "startangle", startAngleAngleAliases)) return false;
            if (HasNonVector3Parameter(parameters, "targetangle", targetAngleAliases)) return false;
            if (HasNonAnimCurveParameter(parameters, "curve", curveAliases)) return false;
            if (HasNonTypedVector2Parameter(parameters, "pivot", pivotAliases)) return false;

            return true;
        }

        public override object GetNewCustomData()
        {
            return new Data
            {
                duration = this.duration,
                pivot = this.pivot,
                startAngle = this.startAngle,
                targetAngle = this.targetAngle,
                curve = this.curve
            };
        }

        private readonly string[] durationAliases = new string[] { "dur", "d" };
        private readonly string[] pivotAliases = new string[] { "p", "pv" };
        private readonly string[] startAngleAngleAliases = new string[] { "start" };
        private readonly string[] targetAngleAliases = new string[] { "target" };
        private readonly string[] curveAliases = new string[] { "c", "crv" };

        private class Data
        {
            public float duration = 1f;
            public TypedVector2 pivot;
            public Vector3 startAngle;
            public Vector3 targetAngle;
            public AnimationCurve curve = AnimationCurveUtility.EaseInOutCubic();

            public bool pivotOnAnchor = false;
        }
    }
}