using System.Collections.Generic;
using TMPEffects.CharacterData;
using UnityEngine;
using static TMPEffects.TMPAnimations.AnimationUtility;
using static TMPEffects.ParameterUtility;
using TMPEffects.Extensions;

namespace TMPEffects.TMPAnimations.ShowAnimations
{
    [CreateAssetMenu(fileName = "new SpreadAnimation", menuName = "TMPEffects/Animations/Spread")]
    public class SpreadAnimation : TMPAnimation
    {
        [Tooltip("The wave that defines the behavior of this animation. No prefix.\nFor more information about Wave, see the section on it in the documentation.")]
        [SerializeField] Wave wave;
        [Tooltip("The way the offset for the wave is calculated.\nFor more information about Wave, see the section on it in the documentation.\nAliases: waveoffset, woffset, waveoff, woff")]
        [SerializeField] WaveOffsetType waveOffsetType;

        [Tooltip("The anchor used for growing.\nAliases: growanchor, growanc, ganc")]
        [SerializeField] TypedVector3 growAnchor = new TypedVector3(VectorType.Anchor, Vector3.zero);
        [Tooltip("The direction used for growing.\nAliases: growdirection, growdir, gdir")]
        [SerializeField] Vector3 growDirection = Vector3.up;

        [Tooltip("The anchor used for shrinking.\nAliases: shrinkanchor, shrinkanc, sanc")]
        [SerializeField] TypedVector3 shrinkAnchor = new TypedVector3(VectorType.Anchor, Vector3.zero);
        [Tooltip("The direction used for shrinking.\nAliases: shrinkdirection, shrinkdir, sdir")]
        [SerializeField] Vector3 shrinkDirection = Vector3.up;

        [Tooltip("The maximum percentage to spread to, at 1 being completely shown.\nAliases: maxpercentage, maxp, max")]
        [SerializeField] float maxPercentage = 1;
        [Tooltip("The minimum percentage to unspread to, at 0 being completely hidden.\nAliases: minpercentage, minp, min")]
        [SerializeField] float minPercentage = 0;

        public override void Animate(CharData cData, IAnimationContext context)
        {
            Data d = context.CustomData as Data;

            // Evaluate the wave based on time and offset
            (float, int) result = d.Wave.Evaluate(context.AnimatorContext.PassedTime, GetWaveOffset(cData, context, d.offsetType));

            // If the wave is moving up, grow the character
            if (result.Item2 > 0)
            {
                Grow(cData, context, d, result.Item1);
            }
            // else, shrink the character
            else
            {
                Shrink(cData, context, d, result.Item1);
            }
        }

        private void Grow(CharData cData, IAnimationContext context, Data d, float t)
        {
            float percentage = Mathf.LerpUnclamped(d.minPercentage, d.maxPercentage, t);

            Vector3 actualDir = new Vector3(-d.growDirection.y, d.growDirection.x, 0f);

            Vector3 lineStart, lineEnd;

            switch (d.growAnchor.type)
            {
                case VectorType.Offset:
                    lineStart = cData.InitialPosition + (d.growAnchor.vector - actualDir * 2);
                    lineEnd = cData.InitialPosition + (d.growAnchor.vector + actualDir * 2);
                    break;
                case VectorType.Anchor:
                    lineStart = AnchorToPosition(d.growAnchor.vector - actualDir * 2, cData);
                    lineEnd = AnchorToPosition(d.growAnchor.vector + actualDir * 2, cData);
                    break;
                case VectorType.Position:
                    lineStart = d.growAnchor.vector - actualDir * 2;
                    lineEnd = d.growAnchor.vector + actualDir * 2;
                    break;

                default: throw new System.NotImplementedException(nameof(growAnchor.type));
            }

            for (int i = 0; i < 4; i++)
            {
                Vector3 startPos = ClosestPointOnLine(lineStart, lineEnd, cData.mesh.initial.GetPosition(i));
                Vector3 pos = Vector3.LerpUnclamped(startPos, cData.mesh.initial.GetPosition(i), percentage);

                SetVertexRaw(i, pos, cData, context);
            }
        }

        private void Shrink(CharData cData, IAnimationContext context, Data d, float t)
        {
            float percentage = Mathf.LerpUnclamped(d.minPercentage, d.maxPercentage, t);

            Vector3 actualDir = new Vector3(-d.shrinkDirection.y, d.shrinkDirection.x, 0f);

            Vector3 lineStart, lineEnd;

            switch (d.shrinkAnchor.type)
            {
                case VectorType.Offset:
                    lineStart = cData.InitialPosition + (d.shrinkAnchor.vector - actualDir * 2);
                    lineEnd = cData.InitialPosition + (d.shrinkAnchor.vector + actualDir * 2);
                    break;
                case VectorType.Anchor:
                    lineStart = AnchorToPosition(d.shrinkAnchor.vector - actualDir * 2, cData);
                    lineEnd = AnchorToPosition(d.shrinkAnchor.vector + actualDir * 2, cData);
                    break;
                case VectorType.Position:
                    lineStart = d.shrinkAnchor.vector - actualDir * 2;
                    lineEnd = d.shrinkAnchor.vector + actualDir * 2;
                    break;

                default: throw new System.NotImplementedException(nameof(shrinkAnchor.type));
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
            if (TryGetVector3Parameter(out Vector3 v, parameters, "growDirection", "growdir", "gdir")) d.growDirection = v;
            if (TryGetTypedVector3Parameter(out TypedVector3 tv, parameters, "growanchor", "growanc", "ganc")) d.growAnchor = tv;

            if (TryGetVector3Parameter(out v, parameters, "shrinkdirection", "shrinkdir", "sdir")) d.shrinkDirection = v;
            if (TryGetTypedVector3Parameter(out tv, parameters, "shrinkanchor", "shrinkanc", "sanc")) d.shrinkAnchor = tv;

            if (TryGetFloatParameter(out float f, parameters, "maxpercentage", "maxp", "max")) d.maxPercentage = f;
            if (TryGetFloatParameter(out f, parameters, "minpercentage", "minp", "min")) d.minPercentage = f;

            if (TryGetWaveOffsetParameter(out var offset, parameters, "waveoffset", WaveOffsetAliases)) d.offsetType = offset;

            d.Wave = CreateWave(wave, GetWaveParameters(parameters));
        }

        public override bool ValidateParameters(IDictionary<string, string> parameters)
        {
            if (parameters == null) return true;

            if (!ValidateWaveParameters(parameters)) return false;
            if (HasNonVector3Parameter(parameters, "growdirection", "growdir", "gdir")) return false;
            if (HasNonTypedVector3Parameter(parameters, "growanchor", "growanc", "ganc")) return false;

            if (HasNonVector3Parameter(parameters, "shrinkdirection", "shrinkdir", "sdir")) return false;
            if (HasNonTypedVector3Parameter(parameters, "shrinkanchor", "shrinkanc", "sanc")) return false;

            if (HasNonFloatParameter(parameters, "maxpercentage", "maxp", "max")) return false;
            if (HasNonFloatParameter(parameters, "minpercentage", "minp", "min")) return false;

            if (HasNonWaveOffsetParameter(parameters, "waveoffset", WaveOffsetAliases)) return false;

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

            public TypedVector3 growAnchor;
            public Vector3 growDirection;

            public TypedVector3 shrinkAnchor;
            public Vector3 shrinkDirection;

            public float maxPercentage = 1;
            public float minPercentage = 1;
        }
    }
}