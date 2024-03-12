using System.Collections.Generic;
using UnityEngine;
using TMPEffects.Components.CharacterData;
using static TMPEffects.ParameterUtility;
using static TMPEffects.TMPAnimations.AnimationUtility;
using TMPEffects.Extensions;

namespace TMPEffects.TMPAnimations.Animations
{
    [CreateAssetMenu(fileName = "new WaveAnimation", menuName = "TMPEffects/Animations/Wave")]
    public class WaveAnimation : TMPAnimation
    {
        [Tooltip("The speed at which the wave moves.")]
        [SerializeField] private float speed = 2.5f;
        [Tooltip("The frequency of the wave.")]
        [SerializeField] private float frequency = 0.1f;
        [Tooltip("The amplitude of the wave.")]
        [SerializeField] private float amplitude = 10f;
        [Tooltip("The actual wave.")]
        [SerializeField] private AnimationCurve curve = AnimationCurveUtility.EaseInOutSine();

        public override void Animate(CharData cData, IAnimationContext context)
        {
            Data data = (Data)context.customData;

            float xPos = cData.info.initialPosition.x;
            xPos /= (cData.info.referenceScale / 36f);


            float t = context.animatorContext.PassedTime * data.speed + xPos / 1000f * data.frequency;
            float y = data.amplitude * GetValue(data.curve, WrapMode.PingPong, t);
            cData.SetPosition(cData.info.initialPosition + Vector3.up * y);
        }

        public override void SetParameters(object customData, IDictionary<string, string> parameters)
        {
            if (parameters == null) return;

            Data data = (Data)customData;
            if (TryGetFloatParameter(out float val, parameters, "speed", speedAliases)) data.speed = val;
            if (TryGetFloatParameter(out val, parameters, "frequency", frequencyAliases)) data.frequency = val;
            if (TryGetFloatParameter(out val, parameters, "amplitude", amplitudeAliases)) data.amplitude = val;
            if (TryGetAnimCurveParameter(out AnimationCurve curve, parameters, "curve", curveAliases)) data.curve = curve;
        }

        public override bool ValidateParameters(IDictionary<string, string> parameters)
        {
            if (parameters == null) return true;
            if (HasNonFloatParameter(parameters, "speed", speedAliases)) return false;
            if (HasNonFloatParameter(parameters, "frequency", frequencyAliases)) return false;
            if (HasNonFloatParameter(parameters, "amplitude", amplitudeAliases)) return false;
            if (HasNonAnimCurveParameter(parameters, "curve", curveAliases)) return false;
            return true;
        }

        public override object GetNewCustomData()
        {
            return new Data() { amplitude = this.amplitude, frequency = this.frequency, speed = this.speed, curve = this.curve };
        }

        private readonly string[] speedAliases = new string[] { "s", "sp" };
        private readonly string[] frequencyAliases = new string[] { "f", "fq" };
        private readonly string[] amplitudeAliases = new string[] { "a", "amp" };
        private readonly string[] curveAliases = new string[] { "c", "crv" };

        private class Data
        {
            public float speed;
            public float frequency;
            public float amplitude;
            public AnimationCurve curve;
        }
    }
}