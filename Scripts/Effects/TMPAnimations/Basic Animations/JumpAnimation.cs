using System.Collections;
using System.Collections.Generic;
using TMPEffects.Components.CharacterData;
using TMPEffects.Extensions;
using UnityEngine;
using static TMPEffects.ParameterUtility;
using static TMPEffects.TMPAnimations.AnimationUtility;

namespace TMPEffects.TMPAnimations.Animations
{
    [CreateAssetMenu(fileName = "new JumpAnimation", menuName = "TMPEffects/Animations/Jump")]
    public class JumpAnimation : TMPAnimation
    {
        [SerializeField] float amplitude = 20;
        [SerializeField] float waitDuration = 1;
        [SerializeField] float radius = 2;
        [SerializeField] float speed = 4;

        [SerializeField]
        AnimationCurve jumpCurve = AnimationCurveUtility.Bezier
        (
            // Rough approximation of easeInBack and easeOutSine
            new(0, 0),
            new(0.1f, 0),
            new(0.3285747f, -0.14886495f),
            new(0.551f, -0.1052525f),
            new(0.7734149f, -0.06164005f),
            new(0.72f, 1),
            new(1, 1)
        );
        [SerializeField] AnimationCurve fallCurve = AnimationCurveUtility.EaseInBounce();

        public override void Animate(CharData cData, IAnimationContext context)
        {
            Data d = context.customData as Data;

            if (d.waitingSince != -1)
            {
                if (context.animatorContext.PassedTime - d.waitingSince >= d.waitDuration)
                {
                    d.playingSince = context.animatorContext.PassedTime;
                    d.waitingSince = -1f;
                }
                else
                    return;
            }

            float effectiveRadius = d.radius >= context.segmentData.length ? context.segmentData.length : d.radius;
            d.index = (context.animatorContext.PassedTime - d.playingSince) * d.speed - effectiveRadius;

            if (d.index > context.segmentData.length + effectiveRadius)
            {
                // begin waiting
                d.playingSince = -1f;
                d.waitingSince = context.animatorContext.PassedTime;
                return;
            }

            if (Mathf.Abs(context.segmentData.SegmentIndexOf(cData) - d.index) <= effectiveRadius/*1*/)
            {
                float diff = (float)context.segmentData.SegmentIndexOf(cData) - d.index;

                float eval;
                if (diff > 0)
                {
                    eval = GetValue(d.jumpCurve, WrapMode.PingPong, context, diff / effectiveRadius + 1, cData);
                }
                else
                {
                    eval = GetValue(d.fallCurve, WrapMode.PingPong, context, diff / effectiveRadius + 1, cData);
                }

                cData.SetPosition(cData.info.initialPosition + Vector3.up * d.amplitude * eval);
                return;
            }
        }

        public override void SetParameters(object customData, IDictionary<string, string> parameters)
        {
            if (parameters == null) return;

            Data d = customData as Data;
            if (TryGetFloatParameter(out float f, parameters, "speed", SpeedAliases)) d.speed = f;
            if (TryGetFloatParameter(out f, parameters, "amplitude", AmplitudeAliases)) d.amplitude = f;
            if (TryGetFloatParameter(out f, parameters, "waitDuration", waitDurationAliases)) d.waitDuration = f;
            if (TryGetFloatParameter(out f, parameters, "radius", RadiusAliases)) d.radius = f;
            if (TryGetAnimCurveParameter(out AnimationCurve c, parameters, "jumpCurve", "jCurve", "jCrv", "jC")) d.jumpCurve = c;
            if (TryGetAnimCurveParameter(out c, parameters, "fallCurve", "fCurve", "fCrv", "fC")) d.fallCurve = c;
        }

        public override bool ValidateParameters(IDictionary<string, string> parameters)
        {
            if (parameters == null) return true;

            if (HasNonFloatParameter(parameters, "speed", SpeedAliases)) return false;
            if (HasNonFloatParameter(parameters, "amplitude", AmplitudeAliases)) return false;
            if (HasNonFloatParameter(parameters, "waitDuration", waitDurationAliases)) return false;
            if (HasNonFloatParameter(parameters, "radius", RadiusAliases)) return false;
            if (TryGetFloatParameter(out var f, parameters, "radius", RadiusAliases)) if (f <= 0) return false;
            if (HasNonAnimCurveParameter(parameters, "jumpCurve", "jCurve", "jCrv", "jC")) return false;
            if (HasNonAnimCurveParameter(parameters, "fallCurve", "fCurve", "fCrv", "fC")) return false;
            return true;
        }

        public override object GetNewCustomData()
        {
            return new Data()
            {
                speed = this.speed,
                amplitude = this.amplitude,
                waitDuration = this.waitDuration,
                radius = this.radius,
                jumpCurve = this.jumpCurve,
                fallCurve = this.fallCurve,
                jumpTime = new Dictionary<int, float>()
            };
        }

        private readonly string[] waitDurationAliases = new[] { "wait", "waitDur" };

        private class Data
        {
            public float speed;
            public float amplitude;
            public float waitDuration;
            public float radius;
            public AnimationCurve jumpCurve;
            public AnimationCurve fallCurve;

            public float waitingSince;
            public float playingSince;
            public float index;

            public Dictionary<int, float> jumpTime;
        }
    }
}

