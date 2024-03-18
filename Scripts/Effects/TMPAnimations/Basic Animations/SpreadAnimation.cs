using System.Collections.Generic;
using TMPEffects.Components.CharacterData;
using UnityEngine;
using static TMPEffects.TMPAnimations.AnimationUtility;
using static TMPEffects.ParameterUtility;
using TMPEffects.Extensions;

namespace TMPEffects.TMPAnimations.ShowAnimations
{
    // TODO 
    // Maybe rename variables shring/growanchor/direction to up/downanchor/direction to
    // be more in line with wave parameter names?

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

        [SerializeField] WaveOffsetType waveOffsetType;

        public override void Animate(CharData cData, IAnimationContext context)
        {
            Data d = context.customData as Data;

            (float, int) result = d.Wave.Evaluate(context.AnimationTimePassed,/* context.animatorContext.PassedTime,*/ GetWaveOffset(cData, context, d.offsetType));

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

            Vector3 actualDir = new Vector3(-d.growDirection.x, d.growDirection.y, 0f);

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

            Vector3 actualDir = new Vector3(-d.shrinkDirection.x, d.shrinkDirection.y, 0f);

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

            if (TryGetWaveOffsetParameter(out var offset, parameters, "waveoffset", "woffset", "waveoff", "woff")) d.offsetType = offset;

            d.Wave = CreateWave(wave, GetWaveParameters(parameters));
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

            if (HasNonWaveOffsetParameter(parameters, "waveoffset", "woffset", "waveoff", "woff")) return false;

            return ValidateWaveParameters(parameters);
        }

        public override object GetNewCustomData()
        {
            return new Data()
            {
                Wave = null,
                offsetType = this.waveOffsetType,

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

            public WaveOffsetType offsetType;

            public Vector3 growAnchor = new Vector3(0, -1, 0);
            public Vector3 growDirection = Vector3.up;

            public Vector3 shrinkAnchor = new Vector3(0, -1, 0);
            public Vector3 shrinkDirection = Vector3.up;

            public float maxPercentage = 1;
            public float minPercentage = 1;
        }
    }
}