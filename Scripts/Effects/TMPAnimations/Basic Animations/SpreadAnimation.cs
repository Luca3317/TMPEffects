using System.Collections.Generic;
using TMPEffects.Components.CharacterData;
using UnityEngine;
using static TMPEffects.TMPAnimations.AnimationUtility;
using static TMPEffects.ParameterUtility;
using TMPEffects.Extensions;

namespace TMPEffects.TMPAnimations.ShowAnimations
{
    [CreateAssetMenu(fileName = "new SpreadAnimation", menuName = "TMPEffects/Animations/Spread")]
    public class SpreadAnimation : TMPAnimation
    {
        [SerializeField] float growDuration = 1;
        [SerializeField] AnimationCurve growCurve = AnimationCurveUtility.EaseOutElastic();

        [SerializeField] float shrinkDuration = 1;
        [SerializeField] AnimationCurve shrinkCurve = AnimationCurveUtility.EaseOutElastic();

        [SerializeField] Vector3 growAnchor = new Vector3(0, -1, 0);
        [SerializeField] Vector3 growDirection = Vector3.up;

        [SerializeField] Vector3 shrinkAnchor = new Vector3(0, -1, 0);
        [SerializeField] Vector3 shrinkDirection = Vector3.up;

        [SerializeField] float maxPercentage = 1;
        [SerializeField] float minPercentage = 0;

        [SerializeField] bool indexbased;

        public override void Animate(CharData cData, IAnimationContext context)
        {
            Data d = context.customData as Data;

            d.Wave ??= new Wave(growCurve, shrinkCurve, growDuration, shrinkDuration, velocity, amplitude, crestWait, throughWait);

            (float, int) result;
            if (indexbased)
            {
                result = d.Wave.Evaluate(context.animatorContext.PassedTime, context.segmentData.SegmentIndexOf(cData) * uniformity);
            }
            else
            {
                float xPos = cData.info.initialPosition.x;
                xPos /= (cData.info.referenceScale / 36f);
                xPos /= 2000f;
                result = d.Wave.Evaluate(context.animatorContext.PassedTime, xPos * uniformity);
            }

            if (result.Item2 > 0)
            {
                Grow(cData, context, d, result.Item1);
            }
            else
            {
                Shrink(cData, context, d, result.Item1);
            }
        }

        private void Grow(CharData cData, IAnimationContext context, Data d, float t)
        {
            float percentage = Mathf.LerpUnclamped(d.minPercentage, d.maxPercentage, t);

            Vector3 actualDir = new Vector3(d.growDirection.y, d.growDirection.x, 0f);

            Vector3 lineStart = AnchorToPosition(d.growAnchor - actualDir * 2, cData);
            Vector3 lineEnd = AnchorToPosition(d.growAnchor + actualDir * 2, cData);

            for (int i = 0; i < 4; i++)
            {
                Vector3 startPos = ClosestPointOnLine(lineStart, lineEnd, cData.mesh.initial.GetVertex(i));
                Vector3 pos = Vector3.LerpUnclamped(startPos, cData.mesh.initial.GetVertex(i), percentage);

                SetVertexRaw(i, pos, cData, ref context);
            }
        }

        private void Shrink(CharData cData, IAnimationContext context, Data d, float t)
        {
            float percentage = Mathf.LerpUnclamped(d.minPercentage, d.maxPercentage, t);

            Vector3 actualDir = new Vector3(d.shrinkDirection.y, d.shrinkDirection.x, 0f);

            Vector3 lineStart = AnchorToPosition(d.shrinkAnchor - actualDir * 2, cData);
            Vector3 lineEnd = AnchorToPosition(d.shrinkAnchor + actualDir * 2, cData);

            for (int i = 0; i < 4; i++)
            {
                Vector3 startPos = ClosestPointOnLine(lineStart, lineEnd, cData.mesh.initial.GetVertex(i));
                Vector3 pos = Vector3.LerpUnclamped(startPos, cData.mesh.initial.GetVertex(i), percentage);

                SetVertexRaw(i, pos, cData, ref context);
            }
        }

