using System.Collections.Generic;
using TMPEffects.Components.Animator;
using TMPEffects.CharacterData;
using TMPEffects.Extensions;
using UnityEngine;
using static TMPEffects.ParameterUtility;
using static TMPEffects.TMPAnimations.AnimationUtility;

namespace TMPEffects.TMPAnimations.HideAnimations
{
    [CreateAssetMenu(fileName = "new FadeHideAnimation", menuName = "TMPEffects/Hide Animations/Fade")]
    public class FadeHideAnimation : TMPHideAnimation
    {
        [Tooltip("How long the animation will take to fully hide the character.\nAliases: duration, dur, d")]
        [SerializeField] float duration = 1;
        [Tooltip("The curve used for fading out.\nAliases: curve, crv, c")]
        [SerializeField] AnimationCurve curve = AnimationCurveUtility.EaseOutSine();

        [Tooltip("The opacity that is faded out to.\nAliases: targetopacity, targetop, target")]
        [SerializeField] float targetOpacity = 0;
        [Tooltip("The anchor that is faded out from.\nAliases: anchor, anc, a")]
        [SerializeField] Vector3 anchor = Vector3.zero;
        [Tooltip("The direction used for fading out.\nAliases: direction, dir")]
        [SerializeField] Vector3 direction = Vector3.up;

        public override void Animate(CharData cData, IAnimationContext context)
        {
            IAnimatorContext ac = context.AnimatorContext;
            Data d = context.CustomData as Data;

            float t = d.duration > 0 ? Mathf.Clamp01((ac.PassedTime - ac.StateTime(cData)) / d.duration) : 1f;
            float t2 = d.curve.Evaluate(t);

            if (t == 1) context.FinishAnimation(cData);

            FadeOut(cData, context, d, t2);
        }

        private void FadeOut(CharData cData, IAnimationContext context, Data d, float t)
        {
            Vector2 anchor = d.anchor;
            FixAnchor(ref anchor);

            if (anchor == Vector2.zero)
            {
                for (int i = 0; i < 4; i++)
                {
                    float eval = Mathf.Lerp(cData.info.color.a, d.targetOpacity, t);

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

                float eval = Mathf.Lerp(cData.info.color.a, d.targetOpacity, t * (2 - currDist));

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
            if (TryGetAnimCurveParameter(out var c, parameters, "curve", curveAliases)) d.curve = c;
            if (TryGetFloatParameter(out float f, parameters, "duration", durationAliases)) d.duration = f;
            if (TryGetFloatParameter(out f, parameters, "targetopacity", targetOpacityAliases)) d.targetOpacity = f;
            if (TryGetTypedVector2Parameter(out var tv3, parameters, "anchor", anchorAliases)) d.anchor = tv3.vector;
            if (TryGetVector2Parameter(out var v3, parameters, "direction", "dir")) d.direction = v3;
        }

        public override bool ValidateParameters(IDictionary<string, string> parameters)
        {
            if (parameters == null) return true;

            if (HasNonFloatParameter(parameters, "targetopacity", targetOpacityAliases)) return false;
            if (HasNonFloatParameter(parameters, "duration", durationAliases)) return false;
            if (HasNonAnimCurveParameter(parameters, "curve", curveAliases)) return false;
            if (HasNonTypedVector2Parameter(parameters, "anchor", anchorAliases) || HasVector2OffsetParameter(parameters, "anchor", anchorAliases)) return false;
            if (HasNonVector2Parameter(parameters, "direction", "dir")) return false;
            return true;
        }

        public override object GetNewCustomData()
        {
            return new Data()
            {
                targetOpacity = this.targetOpacity,
                curve = this.curve,
                anchor = anchor,
                duration = this.duration,
                direction = this.direction,

                waitingSince = -1,
                cycleTime = -1,
                lastT = 0
            };
        }

        private readonly string[] targetOpacityAliases = new string[] { "target", "targetop" };
        private readonly string[] anchorAliases = new string[] { "anc", "a" };
        private readonly string[] durationAliases = new string[] { "dur", "d" };
        private readonly string[] curveAliases = new string[] { "crv", "c" };

        private class Data
        {
            public float targetOpacity;
            public AnimationCurve curve;
            public float duration;
            public Vector2 anchor;
            public Vector2 direction;
            public float afterFadeInWaitDuration;

            public int lastT;
            public float waitingSince;
            public bool fadeIn;

            public float cycleTime;

            public readonly float sqrt2 = Mathf.Sqrt(8);
            public readonly float[] dists = new float[4];
        }
    }
}
