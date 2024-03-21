using System.Collections.Generic;
using UnityEngine;
using TMPEffects.CharacterData;
using static TMPEffects.ParameterUtility;
using static TMPEffects.TMPAnimations.AnimationUtility;
using TMPEffects.Extensions;

namespace TMPEffects.TMPAnimations.Animations
{
    [CreateAssetMenu(fileName = "new PulsateAnimation", menuName = "TMPEffects/Animations/Pulsate")]
    public class PulsateAnimation : TMPAnimation
    {
        [SerializeField] float waitDuration = 1.25f;
        [SerializeField] int repetitions = 2;
        [SerializeField] AnimationCurve curve = AnimationCurveUtility.EaseInExpo();
        [SerializeField] float amplitude = 1.2f;
        [SerializeField] float speed = 4;

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

            float t = (context.animatorContext.PassedTime - d.playingSince) * speed;

            if (t >= 2 * d.repetitions)
            {
                // begin waiting
                d.playingSince = -1f;
                d.waitingSince = context.animatorContext.PassedTime;
                return;
            }

            float t2 = GetValue(d.curve, WrapMode.PingPong, t);
            float currAmplitude = Mathf.LerpUnclamped(1, amplitude, t2);

            cData.SetScale(Vector3.one * currAmplitude);
        }

        public override void SetParameters(object customData, IDictionary<string, string> parameters)
        {
            if (parameters == null) return;

            Data d = customData as Data;
            if (TryGetFloatParameter(out float f, parameters, "waitDuration", "wDuration", "wDur")) d.waitDuration = f;
            if (TryGetFloatParameter(out f, parameters, "speed", "sp")) d.speed = f;
            if (TryGetFloatParameter(out f, parameters, "amplitude", "amp")) d.amplitude = f;
            if (TryGetIntParameter(out int i, parameters, "repetitions", "reps")) d.repetitions = i;
            if (TryGetAnimCurveParameter(out var c, parameters, "curve", "crv", "c")) d.curve = c;
        }

        public override bool ValidateParameters(IDictionary<string, string> parameters)
        {
            if (parameters == null) return true;

            if (HasNonFloatParameter(parameters, "waitDuration", "wDuration", "wDur")) return false;
            if (HasNonAnimCurveParameter(parameters, "curve", "crv", "c")) return false;
            if (HasNonFloatParameter(parameters, "speed", "sp")) return false;
            if (HasNonFloatParameter(parameters, "amplitude", "amp")) return false;
            if (HasNonIntParameter(parameters, "repetitions", "reps")) return false;
            return true;
        }

        public override object GetNewCustomData()
        {
            return new Data
            {
                waitDuration = this.waitDuration,
                curve = this.curve,
                speed = this.speed,
                amplitude = this.amplitude,
                repetitions = this.repetitions
            };
        }

        private class Data
        {
            public float waitDuration;
            public AnimationCurve curve;
            public float speed;
            public float amplitude;
            public int repetitions;

            public float waitingSince;
            public float playingSince;
        }
    }
}