        public override void SetParameters(object customData, IDictionary<string, string> parameters)
        {
            if (parameters == null) return;

            Data d = customData as Data;
            if (TryGetFloatParameter(out float f, parameters, "growDuration", "growDur", "growD", "gD")) d.growDuration = f;
            if (TryGetVector3Parameter(out Vector3 v, parameters, "growDirection", "growDir", "gDir")) d.growDirection = v;
            if (TryGetVector3Parameter(out v, parameters, "growAnchor", "growAnc", "gAnc") || TryGetAnchorParameter(out v, parameters, "growAnchor", "growAnc", "gAnc")) d.growAnchor = v;
            if (TryGetAnimCurveParameter(out AnimationCurve crv, parameters, "growCurve", "growCrv", "growC", "gC")) d.growCurve = crv;

            if (TryGetFloatParameter(out f, parameters, "shrinkDuration", "shrinkDur", "shrinkD", "sD")) d.shrinkDuration = f;
            if (TryGetVector3Parameter(out v, parameters, "shrinkDirection", "shrinkDir", "sDir")) d.shrinkDirection = v;
            if (TryGetVector3Parameter(out v, parameters, "shrinkAnchor", "shrinkAnc", "sAnc") || TryGetAnchorParameter(out v, parameters, "shrinkAnchor", "shrinkAnc", "gAnc")) d.shrinkAnchor = v;
            if (TryGetAnimCurveParameter(out crv, parameters, "shrinkCurve", "shrinkCrv", "shrinkC", "sC")) d.shrinkCurve = crv;

            if (TryGetFloatParameter(out f, parameters, "maxPercentage", "maxP")) d.maxPercentage = f;
            if (TryGetFloatParameter(out f, parameters, "minPercentage", "minP")) d.minPercentage = f;

            WaveParameters wp = GetWaveParameters(parameters);

        }

        public override bool ValidateParameters(IDictionary<string, string> parameters)
        {
            if (parameters == null) return true;

            if (!ValidateWaveParameters(parameters)) return false;
            if (HasNonFloatParameter(parameters, "growDuration", "growDur", "growD", "gD")) return false;
            if (HasNonVector3Parameter(parameters, "growDirection", "growDir", "gDir")) return false;
            if (HasNonVector3Parameter(parameters, "growAnchor", "growAnc", "gAnc") && HasNonAnchorParameter(parameters, "growAnchor", "growAnc", "gAnc")) return false;
            if (HasNonAnimCurveParameter(parameters, "growCurve", "growCrv", "growC", "gC")) return false;

            if (HasNonFloatParameter(parameters, "shrinkDuration", "shrinkDur", "shrinkD", "sD")) return false;
            if (HasNonVector3Parameter(parameters, "shrinkDirection", "shrinkDir", "sDir")) return false;
            if (HasNonVector3Parameter(parameters, "shrinkAnchor", "shrinkAnc", "sAnc") && HasNonAnchorParameter(parameters, "shrinkAnchor", "shrinkAnc", "gAnc")) return false;
            if (HasNonAnimCurveParameter(parameters, "shrinkCurve", "shrinkCrv", "shrinkC", "sC")) return false;

            if (HasNonFloatParameter(parameters, "maxPercentage", "maxP")) return false;
            if (HasNonFloatParameter(parameters, "minPercentage", "minP")) return false;

            return true;
        }

        public override object GetNewCustomData()
        {
            return new Data()
            {
                Wave = null,
                growDuration = this.growDuration,
                growAnchor = this.growAnchor,
                growDirection = this.growDirection,
                growCurve = this.growCurve,

                shrinkDuration = this.shrinkDuration,
                shrinkAnchor = this.shrinkAnchor,
                shrinkDirection = this.shrinkDirection,
                shrinkCurve = this.shrinkCurve,

                maxPercentage = this.maxPercentage,
                minPercentage = this.minPercentage,
            };
        }

        private class Data
        {
            public Wave Wave;

            public float growDuration = 1;
            public Vector3 growAnchor = new Vector3(0, -1, 0);
            public Vector3 growDirection = Vector3.up;
            public AnimationCurve growCurve = AnimationCurveUtility.EaseOutElastic();

            public float shrinkDuration = 1;
            public Vector3 shrinkAnchor = new Vector3(0, -1, 0);
            public Vector3 shrinkDirection = Vector3.up;
            public AnimationCurve shrinkCurve = AnimationCurveUtility.EaseOutElastic();

            public float maxPercentage = 1;
            public float minPercentage = 1;
        }
    }
}