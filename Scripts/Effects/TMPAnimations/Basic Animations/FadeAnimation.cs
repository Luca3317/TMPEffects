using System.Collections;
using System.Collections.Generic;
using TMPEffects.CharacterData;
using TMPEffects.Extensions;
using TMPro;
using UnityEngine;
using static TMPEffects.ParameterUtility;
using static TMPEffects.TMPAnimations.AnimationUtility;

namespace TMPEffects.TMPAnimations.Animations
{
    [CreateAssetMenu(fileName = "new FadeAnimation", menuName = "TMPEffects/Animations/Fade")]
    public class FadeAnimation : TMPAnimation
    {
        [Tooltip("The wave that defines the behavior of this animation. No prefix.\nFor more information about Wave, see the section on it in the documentation.")]
        [SerializeField] Wave wave;
        [Tooltip("The way the offset for the wave is calculated.\nFor more information about Wave, see the section on it in the documentation.\nAliases: waveoffset, woffset, waveoff, woff")]
        [SerializeField] WaveOffsetType waveOffsetType;

        [Tooltip("The maximum opacity that is reached.\nAliases: maxopacity, maxop, max")]
        [SerializeField] float maxOpacity = 255;
        [Tooltip("The anchor used for fading in.\nAliases: fadeinanchor, fianchor, fianc, fia")]
        [SerializeField] Vector3 fadeInAnchor = Vector3.zero;
        [Tooltip("The direction to fade in in.\nAliases: fadeindirection, fidirection, fidir, fid")]
        [SerializeField] Vector3 fadeInDirection = Vector3.up;

        [Tooltip("The minimum opacity that is reached.\nAliases: minopacity, minop, min")]
        [SerializeField] float minOpacity = 0;
        [Tooltip("The anchor used for fading out.\nAliases: fadeoutanchor, foanchor, foanc, foa")]
        [SerializeField] Vector3 fadeOutAnchor = Vector3Int.zero;
        [Tooltip("The direction to fade out in.\nAliases: fadeoutdirection, fodirection, fodir, fod")]
        [SerializeField] Vector3 fadeOutDirection = Vector3.up;

        public override void Animate(CharData cData, IAnimationContext context)
        {
            Data d = context.CustomData as Data;

            (float, int) result = d.Wave.Evaluate(context.AnimatorContext.PassedTime, GetWaveOffset(cData, context, d.waveOffset));

            if (result.Item2 > 0)
            {
                FadeIn(cData, context, d, result.Item1);
            }
            else
            {
                FadeOut(cData, context, d, result.Item1);
            }
        }


        private void FadeIn(CharData cData, IAnimationContext context, Data d, float t)
        {
            Vector2 anchor = d.fadeInAnchor;
            FixAnchor(ref anchor);

            if (anchor == Vector2.zero)
            {
                for (int i = 0; i < 4; i++)
                {
                    float eval = Mathf.Lerp(d.minOpacity, d.maxOpacity, t);

                    Color32 c;
                    c = cData.mesh.initial.GetColor(i);
                    c.a = (byte)((eval / 255f) * c.a);

                    cData.mesh.SetAlpha(i, (byte)((eval / 255f) * c.a));
                }

                return;
            }

            Vector2 direction = new Vector2(-anchor.x, -anchor.y);
            Vector2 opposite = direction;

            Vector3 anchorPosition = AnchorToPosition(anchor, cData);
            Vector3 oppositePosition = AnchorToPosition(opposite, cData);
            float dist = (anchorPosition - oppositePosition).magnitude;

            for (int i = 0; i < 4; i++)
            {
                Vector3 vec = cData.mesh.initial.GetPosition(i) - anchorPosition;
                vec.x *= direction.x;
                vec.y *= direction.y;

                float currDist = (vec.magnitude / dist);

                float eval = Mathf.Lerp(d.minOpacity, d.maxOpacity, t * (2 - currDist));

                Color32 c;
                c = cData.mesh.initial.GetColor(i);
                c.a = (byte)((eval / 255f) * c.a);

                cData.mesh.SetAlpha(i, (byte)((eval / 255f) * c.a));
            }
        }

