using System.Collections.Generic;
using TMPEffects.Components.Animator;
using TMPEffects.Components.CharacterData;
using TMPEffects.Extensions;
using UnityEngine;
using static TMPEffects.ParameterUtility;
using static TMPEffects.TMPAnimations.AnimationUtility;

namespace TMPEffects.TMPAnimations.ShowAnimations
{
    [CreateAssetMenu(fileName = "new FadeHideAnimation", menuName = "TMPEffects/Hide Animations/Fade")]
    public class FadeHideAnimation : TMPHideAnimation
    {
        [SerializeField] float targetOpacity = 0;
        [SerializeField] AnimationCurve curve = AnimationCurveUtility.EaseOutSine();
        [SerializeField] Vector2 anchor = Vector2.zero;
        [SerializeField] float duration = 1;

        public override void Animate(CharData cData, IAnimationContext context)
        {
            ReadOnlyAnimatorContext ac = context.animatorContext;
            Data d = context.customData as Data;

            float t = d.duration > 0 ? Mathf.Clamp01((ac.PassedTime - ac.StateTime(cData)) / d.duration) : 1f;
            float t2 = d.curve.Evaluate(t);

            float opacity = Mathf.LerpUnclamped(cData.info.color.a, d.targetOpacity, t2);

            for (int i = 0; i < 4; i++)
            {
                Color32 c = cData.mesh.initial.GetColor(i);
                c.a = (byte)opacity;
                cData.mesh.SetColor(i, c);
            }

            if (t == 1) context.FinishAnimation(cData);
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
            if (TryGetFloatParameter(out f, parameters, "targetOpacity", startOpacityAliases)) d.targetOpacity = f;
            if (TryGetVector2Parameter(out var v2, parameters, "anchor", anchorAliases)) d.anchor = v2;
        }

        public override bool ValidateParameters(IDictionary<string, string> parameters)
        {
            if (parameters == null) return true;

            if (HasNonFloatParameter(parameters, "targetOpacity", startOpacityAliases)) return false;
            if (HasNonFloatParameter(parameters, "duration", durationAliases)) return false;
            if (HasNonAnimCurveParameter(parameters, "curve", curveAliases)) return false;
            if (HasNonVector2Parameter(parameters, "anchor", anchorAliases)) return false;
            return true;
        }

        public override object GetNewCustomData()
        {
            return new Data()
            {
                targetOpacity = this.targetOpacity,
                curve = this.curve,
                anchor = this.anchor,
                duration = this.duration,

                waitingSince = -1,
                cycleTime = -1,
                lastT = 0
            };
        }

        private readonly string[] startOpacityAliases = new string[] { "target", "targetOp" };
        private readonly string[] anchorAliases = new string[] { "anc" };
        private readonly string[] durationAliases = new string[] { "dur", "d" };
        private readonly string[] curveAliases = new string[] { "crv" };

        private class Data
        {
            public float targetOpacity;
            public AnimationCurve curve;
            public float duration;
            public Vector2 anchor;
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
