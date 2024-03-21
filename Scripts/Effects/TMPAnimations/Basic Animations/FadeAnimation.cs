using System.Collections;
using System.Collections.Generic;
using TMPEffects.CharacterData;
using TMPEffects.Extensions;
using TMPro;
using UnityEngine;
using static TMPEffects.ParameterUtility;
using static TMPEffects.TMPAnimations.AnimationUtility;
using static TMPro.TMP_Compatibility;

namespace TMPEffects.TMPAnimations.Animations
{
    [CreateAssetMenu(fileName = "new FadeAnimation", menuName = "TMPEffects/Animations/Fade")]
    public class FadeAnimation : TMPAnimation
    {
        [SerializeField] Wave wave;
        [SerializeField] WaveOffsetType waveOffsetType;

        [SerializeField] float maxOpacity = 255;
        [SerializeField] Vector3 fadeInAnchor = Vector3.zero;
        [SerializeField] Vector3 fadeInDirection = Vector3.up;

        [SerializeField] float minOpacity = 0;
        [SerializeField] Vector3Int fadeOutAnchor = Vector3Int.zero;
        [SerializeField] Vector3 fadeOutDirection = Vector3.up;

        public override void Animate(CharData cData, IAnimationContext context)
        {
            Data d = context.customData as Data;

            (float, int) result = d.Wave.Evaluate(context.animatorContext.PassedTime, GetWaveOffset(cData, context, d.waveOffset));

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
                    float eval = Mathf.Lerp(minOpacity, maxOpacity, t);

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
                Vector3 vec = cData.mesh.initial.GetVertex(i) - anchorPosition;
                vec.x *= direction.x;
                vec.y *= direction.y;

                float currDist = (vec.magnitude / dist);

                float eval = Mathf.Lerp(minOpacity, maxOpacity, t * (2 - currDist));

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
                    float eval = Mathf.Lerp(minOpacity, maxOpacity, t);

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
                Vector3 vec = cData.mesh.initial.GetVertex(i) - anchorPosition;
                vec.x *= direction.x;
                vec.y *= direction.y;

                float currDist = (vec.magnitude / dist);

                float eval = Mathf.Lerp(minOpacity, maxOpacity, t * (2 - currDist));

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
            if (TryGetFloatParameter(out var f, parameters, "maxOpacity", maxOpAliases)) d.maxOpacity = f;
            if (TryGetVector2Parameter(out var v2, parameters, "fadeInAnchor", fadeInAnchorAliases)) d.fadeInAnchor = v2;
            if (TryGetVector2Parameter(out v2, parameters, "fadeInDirection", "fadeInDir", "fiDir")) d.fadeInDirection = v2;

            if (TryGetFloatParameter(out f, parameters, "minOpacity", minOpAliases)) d.minOpacity = f;
            if (TryGetVector2Parameter(out v2, parameters, "fadeOutAnchor", fadeOutAnchorAliases)) d.fadeOutAnchor = v2;
            if (TryGetVector2Parameter(out v2, parameters, "fadeOutDirection", "fadeOutDir", "foDir")) d.fadeOutDirection = v2;

            if (TryGetWaveOffsetParameter(out var offset, parameters, "waveoffset", WaveOffsetAliases)) d.waveOffset = offset;

            d.Wave = CreateWave(wave, GetWaveParameters(parameters));
        }

        public override bool ValidateParameters(IDictionary<string, string> parameters)
        {
            if (parameters == null) return true;

            if (HasNonFloatParameter(parameters, "maxOpacity", maxOpAliases)) return false;
            if (HasNonVector2Parameter(parameters, "fadeInAnchor", fadeInAnchorAliases)) return false;
            if (HasNonVector2Parameter(parameters, "fadeInDirection", "fadeInDir", "fiDir")) return false;

            if (HasNonFloatParameter(parameters, "minOpacity", minOpAliases)) return false;
            if (HasNonVector2Parameter(parameters, "fadeOutAnchor", "fadeOutAnc", "foAnc", "foA")) return false;
            if (HasNonVector2Parameter(parameters, "fadeOutDirection", "fadeOutDir", "foDir")) return false;

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

        private readonly string[] maxOpAliases = new string[] { "max", "maxOp" };
        private readonly string[] fadeInAnchorAliases = new string[] { "fiAnchor", "fiAnc", "fiA" };

        private readonly string[] minOpAliases = new string[] { "min", "minOp" };
        private readonly string[] fadeOutAnchorAliases = new string[] { "foAnchor", "foAnc", "foA" };

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