        private void FadeOut(CharData cData, IAnimationContext context, Data d, float t)
        {
            Vector2 anchor = d.fadeOutAnchor;
            FixAnchor(ref anchor);

            if (anchor == Vector2.zero)
            {
                for (int i = 0; i < 4; i++)
                {
                    float eval = Mathf.Lerp(d.minOpacity, d.maxOpacity, t);

                    Color32 c;
                    c = cData.mesh.initial.GetColor(i);
                    c.a = (byte)((eval / 255f) * c.a);

                    cData.mesh.SetAlpha(i, (byte)((eval / 255f) * c.a));
                }

                return;
            }

            Vector2 direction = new Vector2(-anchor.x, -anchor.y);
            Vector2 opposite = direction;

            Vector3 anchorPosition = AnchorToPosition(anchor, cData);
            Vector3 oppositePosition = AnchorToPosition(opposite, cData);
            float dist = (anchorPosition - oppositePosition).magnitude;

            for (int i = 0; i < 4; i++)
            {
                Vector3 vec = cData.mesh.initial.GetPosition(i) - anchorPosition;
                vec.x *= direction.x;
                vec.y *= direction.y;

                float currDist = (vec.magnitude / dist);

                float eval = Mathf.Lerp(d.minOpacity, d.maxOpacity, t * (2 - currDist));

                Color32 c;
                c = cData.mesh.initial.GetColor(i);
                c.a = (byte)((eval / 255f) * c.a);
                cData.mesh.SetAlpha(i, (byte)((eval / 255f) * c.a));
            }
        }

        private void FixAnchor(ref Vector2 v)
        {
            if (v.x != 0)
            {
                if (v.x > 0) v.x = 1;
                else v.x = -1;
            }
            if (v.y != 0)
            {
                if (v.y > 0) v.y = 1;
                else v.y = -1;
            }
        }

        public override void SetParameters(object customData, IDictionary<string, string> parameters)
        {
            if (parameters == null) return;

            Data d = (Data)customData;
            if (TryGetFloatParameter(out var f, parameters, "maxopacity", maxOpAliases)) d.maxOpacity = f;
            if (TryGetTypedVector2Parameter(out var tv2, parameters, "fadeinanchor", fadeInAnchorAliases)) d.fadeInAnchor = tv2.vector;
            if (TryGetVector2Parameter(out var v2, parameters, "fadeindirection", "fadeindir", "fidir")) d.fadeInDirection = v2;

            if (TryGetFloatParameter(out f, parameters, "minopacity", minOpAliases)) d.minOpacity = f;
            if (TryGetTypedVector2Parameter(out tv2, parameters, "fadeoutanchor", fadeOutAnchorAliases)) d.fadeOutAnchor = tv2.vector;
            if (TryGetVector2Parameter(out v2, parameters, "fadeoutdirection", "fadeoutdir", "fodir")) d.fadeOutDirection = v2;

            if (TryGetWaveOffsetParameter(out var offset, parameters, "waveoffset", WaveOffsetAliases)) d.waveOffset = offset;

            d.Wave = CreateWave(wave, GetWaveParameters(parameters));
        }

        public override bool ValidateParameters(IDictionary<string, string> parameters)
        {
            if (parameters == null) return true;

            if (HasNonFloatParameter(parameters, "maxopacity", maxOpAliases)) return false;
            if (HasNonTypedVector2Parameter(parameters, "fadeinanchor", fadeInAnchorAliases) || HasVector2OffsetParameter(parameters, "fadeoutanchor", fadeOutAnchorAliases)) return false;
            if (HasNonVector2Parameter(parameters, "fadeindirection", "fadeindir", "fidir", "fid")) return false;

            if (HasNonFloatParameter(parameters, "minopacity", minOpAliases)) return false;
            if (HasNonTypedVector2Parameter(parameters, "fadeoutanchor", fadeOutAnchorAliases) || HasVector2OffsetParameter(parameters, "fadeoutanchor", fadeOutAnchorAliases)) return false;
            if (HasNonVector2Parameter(parameters, "fadeoutdirection", "fadeoutdir", "fodir", "fod")) return false;

            if (HasNonWaveOffsetParameter(parameters, "waveoffset", WaveOffsetAliases)) return false;

            return ValidateWaveParameters(parameters);
        }

        public override object GetNewCustomData()
        {
            return new Data()
            {
                Wave = null,

                maxOpacity = this.maxOpacity,
                fadeInAnchor = this.fadeInAnchor,
                fadeInDirection = this.fadeInDirection,

                minOpacity = this.minOpacity,
                fadeOutAnchor = this.fadeOutAnchor,
                fadeOutDirection = this.fadeOutDirection,
            };
        }

        private readonly string[] maxOpAliases = new string[] { "max", "maxop" };
        private readonly string[] fadeInAnchorAliases = new string[] { "fianchor", "fianc", "fia" };

        private readonly string[] minOpAliases = new string[] { "min", "minop" };
        private readonly string[] fadeOutAnchorAliases = new string[] { "foanchor", "foanc", "foa" };

        private class Data
        {
            public Wave Wave;
            public WaveOffsetType waveOffset;

            public float maxOpacity;
            public Vector3 fadeInAnchor;
            public Vector3 fadeInDirection;

            public float minOpacity;
            public Vector3 fadeOutAnchor;
            public Vector3 fadeOutDirection;

            public readonly float sqrt2 = Mathf.Sqrt(8);
            public readonly float[] dists = new float[4];
        }
    }
}