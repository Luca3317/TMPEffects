using System.Collections.Generic;
using TMPEffects.CharacterData;
using UnityEngine;
using static TMPEffects.TMPAnimations.AnimationUtility;
using static TMPEffects.ParameterUtility;
using TMPEffects.Extensions;

namespace TMPEffects.TMPAnimations.ShowAnimations
{
    [CreateAssetMenu(fileName = "new SpreadShowAnimation", menuName = "TMPEffects/Show Animations/Spread")]
    public class SpreadShowAnimation : TMPShowAnimation
    {
        [Tooltip("How long the animation will take to fully show the character.\nAliases: duration, dur, d")]
        [SerializeField] float duration = 1;
        [Tooltip("The curve used for getting the t-value to interpolate between the percentages.\nAliases: curve, crv, c")]
        [SerializeField] AnimationCurve curve = AnimationCurveUtility.EaseOutElastic();
        [Tooltip("The anchor from where the character spreads.\nAliases: anchor, anc, a")]
        [SerializeField] TypedVector2 anchor = new TypedVector2(VectorType.Anchor, Vector2.zero);
        [Tooltip("The direction in which the character spreads.\nAliases: direction, dir")]
        [SerializeField] Vector3 direction = Vector3.up;
        [Tooltip("The start percentage of the spread, 0 being fully hidden.\nAliases: startpercentage, start")]
        [SerializeField] float startPercentage = 0;
        [Tooltip("The target percentage of the spread, 1 being fully shown.\nAliases: targetpercentage, target")]
        [SerializeField] float targetPercentage = 1;

        public override void Animate(CharData cData, IAnimationContext context)
        {
            Data d = context.CustomData as Data;

            float t = Mathf.Lerp(d.startPercentage, d.targetPercentage, (context.AnimatorContext.PassedTime - context.AnimatorContext.StateTime(cData)) / d.duration);
            float t2 = d.curve.Evaluate(t);

            float l = Mathf.Lerp(0f, 1f, (context.AnimatorContext.PassedTime - context.AnimatorContext.StateTime(cData)) / d.duration);
            if (l == 1)
                context.FinishAnimation(cData);

            Grow(cData, context, d, t2);
        }

        private void Grow(CharData cData, IAnimationContext context, Data d, float t)
        {
            float percentage = Mathf.LerpUnclamped(d.startPercentage, d.targetPercentage, t);

            Vector2 actualDir = new Vector2(-d.direction.y, d.direction.x);

            Vector3 lineStart, lineEnd;

            switch (d.anchor.type)
            {
                case VectorType.Offset:
                    lineStart = cData.InitialPosition + (Vector3)(d.anchor.vector - actualDir * 2);
                    lineEnd = cData.InitialPosition + (Vector3)(d.anchor.vector + actualDir * 2);
                    break;
                case VectorType.Anchor:
                    lineStart = AnchorToPosition(d.anchor.vector - actualDir * 2, cData);
                    lineEnd = AnchorToPosition(d.anchor.vector + actualDir * 2, cData);
                    break;
                case VectorType.Position:
                    lineStart = d.anchor.vector - actualDir * 2;
                    lineEnd = d.anchor.vector + actualDir * 2;
                    break;

                default: throw new System.NotImplementedException(nameof(anchor.type));
            }

            for (int i = 0; i < 4; i++)
            {
                Vector3 startPos = ClosestPointOnLine(lineStart, lineEnd, cData.mesh.initial.GetPosition(i));
                Vector3 pos = Vector3.LerpUnclamped(startPos, cData.mesh.initial.GetPosition(i), percentage);

                SetVertexRaw(i, pos, cData, context);
            }
        }


        public override void SetParameters(object customData, IDictionary<string, string> parameters)
        {
            if (parameters == null) return;

            Data d = customData as Data;
            if (TryGetFloatParameter(out float f, parameters, "duration", "dur", "d")) d.duration = f;
            if (TryGetFloatParameter(out f, parameters, "startpercentage", "start")) d.startPercentage = f;
            if (TryGetFloatParameter(out f, parameters, "targetpercentage", "target")) d.targetPercentage = f;
            if (TryGetVector3Parameter(out Vector3 v, parameters, "direction", "dir")) d.direction = v;
            if (TryGetTypedVector2Parameter(out var tv2, parameters, "anchor", "anc", "a")) d.anchor = tv2;
            if (TryGetAnimCurveParameter(out AnimationCurve crv, parameters, "curve", "crv", "c")) d.curve = crv;
        }

        public override bool ValidateParameters(IDictionary<string, string> parameters)
        {
            if (parameters == null) return true;

            if (HasNonFloatParameter(parameters, "duration", "dur", "d")) return false;
            if (HasNonFloatParameter(parameters, "startpercentage", "start")) return false;
            if (HasNonFloatParameter(parameters, "targetpercentage", "target")) return false;
            if (HasNonVector3Parameter(parameters, "direction", "dir")) return false;
            if (HasNonTypedVector2Parameter(parameters, "anchor", "anc", "a")) return false;
            if (HasNonAnimCurveParameter(parameters, "curve", "crv", "c")) return false;
            return true;
        }

        public override object GetNewCustomData()
        {
            return new Data()
            {
                duration = this.duration,
                anchor = this.anchor,
                direction = this.direction,
                startPercentage = this.startPercentage,
                targetPercentage = this.targetPercentage,
                curve = this.curve
            };
        }

        private class Data
        {
            public float duration;
            public TypedVector2 anchor;
            public Vector3 direction;
            public float startPercentage;
            public float targetPercentage;
            public AnimationCurve curve;
        }
    }
}

