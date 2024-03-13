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
        [SerializeField] Wave wave;

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

            (float, int) result;
            if (indexbased)
            {
                result = d.Wave.Evaluate(context.animatorContext.PassedTime, context.segmentData.SegmentIndexOf(cData));
            }
            else
            {
                float xPos = cData.info.initialPosition.x;
                xPos /= (cData.info.referenceScale / 36f);
                xPos /= 2000f;
                result = d.Wave.Evaluate(context.animatorContext.PassedTime, xPos);
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
            if (TryGetVector3Parameter(out Vector3 v, parameters, "growDirection", "growDir", "gDir")) d.growDirection = v;
            if (TryGetVector3Parameter(out v, parameters, "growAnchor", "growAnc", "gAnc") || TryGetAnchorParameter(out v, parameters, "growAnchor", "growAnc", "gAnc")) d.growAnchor = v;

            if (TryGetVector3Parameter(out v, parameters, "shrinkDirection", "shrinkDir", "sDir")) d.shrinkDirection = v;
            if (TryGetVector3Parameter(out v, parameters, "shrinkAnchor", "shrinkAnc", "sAnc") || TryGetAnchorParameter(out v, parameters, "shrinkAnchor", "shrinkAnc", "gAnc")) d.shrinkAnchor = v;

            if (TryGetFloatParameter(out float f, parameters, "maxPercentage", "maxP")) d.maxPercentage = f;
            if (TryGetFloatParameter(out f, parameters, "minPercentage", "minP")) d.minPercentage = f;

            WaveParameters wp = GetWaveParameters(parameters);
            float upPeriod = wp.upPeriod == null ? wave.UpPeriod : wp.upPeriod.Value;
            float downPeriod = wp.downPeriod == null ? wave.DownPeriod : wp.downPeriod.Value;

            if (wave == null) Debug.Log("its null?");

            float velocity = wave.Velocity; 
            if (wp.wavevelocity != null) velocity = wp.wavevelocity.Value;
            else if (wp.wavelength != null) velocity = wp.wavelength.Value * (1f / (upPeriod + downPeriod)); // TODO math

            d.Wave = new Wave
            (
                wp.upwardCurve == null ? wave.UpwardCurve : wp.upwardCurve,
                wp.downwardCurve == null ? wave.DownwardCurve : wp.downwardCurve,
                upPeriod,
                downPeriod,
                velocity,
                wp.amplitude == null ? wave.Amplitude : wp.amplitude.Value
            );
        }

        public override bool ValidateParameters(IDictionary<string, string> parameters)
        {
            if (parameters == null) return true;

            if (!ValidateWaveParameters(parameters)) return false;
            if (HasNonVector3Parameter(parameters, "growDirection", "growDir", "gDir")) return false;
            if (HasNonVector3Parameter(parameters, "growAnchor", "growAnc", "gAnc") && HasNonAnchorParameter(parameters, "growAnchor", "growAnc", "gAnc")) return false;

            if (HasNonVector3Parameter(parameters, "shrinkDirection", "shrinkDir", "sDir")) return false;
            if (HasNonVector3Parameter(parameters, "shrinkAnchor", "shrinkAnc", "sAnc") && HasNonAnchorParameter(parameters, "shrinkAnchor", "shrinkAnc", "gAnc")) return false;

            if (HasNonFloatParameter(parameters, "maxPercentage", "maxP")) return false;
            if (HasNonFloatParameter(parameters, "minPercentage", "minP")) return false;

            return ValidateWaveParameters(parameters);
        }

        public override object GetNewCustomData()
        {
            return new Data()
            {
                Wave = null,

                growAnchor = this.growAnchor,
                growDirection = this.growDirection,

                shrinkAnchor = this.shrinkAnchor,
                shrinkDirection = this.shrinkDirection,

                maxPercentage = this.maxPercentage,
                minPercentage = this.minPercentage,
            };
        }

        private class Data
        {
            public Wave Wave;

            public Vector3 growAnchor = new Vector3(0, -1, 0);
            public Vector3 growDirection = Vector3.up;

            public Vector3 shrinkAnchor = new Vector3(0, -1, 0);
            public Vector3 shrinkDirection = Vector3.up;

            public float maxPercentage = 1;
            public float minPercentage = 1;
        }
    }
}