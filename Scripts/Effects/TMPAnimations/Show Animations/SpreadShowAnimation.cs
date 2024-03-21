using System.Collections.Generic;
using TMPEffects.CharacterData;
using UnityEngine;
using static TMPEffects.TMPAnimations.AnimationUtility;
using static TMPEffects.ParameterUtility;
using TMPEffects.Extensions;

namespace TMPEffects.TMPAnimations.ShowAnimations
{
    [CreateAssetMenu(fileName = "new SpreadShowAnimation", menuName = "TMPEffects/Show Animations/Spread")]
    public class SpreadShowAnimation : TMPShowyAnimation
    {
        [SerializeField] float duration = 1;
        [SerializeField] Vector3 anchor = new Vector3(0, -1, 0);
        [SerializeField] Vector3 direction = Vector3.up;
        [SerializeField] float startPercentage = 0;
        [SerializeField] float targetPercentage = 1;
        [SerializeField] AnimationCurve curve = AnimationCurveUtility.EaseOutElastic();

        public override void Animate(CharData cData, IAnimationContext context)
        {
            Data d = context.customData as Data;

            float t = Mathf.Lerp(d.startPercentage, d.targetPercentage, (context.animatorContext.PassedTime - context.animatorContext.StateTime(cData)) / d.duration);
            float t2 = d.curve.Evaluate(t);

            float l = Mathf.Lerp(0f, 1f, (context.animatorContext.PassedTime - context.animatorContext.StateTime(cData)) / d.duration);
            if (l == 1)
                context.FinishAnimation(cData);

            Vector3 actualDir = new Vector3(d.direction.y, d.direction.x, 0f);

            Vector3 lineStart = AnchorToPosition(d.anchor - actualDir * 2, cData);
            Vector3 lineEnd = AnchorToPosition(d.anchor + actualDir * 2, cData);

            for (int i = 0; i < 4; i++)
            {
                Vector3 startPos = ClosestPointOnLine(lineStart, lineEnd, cData.mesh.initial.GetVertex(i));
                Vector3 pos = Vector3.LerpUnclamped(startPos, cData.mesh.initial.GetVertex(i), t2);

                SetVertexRaw(i, pos, cData, ref context);
            }
        }

        public override void SetParameters(object customData, IDictionary<string, string> parameters)
        {
            if (parameters == null) return;

            Data d = customData as Data;
            if (TryGetFloatParameter(out float f, parameters, "duration", "dur", "d")) d.duration = f;
            if (TryGetFloatParameter(out f, parameters, "startPercentage", "start", "st")) d.startPercentage = f;
            if (TryGetFloatParameter(out f, parameters, "targetPercentage", "target", "tg")) d.targetPercentage = f;
            if (TryGetVector3Parameter(out Vector3 v, parameters, "direction", "dir")) d.direction = v;
            if (TryGetVector3Parameter(out v, parameters, "anchor", "anc") || TryGetAnchorParameter(out v, parameters, "anchor", "anc")) d.anchor = v;
            if (TryGetAnimCurveParameter(out AnimationCurve crv, parameters, "curve", "crv", "c")) d.curve = crv;
        }

        public override bool ValidateParameters(IDictionary<string, string> parameters)
        {
            if (parameters == null) return true;

            if (HasNonFloatParameter(parameters, "duration", "dur", "d")) return false;
            if (HasNonFloatParameter(parameters, "startPercentage", "start", "st")) return false;
            if (HasNonFloatParameter(parameters, "targetPercentage", "target", "tg")) return false;
            if (HasNonVector3Parameter(parameters, "anchor", "anc") && HasNonAnchorParameter(parameters, "anchor", "anc")) return false;
            if (HasNonVector3Parameter(parameters, "direction", "dir")) return false;
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
            public Vector3 anchor;
            public Vector3 direction;
            public float startPercentage;
            public float targetPercentage;
            public AnimationCurve curve;
        }
    }
}

